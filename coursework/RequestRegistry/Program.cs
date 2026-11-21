using System.Text;
using System.Text.Json;
using RequestRegistry.Models;
using RequestRegistry.Services;

namespace RequestRegistry;

internal static class Program
{
    /// <summary>Файл рабочих данных, с которым работает меню.</summary>
    private const string DataFileName = "requests.json";

    /// <summary>Отдельный файл демонстрационного режима, чтобы не затирать рабочие данные.</summary>
    private const string DemoDataFileName = "requests.demo.json";

    private const string AppFolderName = "RequestRegistry";

    /// <summary>Каталог данных в профиле пользователя: переживает dotnet clean и пересборку.</summary>
    private static string DataDirectory => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), AppFolderName);

    private static string DataPath => Path.Combine(DataDirectory, DataFileName);

    private static string DemoDataPath => Path.Combine(DataDirectory, DemoDataFileName);

    private static async Task Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("Курсовая работа «Учёт заявок на ИТ-поддержку»");
        Console.WriteLine("Дисциплина «Языки программирования», группа БИЦз-22-01");
        Console.WriteLine(new string('=', 110));

        var repository = new RequestRepository();

        if (args.Contains("--demo", StringComparer.OrdinalIgnoreCase))
        {
            await RunDemoAsync(repository, DemoDataPath, args);
            return;
        }

        await RunMenuAsync(repository, DataPath);
    }

    private static async Task RunDemoAsync(RequestRepository repository, string demoPath, string[] args)
    {
        SeedDemoData(repository);

        bool statsOnly = args.Contains("--stats", StringComparer.OrdinalIgnoreCase);
        bool listOnly = args.Contains("--list", StringComparer.OrdinalIgnoreCase);

        if (!statsOnly)
        {
            PrintLists(repository);
        }

        if (!listOnly)
        {
            await PrintStorageAsync(repository, demoPath);
        }

        Console.WriteLine();
        if (!args.Contains("--no-wait"))
        {
            WaitForKey("Демонстрация завершена. Нажмите любую клавишу...");
        }
    }

    private static void SeedDemoData(RequestRepository repository)
    {
        DateTime start = new(2026, 11, 2, 9, 0, 0);

        repository.Add("Арслан", "Бухгалтерия",
            "Не открывается VPN после обновления", RequestPriority.High, start);
        repository.Add("Марина", "Отдел кадров",
            "Не печатает сетевой принтер", RequestPriority.Normal, start.AddDays(1));
        repository.Add("Ильдар", "ИТ-отдел",
            "Нужен доступ к файловому серверу", RequestPriority.Normal, start.AddDays(3));
        repository.Add("Ольга", "Бухгалтерия",
            "Ошибка в 1С при формировании отчёта", RequestPriority.Critical, start.AddDays(4));
        repository.Add("Сергей", "Склад",
            "Медленно работает компьютер", RequestPriority.Low, start.AddDays(5));
        repository.Add("Марина", "Отдел кадров",
            "Восстановить пароль от почты", RequestPriority.High, start.AddDays(7));

        repository.ChangeStatus(3, RequestStatus.InProgress);
        repository.ChangeStatus(4, RequestStatus.InProgress);
        repository.ChangeStatus(5, RequestStatus.Resolved, start.AddDays(10));
        repository.ChangeStatus(6, RequestStatus.Resolved, start.AddDays(9));
    }

    private static void PrintLists(RequestRepository repository)
    {
        PrintSection("Все заявки");
        RequestPrinter.PrintTable(repository.All);

        PrintSection("Только новые заявки");
        RequestPrinter.PrintTable(repository.ByStatus(RequestStatus.New));

        PrintSection("Поиск по слову «VPN»");
        RequestPrinter.PrintTable(repository.Search("VPN"));

        PrintSection("Количество заявок по подразделениям");
        foreach (IGrouping<string, Request> group in repository.ByDepartment())
        {
            Console.WriteLine($"   {group.Key,-20} — {group.Count()}");
        }
    }

    private static async Task PrintStorageAsync(RequestRepository repository, string path)
    {
        PrintSection("Статистика");
        PrintStats(repository);

        PrintSection("Сохранение и загрузка JSON");
        try
        {
            await repository.SaveAsync(path);
            Console.WriteLine($"   Файл сохранён: {path}");

            var loaded = new RequestRepository();
            int count = await loaded.LoadAsync(path);
            Console.WriteLine($"   Из файла загружено заявок: {count}");
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
        {
            Console.WriteLine($"   Ошибка работы с файлом: {ex.Message}");
        }
    }

    private static async Task RunMenuAsync(RequestRepository repository, string dataPath)
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("Меню:");
            Console.WriteLine("  1 — показать все заявки");
            Console.WriteLine("  2 — добавить заявку");
            Console.WriteLine("  3 — фильтр по статусу");
            Console.WriteLine("  4 — статистика");
            Console.WriteLine("  5 — сохранить в JSON");
            Console.WriteLine("  6 — загрузить из JSON");
            Console.WriteLine("  0 — выход");
            Console.Write("Выбор: ");

            string? choice = Console.ReadLine();
            Console.WriteLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        RequestPrinter.PrintTable(repository.All);
                        break;
                    case "2":
                        AddRequest(repository);
                        break;
                    case "3":
                        FilterByStatus(repository);
                        break;
                    case "4":
                        PrintStats(repository);
                        break;
                    case "5":
                        await repository.SaveAsync(dataPath);
                        Console.WriteLine($"Сохранено: {dataPath}");
                        break;
                    case "6":
                        int count = await repository.LoadAsync(dataPath);
                        Console.WriteLine($"Загружено заявок: {count}");
                        break;
                    case "0":
                        Console.WriteLine("Работа завершена.");
                        WaitForKey();
                        return;
                    default:
                        Console.WriteLine("Неизвестная команда.");
                        break;
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"Файл не найден: {dataPath}. Список заявок остаётся пустым.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Некорректные данные: {ex.Message}");
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }

    private static void AddRequest(RequestRepository repository)
    {
        string author = ReadRequired("Автор: ", RequestRepository.MaxAuthorLength);
        string department = ReadRequired("Подразделение: ", RequestRepository.MaxDepartmentLength);
        string description = ReadRequired("Описание: ", RequestRepository.MaxDescriptionLength);
        int priorityNumber = ReadInt(
            "Приоритет (1 — низкий, 2 — обычный, 3 — высокий, 4 — критический): ", 1, 4);

        var priority = (RequestPriority)priorityNumber;
        Request request = repository.Add(author, department, description, priority);

        Console.WriteLine($"Создана заявка № {request.Id}.");
    }

    private static void FilterByStatus(RequestRepository repository)
    {
        int statusNumber = ReadInt(
            "Статус (1 — новая, 2 — в работе, 3 — решена, 4 — отклонена): ", 1, 4);
        var status = (RequestStatus)statusNumber;

        IEnumerable<Request> requests = repository.ByStatus(status);
        RequestPrinter.PrintTable(requests);
    }

    private static void PrintStats(RequestRepository repository)
    {
        Console.WriteLine($"   Всего заявок: {repository.Count}");

        foreach ((RequestStatus status, int count) in repository.CountByStatus())
        {
            Console.WriteLine($"   {RequestPrinter.StatusText(status),-12} — {count}");
        }

        double? average = repository.AverageResolutionDays();
        Console.WriteLine(average is null
            ? "   Среднее время решения: нет закрытых заявок"
            : $"   Среднее время решения: {average:F1} дн.");
    }

    private static void PrintSection(string title)
    {
        Console.WriteLine();
        Console.WriteLine(title);
        Console.WriteLine(new string('-', 110));
    }

    /// <summary>Ожидание клавиши только при интерактивном вводе.</summary>
    private static void WaitForKey(string message = "Нажмите любую клавишу...")
    {
        // При перенаправлённом вводе (echo 0 | dotnet run) Console.ReadKey бросает InvalidOperationException.
        if (Console.IsInputRedirected)
        {
            return;
        }

        Console.WriteLine(message);
        Console.ReadKey();
    }

    private static string ReadRequired(string prompt, int maxLength)
    {
        while (true)
        {
            Console.Write(prompt);
            string value = Console.ReadLine()?.Trim() ?? string.Empty;

            if (value.Length == 0)
            {
                Console.WriteLine("Значение не может быть пустым.");
                continue;
            }

            if (value.Length > maxLength)
            {
                Console.WriteLine($"Слишком длинное значение: не больше {maxLength} символов.");
                continue;
            }

            return value;
        }
    }

    private static int ReadInt(string prompt, int min, int max)
    {
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out int value) && value >= min && value <= max)
            {
                return value;
            }

            Console.WriteLine($"Введите число от {min} до {max}.");
        }
    }
}
