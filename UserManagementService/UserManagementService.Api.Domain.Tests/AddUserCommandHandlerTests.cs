using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UserManagementService.Api.Data;
using UserManagementService.Api.Domain.Commands;
using UserManagementService.Api.Domain.Handlers;
using UserManagementService.Api.Domain.Tests.Database;
using UserManagementService.Shared.Enums;

namespace UserManagementService.Api.Domain.Tests;

public class AddUserCommandHandlerTests : IClassFixture<TestDatabaseFixture>, IDisposable
{
    const string Password = "TestPassed1!";
    const string Username = "SuccessfulTest";
    const UserRole Role = UserRole.Administrator;

    const int UserId = 1;

    private AppDbContext appDbContext;
    private Mapper mapper;
    private TestDatabaseFixture fixture;

    public AddUserCommandHandlerTests(TestDatabaseFixture fixture)
    {
        this.fixture = fixture;

        if(appDbContext == null)
        {
            appDbContext = fixture.CreateContext();
        }

        MapperConfiguration config = new MapperConfiguration(cfg => cfg.AddMaps(typeof(AddUserCommand).Assembly));
        mapper = new Mapper(config);
    }

    [Fact]
    public async Task AddUserCommandHandler_ReturnsAUserResult_MappedFromTheCreatedDatabaseEntity()
    {
        var sut = new AddUserCommandHandler(appDbContext, mapper);

        var response = await sut.Handle(new AddUserCommand(Username, Password, Role), CancellationToken.None);

        Assert.NotNull(response);
        Assert.Equal(Username, response.Username);
        Assert.Equal(Password, response.Password);
        Assert.Equal(Role, response.Role);
    }

    public void Dispose()
    {
        ClearDatabase();
    }

    private async void ClearDatabase()
    {
        await appDbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE [Users]");
        appDbContext.SaveChanges();
    }
}