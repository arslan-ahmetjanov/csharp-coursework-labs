using RequestRegistry.Models;
using RequestRegistry.Services;

namespace RequestRegistry.Tests;

/// <summary>Общие исходные данные для тестов репозитория и хранилища.</summary>
internal static class TestData
{
    public static readonly DateTime Start = new(2026, 11, 2, 9, 0, 0);

    /// <summary>Четыре заявки: id 1–4, авторы Арслан, Марина, Ильдар, Ольга.</summary>
    public static RequestRepository CreateRepository()
    {
        var repository = new RequestRepository();
        repository.Add("Арслан", "Бухгалтерия",
            "Не открывается VPN после обновления", RequestPriority.High, Start);
        repository.Add("Марина", "Отдел кадров",
            "Не печатает сетевой принтер", RequestPriority.Normal, Start.AddDays(1));
        repository.Add("Ильдар", "ИТ-отдел",
            "Нужен доступ к файловому серверу", RequestPriority.Low, Start.AddDays(2));
        repository.Add("Ольга", "Бухгалтерия",
            "Ошибка в 1С при формировании отчёта", RequestPriority.Critical, Start.AddDays(3));
        return repository;
    }
}
