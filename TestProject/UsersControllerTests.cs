using AccountApiNew.Controllers;
using AccountApiNew.Model;
using AccountApiNew.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountApiNewTesting
{
    [TestClass]
    public class UsersControllerTests
    {
        private UsersController _controller;
        private UserRepository _repository;
        private DbContextOptions<BankingAppDbContext> _options;

        [TestInitialize]
        public void Setup()
        {
            // Initialize the DbContextOptions properly
            _options = new DbContextOptionsBuilder<BankingAppDbContext>()
                .UseSqlServer("Server=(local)\\MSSQLSERVERNEW;Database=NewBankingApp;Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=True")
                .Options;

            var context = new BankingAppDbContext(_options);
            _repository = new UserRepository(context);
            _controller = new UsersController(_repository);
        }

        [TestMethod]
        public async Task ChangePassword_ReturnsOk_WhenPasswordIsUpdated()
        {
            // Arrange: Setup a valid model
            var model = new PasswordChangeModel
            {
                Username = "john_doe",
                NewPassword = "12345678"
            };

            // Act: Call the ChangePassword method
            var result = await _controller.ChangePassword(model);

            // Assert: Check that the response is OK 
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

        }

        [TestMethod]
        public async Task ChangePassword_ReturnsBadRequest_WhenUserDoesNotExist()
        {
            // Arrange: Setup model for a non-existent user
            var model = new PasswordChangeModel
            {
                Username = "nonexistent",
                NewPassword = "newPassword"
            };

            // Act
            var result = await _controller.ChangePassword(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);

        }

        [TestMethod]
        public async Task ChangePassword_ReturnsBadRequest_WhenPasswordIsInvalid()
        {
            // Arrange: Setup a model with an invalid password
            var model = new PasswordChangeModel
            {
                Username = "john_doe",
                NewPassword = "short"
            };

            // Act
            var result = await _controller.ChangePassword(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);

        }
        [TestMethod]
        public async Task ChangePassword_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange: Setup the controller to have an invalid model state
            var model = new PasswordChangeModel
            {
                Username = "",  // Invalid username
                NewPassword = "validPassword"
            };

            _controller.ModelState.AddModelError("Username", "Username is required");

            // Act
            var result = await _controller.ChangePassword(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);

        
        }


    }
}
