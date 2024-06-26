﻿using NUnit.Framework;
using UserService.AuthorizationModel;

namespace TestProject
{
    [TestFixture]
    public class AuthenticationTests
    {
        private AuthenticationMock _authenticationMock;

        [SetUp]
        public void Setup()
        {
            _authenticationMock = new AuthenticationMock();
        }

        [Test]
        public void AuthenticateReturnsAdminUserModel()
        {
            // Arrange
            LoginModel loginModel = new LoginModel
            {
                Name = "admin",
                Password = "admin"
            };

            // Act
            UserModel result = _authenticationMock.Authenticate(loginModel);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("admin", result.Username);
            Assert.AreEqual("admin", result.Password);
            Assert.AreEqual(UserRole.Administrator, result.Role);
        }

        [Test]
        public void Authenticate()
        {
            // Arrange
            LoginModel loginModel = new LoginModel
            {
                Name = "user",
                Password = "user"
            };

            // Act
            UserModel result = _authenticationMock.Authenticate(loginModel);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("user", result.Username);
            Assert.AreEqual("user", result.Password);
            Assert.AreEqual(UserRole.User, result.Role);
        }

        [Test]
        public void AuthenticateReturnsNull()
        {
            // Arrange
            LoginModel loginModel = new LoginModel
            {
                Name = "invalid",
                Password = "invalid"
            };

            // Act
            UserModel result = _authenticationMock.Authenticate(loginModel);

            // Assert
            Assert.IsNull(result);
        }
    }
}
