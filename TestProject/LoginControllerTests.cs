using DataBase.BD;
using DataBase.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using System.Security.Claims;
using UserService.AuthorizationModel;
using UserService.Controllers;


namespace TestProject;

[TestFixture]
public class LoginControllerTests
{
    private LoginController _controller;
    private Mock<IUserRepository> _userRepositoryMock;

    [SetUp]
    public void Setup()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _controller = new LoginController(new ConfigurationManager(), _userRepositoryMock.Object);
    }

    [Test]
    public void LoginOk()
    {
        // Arrange
        var userLogin = new LoginModel { Name = "Neitan", Password = "password" };
        var roleId = RoleId.User;
        _userRepositoryMock.Setup(repo => repo.UserCheck(userLogin.Name, userLogin.Password)).Returns(roleId);

        // Act
        var result = _controller.Login(userLogin) as OkObjectResult;

        // Assert
        Assert.IsNull(result);
    }

    [Test]
    public void Login()
    {
        // Arrange
        var userLogin = new LoginModel { Name = "Neitan", Password = "password" };
        _userRepositoryMock.Setup(repo => repo.UserCheck(userLogin.Name, userLogin.Password))
            .Throws(new Exception("Сообщение об ошибке"));

        // Act
        var result = _controller.Login(userLogin) as StatusCodeResult;

        // Assert
        Assert.IsNull(result);
    }

    [Test]
    public void AddAdminOk()
    {
        // Arrange
        var userLogin = new LoginModel { Name = "Neitan", Password = "password" };

        // Act
        var result = _controller.AddAdmin(userLogin) as OkResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
    }

    [Test]
    public void AddAdmin()
    {
        // Arrange
        var userLogin = new LoginModel { Name = "Neitan", Password = "password" };
        _userRepositoryMock.Setup(repo => repo.UserAdd(userLogin.Name, userLogin.Password, RoleId.Admin))
            .Throws(new Exception("Сообщение об ошибке"));

        // Act
        var result = _controller.AddAdmin(userLogin) as StatusCodeResult;

        // Assert
        Assert.IsNull(result);
    }

    [Test]
    public void AddUserOk()
    {
        // Arrange
        var userLogin = new LoginModel { Name = "Neitan", Password = "password" };

        // Act
        var result = _controller.AddUser(userLogin) as OkResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
    }

    [Test]
    public void AddUser()
    {
        // Arrange
        var userLogin = new LoginModel { Name = "Neitan", Password = "password" };
        _userRepositoryMock.Setup(repo => repo.UserAdd(userLogin.Name, userLogin.Password, RoleId.User))
            .Throws(new Exception("Сообщение об ошибке"));

        // Act
        var result = _controller.AddUser(userLogin) as StatusCodeResult;

        // Assert
        Assert.IsNull(result);
    }

    [Test]
    public void GetUserIdOk()
    {
        // Arrange
        var identity = new ClaimsIdentity(new List<Claim> {
            new Claim(ClaimTypes.NameIdentifier, "1984")
        });
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(identity)
            }
        };

        // Act
        var result = _controller.GetUserId() as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        Assert.AreEqual("1984", result.Value);
    }

    [Test]
    public void GetUserIdNull()
    {
        // Arrange
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = null
            }
        };

        // Act
        var result = _controller.GetUserId() as UnauthorizedResult;

        // Assert
        Assert.IsNull(result);
    }
}