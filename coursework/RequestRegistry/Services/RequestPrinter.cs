using RequestRegistry.Models;

namespace RequestRegistry.Services;

public static class RequestPrinter
{
    public static void PrintTable(IEnumerable<Request> requests)
    {
        Console.WriteLine($"{"№",3}  {"Дата",10}  {"Автор",-12}  {"Подразделение",-18}  " +
                          $"{"Приоритет",-10}  {"Статус",-12}  Описание");
        Console.WriteLine(new string('-', 110));

        foreach (Request request in requests)
        {
            string description = request.Description.Length > 34
                ? request.Description[..33] + "…"
                : request.Description;

            Console.WriteLine(
                $"{request.Id,3}  {request.CreatedAt:dd.MM.yyyy}  {request.Author,-12}  " +
                $"{request.Department,-18}  {PriorityText(request.Priority),-10}  " +
                $"{StatusText(request.Status),-12}  {description}");
        }
    }

    public static string PriorityText(RequestPriority priority) => priority switch
    {
        RequestPriority.Low => "Низкий",
        RequestPriority.Normal => "Обычный",
        RequestPriority.High => "Высокий",
        RequestPriority.Critical => "Критический",
        _ => priority.ToString()
    };

    public static string StatusText(RequestStatus status) => status switch
    {
        RequestStatus.New => "Новая",
        RequestStatus.InProgress => "В работе",
        RequestStatus.Resolved => "Решена",
        RequestStatus.Rejected => "Отклонена",
        _ => status.ToString()
    };
}
