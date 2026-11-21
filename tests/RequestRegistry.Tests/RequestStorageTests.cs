using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using RequestRegistry.Models;
using RequestRegistry.Services;
using Xunit;

namespace RequestRegistry.Tests;

public sealed class RequestStorageTests
{
    [Fact]
    public async Task SaveAndLoad_RoundTripsRequests()
    {
        using var temp = new TempDirectory();
        string path = temp.FilePath("requests.json");
        RequestRepository repository = TestData.CreateRepository();
        repository.ChangeStatus(3, RequestStatus.Resolved, TestData.Start.AddDays(7));

        await repository.SaveAsync(path, CancellationToken.None);

        var loaded = new RequestRepository();
        int count = await loaded.LoadAsync(path, CancellationToken.None);

        Assert.Equal(4, count);
        Request expected = repository.Find(3)!;
        Request actual = loaded.Find(3)!;
        Assert.Equal(expected.Author, actual.Author);
        Assert.Equal(expected.Department, actual.Department);
        Assert.Equal(expected.Description, actual.Description);
        Assert.Equal(expected.Priority, actual.Priority);
        Assert.Equal(expected.Status, actual.Status);
        Assert.Equal(expected.CreatedAt, actual.CreatedAt);
        Assert.Equal(expected.ClosedAt, actual.ClosedAt);
        Assert.Equal(expected.ResolutionDays, actual.ResolutionDays);
    }

    [Fact]
    public async Task SaveAsync_OverwritesExistingFile()
    {
        using var temp = new TempDirectory();
        string path = temp.FilePath("requests.json");
        await TestData.CreateRepository().SaveAsync(path, CancellationToken.None);

        var smaller = new RequestRepository();
        smaller.Add("Сергей", "Склад", "Медленно работает компьютер", RequestPriority.Low, TestData.Start);
        await smaller.SaveAsync(path, CancellationToken.None);

        var loaded = new RequestRepository();
        await loaded.LoadAsync(path, CancellationToken.None);

        Assert.Equal(1, loaded.Count);
    }

    [Fact]
    public async Task SaveAsync_LeavesNoTemporaryFiles()
    {
        using var temp = new TempDirectory();
        string path = temp.FilePath("requests.json");

        await TestData.CreateRepository().SaveAsync(path, CancellationToken.None);

        Assert.Empty(Directory.GetFiles(temp.Root, "*.tmp"));
    }

    [Fact]
    public async Task SaveAsync_CreatesMissingDirectory()
    {
        using var temp = new TempDirectory();
        string path = Path.Combine(temp.Root, "nested", "requests.json");

        await TestData.CreateRepository().SaveAsync(path, CancellationToken.None);

        Assert.True(File.Exists(path));
    }

    [Fact]
    public async Task SaveAsync_WritesValidJson()
    {
        using var temp = new TempDirectory();
        string path = temp.FilePath("requests.json");
        RequestRepository repository = TestData.CreateRepository();
        repository.ChangeStatus(3, RequestStatus.Resolved, TestData.Start.AddDays(5));

        await repository.SaveAsync(path, CancellationToken.None);

        string json = await File.ReadAllTextAsync(path, CancellationToken.None);
        Assert.Contains("Бухгалтерия", json, StringComparison.Ordinal);
        Assert.Contains("Resolved", json, StringComparison.Ordinal);
    }

    [Fact]
    public async Task LoadAsync_MissingFile_ThrowsFileNotFoundException()
    {
        using var temp = new TempDirectory();
        string path = temp.FilePath("missing.json");
        var repository = new RequestRepository();

        await Assert.ThrowsAsync<FileNotFoundException>(
            () => repository.LoadAsync(path, CancellationToken.None));
    }

    [Fact]
    public async Task LoadAsync_RecalculatesNextId()
    {
        using var temp = new TempDirectory();
        string path = temp.FilePath("requests.json");
        await TestData.CreateRepository().SaveAsync(path, CancellationToken.None);

        var loaded = new RequestRepository();
        await loaded.LoadAsync(path, CancellationToken.None);

        Request added = loaded.Add(
            "Новый", "ИТ-отдел", "Заявка после загрузки", RequestPriority.Normal, TestData.Start);

        Assert.Equal(5, added.Id);
    }

    [Fact]
    public async Task LoadAsync_EmptyFileContent_StartsFromEmptyList()
    {
        using var temp = new TempDirectory();
        string path = temp.FilePath("requests.json");
        await File.WriteAllTextAsync(path, "[]", CancellationToken.None);
        var repository = new RequestRepository();

        int count = await repository.LoadAsync(path, CancellationToken.None);

        Assert.Equal(0, count);
        Assert.Empty(repository.All);
    }
}
