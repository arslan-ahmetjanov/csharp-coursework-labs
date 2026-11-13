using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using RequestRegistry.Models;

namespace RequestRegistry.Services;

public sealed class RequestRepository
{
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

    public Request Add(
        string author,
        string department,
        string description,
        RequestPriority priority,
        DateTime? createdAt = null)
    {
        var request = new Request
        {
            Id = _nextId++,
            CreatedAt = createdAt ?? DateTime.Now,
            Author = author.Trim(),
            Department = department.Trim(),
            Description = description.Trim(),
            Priority = priority,
            Status = RequestStatus.New
        };

        _requests.Add(request);
        return request;
    }

    public Request? Find(int id) => _requests.FirstOrDefault(request => request.Id == id);

    public bool ChangeStatus(int id, RequestStatus status, DateTime? closedAt = null)
    {
        Request? request = Find(id);
        if (request is null)
        {
            return false;
        }

        request.Status = status;
        request.ClosedAt = status is RequestStatus.Resolved or RequestStatus.Rejected
            ? closedAt ?? DateTime.Now
            : null;
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

    public double? AverageResolutionDays()
    {
        List<double> days = _requests
            .Where(request => request.ResolutionDays.HasValue)
            .Select(request => request.ResolutionDays!.Value)
            .ToList();

        return days.Count == 0 ? null : days.Average();
    }

    public async Task SaveAsync(string path, CancellationToken cancellationToken = default)
    {
        await using FileStream stream = File.Create(path);
        await JsonSerializer.SerializeAsync(stream, _requests, JsonOptions, cancellationToken);
    }

    public async Task<int> LoadAsync(string path, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("Файл с заявками не найден.", path);
        }

        await using FileStream stream = File.OpenRead(path);
        List<Request> loaded = await JsonSerializer.DeserializeAsync<List<Request>>(
            stream, JsonOptions, cancellationToken) ?? new List<Request>();

        _requests.Clear();
        _requests.AddRange(loaded);
        _nextId = _requests.Count == 0 ? 1 : _requests.Max(request => request.Id) + 1;
        return _requests.Count;
    }
}
