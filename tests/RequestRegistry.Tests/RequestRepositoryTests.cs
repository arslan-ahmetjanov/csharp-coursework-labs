using System;
using System.Linq;
using RequestRegistry.Models;
using RequestRegistry.Services;
using Xunit;

namespace RequestRegistry.Tests;

public sealed class RequestRepositoryTests
{
    [Fact]
    public void Add_AssignsSequentialIds()
    {
        RequestRepository repository = TestData.CreateRepository();

        Assert.Equal(4, repository.Count);
        Assert.Equal(new[] { 1, 2, 3, 4 }, repository.All.Select(request => request.Id));
    }

    [Fact]
    public void Add_TrimsTextFieldsAndSetsNewStatus()
    {
        var repository = new RequestRepository();
        DateTime createdAt = TestData.Start;

        Request request = repository.Add(
            "  Арслан  ", "  ИТ-отдел  ", "  Описание заявки  ", RequestPriority.High, createdAt);

        Assert.Equal(1, request.Id);
        Assert.Equal("Арслан", request.Author);
        Assert.Equal("ИТ-отдел", request.Department);
        Assert.Equal("Описание заявки", request.Description);
        Assert.Equal(RequestPriority.High, request.Priority);
        Assert.Equal(RequestStatus.New, request.Status);
        Assert.Equal(createdAt, request.CreatedAt);
        Assert.Null(request.ClosedAt);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    public void Add_BlankAuthor_ThrowsArgumentException(string author)
    {
        var repository = new RequestRepository();

        var exception = Assert.Throws<ArgumentException>(
            () => repository.Add(author, "ИТ-отдел", "Описание", RequestPriority.Normal));

        Assert.Equal("author", exception.ParamName);
        Assert.Contains("Автор", exception.Message);
        Assert.Empty(repository.All);
    }

    [Fact]
    public void Add_BlankDepartment_ThrowsArgumentException()
    {
        var repository = new RequestRepository();

        var exception = Assert.Throws<ArgumentException>(
            () => repository.Add("Арслан", " ", "Описание", RequestPriority.Normal));

        Assert.Equal("department", exception.ParamName);
    }

    [Fact]
    public void Add_BlankDescription_ThrowsArgumentException()
    {
        var repository = new RequestRepository();

        var exception = Assert.Throws<ArgumentException>(
            () => repository.Add("Арслан", "ИТ-отдел", "   ", RequestPriority.Normal));

        Assert.Equal("description", exception.ParamName);
    }

    [Fact]
    public void Add_TooLongAuthor_ThrowsArgumentException()
    {
        var repository = new RequestRepository();
        string author = new('А', RequestRepository.MaxAuthorLength + 1);

        var exception = Assert.Throws<ArgumentException>(
            () => repository.Add(author, "ИТ-отдел", "Описание", RequestPriority.Normal));

        Assert.Equal("author", exception.ParamName);
    }

    [Fact]
    public void Add_TooLongDescription_ThrowsArgumentException()
    {
        var repository = new RequestRepository();
        string description = new('x', RequestRepository.MaxDescriptionLength + 1);

        var exception = Assert.Throws<ArgumentException>(
            () => repository.Add("Арслан", "ИТ-отдел", description, RequestPriority.Normal));

        Assert.Equal("description", exception.ParamName);
    }

    [Fact]
    public void Add_MaxLengthDescription_IsAccepted()
    {
        var repository = new RequestRepository();
        string description = new('x', RequestRepository.MaxDescriptionLength);

        Request request = repository.Add("Арслан", "ИТ-отдел", description, RequestPriority.Normal);

        Assert.Equal(RequestRepository.MaxDescriptionLength, request.Description.Length);
    }

    [Fact]
    public void Add_UndefinedPriority_ThrowsArgumentException()
    {
        var repository = new RequestRepository();

        var exception = Assert.Throws<ArgumentException>(
            () => repository.Add("Арслан", "ИТ-отдел", "Описание", (RequestPriority)42));

        Assert.Equal("priority", exception.ParamName);
        Assert.Empty(repository.All);
    }

    [Fact]
    public void ByStatus_ReturnsOnlyMatchingRequests()
    {
        RequestRepository repository = TestData.CreateRepository();
        repository.ChangeStatus(2, RequestStatus.InProgress);
        repository.ChangeStatus(3, RequestStatus.Resolved, TestData.Start.AddDays(5));

        int[] ids = repository.ByStatus(RequestStatus.New).Select(request => request.Id).ToArray();

        Assert.Equal(new[] { 1, 4 }, ids);
    }

    [Fact]
    public void ByStatus_NoMatches_ReturnsEmptySequence()
    {
        RequestRepository repository = TestData.CreateRepository();

        Assert.Empty(repository.ByStatus(RequestStatus.Rejected));
    }

    [Fact]
    public void ByPriority_ReturnsOnlyMatchingRequests()
    {
        RequestRepository repository = TestData.CreateRepository();

        int[] ids = repository.ByPriority(RequestPriority.Critical)
            .Select(request => request.Id)
            .ToArray();

        Assert.Equal(new[] { 4 }, ids);
    }

    [Fact]
    public void ByPriority_NoMatches_ReturnsEmptySequence()
    {
        var repository = new RequestRepository();
        repository.Add("Арслан", "ИТ-отдел", "Описание", RequestPriority.Low, TestData.Start);

        Assert.Empty(repository.ByPriority(RequestPriority.Critical));
    }

    [Fact]
    public void Search_FindsRequestsByAuthorDepartmentAndDescription()
    {
        // Отдельного поля «исполнитель» в модели нет, поиск по автору — ближайший аналог.
        RequestRepository repository = TestData.CreateRepository();

        Assert.Equal(new[] { 2 }, repository.Search("марина").Select(request => request.Id).ToArray());
        Assert.Equal(new[] { 1, 4 }, repository.Search("Бухгалтерия").Select(request => request.Id).ToArray());
        Assert.Equal(new[] { 1 }, repository.Search("VPN").Select(request => request.Id).ToArray());
    }

    [Fact]
    public void Search_UnknownText_ReturnsEmptySequence()
    {
        RequestRepository repository = TestData.CreateRepository();

        Assert.Empty(repository.Search("нет такого слова"));
    }

    [Fact]
    public void ChangeStatus_Resolved_SetsClosedAtAndResolutionDays()
    {
        RequestRepository repository = TestData.CreateRepository();
        DateTime closedAt = TestData.Start.AddDays(5);

        bool changed = repository.ChangeStatus(1, RequestStatus.Resolved, closedAt);

        Assert.True(changed);
        Request request = repository.Find(1)!;
        Assert.Equal(RequestStatus.Resolved, request.Status);
        Assert.Equal(closedAt, request.ClosedAt);
        Assert.Equal(5.0, request.ResolutionDays);
    }

    [Fact]
    public void ChangeStatus_Rejected_AlsoClosesRequest()
    {
        RequestRepository repository = TestData.CreateRepository();
        DateTime closedAt = TestData.Start.AddDays(2);

        repository.ChangeStatus(1, RequestStatus.Rejected, closedAt);

        Request request = repository.Find(1)!;
        Assert.Equal(RequestStatus.Rejected, request.Status);
        Assert.Equal(closedAt, request.ClosedAt);
    }

    [Fact]
    public void ChangeStatus_BackToInProgress_ClearsClosedAt()
    {
        RequestRepository repository = TestData.CreateRepository();
        repository.ChangeStatus(1, RequestStatus.Resolved, TestData.Start.AddDays(3));

        repository.ChangeStatus(1, RequestStatus.InProgress);

        Request request = repository.Find(1)!;
        Assert.Equal(RequestStatus.InProgress, request.Status);
        Assert.Null(request.ClosedAt);
        Assert.Null(request.ResolutionDays);
    }

    [Fact]
    public void ChangeStatus_UnknownId_ReturnsFalse()
    {
        RequestRepository repository = TestData.CreateRepository();

        Assert.False(repository.ChangeStatus(999, RequestStatus.Resolved));
    }

    [Fact]
    public void ChangeStatus_UndefinedStatus_ThrowsArgumentException()
    {
        RequestRepository repository = TestData.CreateRepository();

        var exception = Assert.Throws<ArgumentException>(
            () => repository.ChangeStatus(1, (RequestStatus)99));

        Assert.Equal("status", exception.ParamName);
    }

    [Fact]
    public void ChangeStatus_ClosedBeforeCreated_DoesNotProduceNegativeResolutionDays()
    {
        RequestRepository repository = TestData.CreateRepository();

        repository.ChangeStatus(1, RequestStatus.Resolved, TestData.Start.AddDays(-10));

        Request request = repository.Find(1)!;
        Assert.Equal(request.CreatedAt, request.ClosedAt);
        Assert.Equal(0.0, request.ResolutionDays);
    }

    [Fact]
    public void AverageResolutionDays_NoClosedRequests_ReturnsNull()
    {
        RequestRepository repository = TestData.CreateRepository();

        Assert.Null(repository.AverageResolutionDays());
    }

    [Fact]
    public void AverageResolutionDays_ComputesMeanOfClosedRequests()
    {
        RequestRepository repository = TestData.CreateRepository();
        repository.ChangeStatus(1, RequestStatus.Resolved, TestData.Start.AddDays(2));
        repository.ChangeStatus(2, RequestStatus.Rejected, TestData.Start.AddDays(1).AddDays(4));
        repository.ChangeStatus(3, RequestStatus.InProgress);

        double? average = repository.AverageResolutionDays();

        Assert.True(average.HasValue);
        Assert.Equal(3.0, average.Value, 6);
    }

    [Fact]
    public void CountByStatus_CountsEveryStatus()
    {
        RequestRepository repository = TestData.CreateRepository();
        repository.ChangeStatus(1, RequestStatus.InProgress);
        repository.ChangeStatus(2, RequestStatus.Resolved, TestData.Start.AddDays(2));

        var counts = repository.CountByStatus();

        Assert.Equal(2, counts[RequestStatus.New]);
        Assert.Equal(1, counts[RequestStatus.InProgress]);
        Assert.Equal(1, counts[RequestStatus.Resolved]);
        Assert.Equal(0, counts[RequestStatus.Rejected]);
    }
}
