using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using RequestRegistry.Models;

namespace RequestRegistry.Services;

public sealed class RequestRepository
{
    /// <summary>Максимальная длина поля «Автор».</summary>
    public const int MaxAuthorLength = 100;

    /// <summary>Максимальная длина поля «Подразделение».</summary>
    public const int MaxDepartmentLength = 100;

    /// <summary>Максимальная длина поля «Описание».</summary>
    public const int MaxDescriptionLength = 500;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly List<Request> _requests = new();
    private int _nextId = 1;

    public IReadOnlyList<Request> All => _requests;

    public int Count => _requests.Count;

    /// <summary>
    /// Добавляет заявку. Входные данные проверяются здесь, а не только в интерфейсе,
    /// чтобы некорректная заявка не попала в хранилище ни из меню, ни из другого кода.
    /// При пустом или слишком длинном поле, а также недопустимом приоритете
    /// выбрасывается ArgumentException с именем проблемного параметра.
    /// </summary>
    public Request Add(
        string author,
        string department,
        string description,
        RequestPriority priority,
        DateTime? createdAt = null)
    {
        string authorText = ValidateText(author, nameof(author), "Автор", MaxAuthorLength);
        string departmentText = ValidateText(
            department, nameof(department), "Подразделение", MaxDepartmentLength);
        string descriptionText = ValidateText(
            description, nameof(description), "Описание", MaxDescriptionLength);

        if (!Enum.IsDefined(priority))
        {
            throw new ArgumentException(
                $"Поле «Приоритет» содержит недопустимое значение {(int)priority}.",
                nameof(priority));
        }

        var request = new Request
        {
            Id = _nextId++,
            CreatedAt = createdAt ?? DateTime.Now,
            Author = authorText,
            Department = departmentText,
            Description = descriptionText,
            Priority = priority,
            Status = RequestStatus.New
        };

        _requests.Add(request);
        return request;
    }

    public Request? Find(int id) => _requests.FirstOrDefault(request => request.Id == id);

    public bool ChangeStatus(int id, RequestStatus status, DateTime? closedAt = null)
    {
        if (!Enum.IsDefined(status))
        {
            throw new ArgumentException(
                $"Поле «Статус» содержит недопустимое значение {(int)status}.",
                nameof(status));
        }

        Request? request = Find(id);
        if (request is null)
        {
            return false;
        }

        request.Status = status;

        if (status is RequestStatus.Resolved or RequestStatus.Rejected)
        {
            DateTime moment = closedAt ?? DateTime.Now;

            // Заявку нельзя закрыть раньше, чем она создана: иначе срок решения станет отрицательным.
            request.ClosedAt = moment < request.CreatedAt ? request.CreatedAt : moment;
        }
        else
        {
            request.ClosedAt = null;
        }

        return true;
    }

    public IEnumerable<Request> ByStatus(RequestStatus status) =>
        _requests.Where(request => request.Status == status);

    public IEnumerable<Request> ByPriority(RequestPriority priority) =>
        _requests.Where(request => request.Priority == priority);

    public IEnumerable<Request> Search(string text)
    {
        string query = text.Trim();
        return _requests.Where(request =>
            request.Description.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            request.Author.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            request.Department.Contains(query, StringComparison.OrdinalIgnoreCase));
    }

    public IEnumerable<IGrouping<string, Request>> ByDepartment() =>
        _requests
            .GroupBy(request => request.Department)
            .OrderByDescending(group => group.Count())
            .ThenBy(group => group.Key);

    public IReadOnlyDictionary<RequestStatus, int> CountByStatus() =>
        Enum.GetValues<RequestStatus>()
            .ToDictionary(status => status, status => _requests.Count(request => request.Status == status));

    /// <summary>Средний срок решения закрытых заявок в днях; null, если закрытых заявок нет.</summary>
    public double? AverageResolutionDays()
    {
        List<double> days = _requests
            .Where(request => request.ClosedAt is not null)
            .Select(request => request.ResolutionDays!.Value)
            .Where(value => double.IsFinite(value))
            .ToList();

        return days.Count == 0 ? null : days.Average();
    }

    /// <summary>
    /// Сохраняет заявки атомарно: сначала во временный файл в том же каталоге,
    /// затем подменяет целевой файл. Падение процесса не оставляет пустой JSON.
    /// </summary>
    public async Task SaveAsync(string path, CancellationToken cancellationToken = default)
    {
        string fullPath = Path.GetFullPath(path);
        string? directory = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        string tempPath = Path.Combine(
            directory ?? ".",
            $"{Path.GetFileName(fullPath)}.{Guid.NewGuid():N}.tmp");

        try
        {
            await using (FileStream stream = new(
                tempPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                await JsonSerializer.SerializeAsync(stream, _requests, JsonOptions, cancellationToken);
                await stream.FlushAsync(cancellationToken);
            }

            File.Move(tempPath, fullPath, overwrite: true);
        }
        catch
        {
            TryDeleteFile(tempPath);
            throw;
        }
    }

    public async Task<int> LoadAsync(string path, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("Файл с заявками не найден.", path);
        }

        List<Request> loaded;
        await using (FileStream stream = new(
            path, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            loaded = await JsonSerializer.DeserializeAsync<List<Request>>(
                stream, JsonOptions, cancellationToken) ?? new List<Request>();
        }

        _requests.Clear();
        _requests.AddRange(loaded.Where(request => request is not null));
        _nextId = _requests.Count == 0 ? 1 : _requests.Max(request => request.Id) + 1;
        return _requests.Count;
    }

    private static string ValidateText(string value, string paramName, string fieldName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"Поле «{fieldName}» не может быть пустым.", paramName);
        }

        string trimmed = value.Trim();
        if (trimmed.Length > maxLength)
        {
            throw new ArgumentException(
                $"Поле «{fieldName}» не может быть длиннее {maxLength} символов.", paramName);
        }

        return trimmed;
    }

    private static void TryDeleteFile(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch (IOException)
        {
            // Временный файл не удалось удалить — не скрываем исходную ошибку сохранения.
        }
        catch (UnauthorizedAccessException)
        {
            // То же самое: приоритет у исходного исключения.
        }
    }
}
