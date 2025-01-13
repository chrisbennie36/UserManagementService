using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using UserManagementService.Api.Data;
using UserManagementService.Api.Data.Entities;
using UserManagementService.Api.Data.Repositories;
using UserManagementService.Api.Domain.Commands;
using UserManagementService.Api.Domain.Handlers;
using UserManagementService.Api.Domain.Tests.Database;
using UserManagementService.Shared.Enums;
using Utilities.ResultPattern;

namespace UserManagementService.Api.Domain.Tests;

public class AddUserCommandHandlerTests : IClassFixture<TestDatabaseFixture>, IDisposable
{
    const string Password = "TestPassed1!";
    const string Username = "SuccessfulTest";
    const UserRole Role = UserRole.Administrator;

    const int UserId = 1;

    private AppDbContext appDbContext;
    private Mock<IEntityRepository<User>> userRepositoryMock;
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

        userRepositoryMock = new Mock<IEntityRepository<User>>();
    }

    [Fact]
    public async Task AddUserCommandHandler_ReturnsASuccessResult()
    {
        userRepositoryMock.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>())).ReturnsAsync(new User());
        var sut = new AddUserCommandHandler(mapper, userRepositoryMock.Object);

        var response = await sut.Handle(new AddUserCommand(Username, Password, Role), CancellationToken.None);

        Assert.NotNull(response);
        Assert.Equal(ResponseStatus.Success, response.status);
    }

    [Fact]
    public async Task AddUserCommandHandler_ReturnsASuccessResult_WithMappedFieldsFromCreatedEntity()
    {
        User user = new User
        {
            Username = Username,
            Password = Password,
            Role = Role
        };

        userRepositoryMock.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>())).ReturnsAsync(user);
        var sut = new AddUserCommandHandler(mapper, userRepositoryMock.Object);

        var response = await sut.Handle(new AddUserCommand(Username, Password, Role), CancellationToken.None);

        Assert.NotNull(response);
        Assert.NotNull(response.resultModel);
        Assert.Equal(user.Username, response.resultModel.Username);
        Assert.Equal(user.Password, response.resultModel.Password);
        Assert.Equal(user.Role, response.resultModel.Role);
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