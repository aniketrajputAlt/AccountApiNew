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
    public class AccountsRepositoryTest
    {
        private BankingAppDbContext _context;
        private AccountRepository _repository;

        private AccountsController _controller;

        [TestInitialize]
        public void Initialize()
        {
            var options = new DbContextOptionsBuilder<BankingAppDbContext>()
                .UseSqlServer("Server=(local)\\MSSQLSERVERNEW;Database=NewBankingApp;Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=True")
                .Options;

            _context = new BankingAppDbContext(options);
            _repository = new AccountRepository(_context);
            _controller = new AccountsController(_repository);
        }


        [TestMethod]
        public async Task CreateAccount_ValidInput_ReturnsTrue()
        {
            // Arrange
            var input = new AccountInputModel
            {
                CustomerID = 1,
                TypeID = 1,
                BranchID = "1",
                Balance = 7777
            };

            // Act
            var result = await _repository.CreateAccount(input);

            // Assert
            Assert.IsTrue(result);
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.CustomerID == input.CustomerID);
            Assert.IsNotNull(account);
        }
        [TestMethod]
        public async Task CreateAccount_ValidInputCurrent_ReturnsTrue()
        {
            // Arrange
            var input = new AccountInputModel
            {
                CustomerID = 1,
                TypeID = 2,
                BranchID = "1",
                Balance = 17777
            };

            // Act
            var result = await _repository.CreateAccount(input);

            // Assert
            Assert.IsTrue(result);
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.CustomerID == input.CustomerID);
            Assert.IsNotNull(account);
        }
        [TestMethod]
        public async Task CreateAccount_InvalidCustomerId_ThrowsException()
        {
            // Arrange
            var input = new AccountInputModel
            {
                CustomerID = 99, // Invalid customer ID
                TypeID = 1,
                BranchID = "1",
                Balance = 7777
            };

            // Act & Assert
            await Assert.ThrowsExceptionAsync<Exception>(async () =>
            {
                // Act: Call the repository method with invalid input
                var result = await _repository.CreateAccount(input);
            });
        }
        [TestMethod]
        public async Task CreateAccount_InvalidBrancId_ThrowsException()
        {
            // Arrange
            var input = new AccountInputModel
            {
                CustomerID = 1,
                TypeID = 1,
                BranchID ="aa",
                Balance = 7777
            };
            var exception = await Assert.ThrowsExceptionAsync<Exception>(() => _repository.CreateAccount(input));
            Assert.AreEqual("Branch with the provided ID does not exist.", exception.Message);

        }
        [TestMethod]
        public async Task CreateAccount_InvalidTypeId_ThrowsException()
        {
            // Arrange
            var input = new AccountInputModel
            {
                CustomerID = 1,
                TypeID = 4,
                BranchID = "1",
                Balance = 7777
            };
            var exception = await Assert.ThrowsExceptionAsync<Exception>(() => _repository.CreateAccount(input));
            Assert.AreEqual("Account type with the provided ID does not exist.", exception.Message);
        }
   
        [TestMethod]
        public async Task CreateAccount_InsufficientBalance_ThrowsException()
        {
            // Arrange
            var input = new AccountInputModel
            {
                CustomerID = 1,
                TypeID = 1,
                BranchID = "1",
                Balance = 900
            };

            // Act & Assert
            var exception = await Assert.ThrowsExceptionAsync<Exception>(() => _repository.CreateAccount(input));
            Assert.AreEqual("Account balance is not enough.", exception.Message);
        }

        [TestMethod()]
        public async Task Test_ForGettingAccountsByCustomerId_InvalidId_Controller()
        {
            var invalidId = 300;
            var actionResult = await _controller.GetAccountsByCustomerId(invalidId);

            Assert.IsNotNull(actionResult, "Action result should not be null.");

            var notFoundResult = actionResult as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult, "Expected NotFoundObjectResult, but got null.");

            var resultValue = notFoundResult.Value;
            Assert.IsNotNull(resultValue, "Value should not be null.");

            var actualMessage = resultValue.GetType()?.GetProperty("message")?.GetValue(resultValue, null) as string;
            var expectedMessage = "Accounts not found.";
            Assert.AreEqual(expectedMessage, actualMessage, "Incorrect message returned.");
        }


        [TestMethod]
        public async Task GetAccountById_ValidId_ReturnsAccount()
        {
            // Arrange


            // Act
            var result = await _repository.GetAccountById(6);
            Assert.AreEqual(3, result.CustomerID);


        }

        [TestMethod]
        public async Task GetAccountById_InvalidId_ThrowsException()
        {
            // Arrange

            // Act & Assert
            await Assert.ThrowsExceptionAsync<Exception>(async () =>
            {
                // Act: Call the repository method with invalid input
                var result = await _repository.GetAccountById(99);

            });
            // Act

            // Assert
            // No need for explicit assertion, the ExpectedException attribute handles it
        }

        [TestMethod]
        public async Task DeleteAccount_ValidId_DeletesAccount()
        {
            // Arrange
            long accountIdToDelete = 10072;

            // Act
            await _repository.DeleteAccount(accountIdToDelete);

            var exception = await Assert.ThrowsExceptionAsync<Exception>(() => _repository.GetAccountById(accountIdToDelete));
            Assert.AreEqual("Account was deleted by the user", exception.Message);
           

            // Assert
            // Optionally, you can assert that the account with the given ID no longer exists in the repository
        }



        [TestMethod]
        public async Task DeleteAccount_InvalidId_ThrowsException()
        {
            // Arrange

            // Act & Assert
            await Assert.ThrowsExceptionAsync<Exception>(async () =>
            {
                // Act: Call the repository method with invalid input
                long accountIdToDelete = 100;
                await _repository.DeleteAccount(accountIdToDelete);
            });
            // Act

            // Assert
            // No need for explicit assertion, the ExpectedException attribute handles it
        }

        [TestMethod]
        public async Task Test_ForGettingAllAccountsById()
        {
            var list = await _repository.GetAccountsByCustomerId(3);
            Assert.IsNotNull(list);
            var expectedLength = list.Count;
            var actualLength = 2;
            Assert.AreEqual(actualLength, expectedLength);
        }

        [TestMethod()]
        public async Task Test_ForGettingAllAccountsByWrongId()
        {
            var list = await _repository.GetAccountsByCustomerId(9);
            
            var expectedLength = list.Count;
            var actualLength = 0;
            Assert.AreEqual(actualLength, expectedLength);
        }


        //controller
        [TestMethod]
        public async Task CreateAccount_ReturnsOk_WhenAccountIsCreated()
        {
            // Arrange
            var accountInput = new AccountInputModel
            {
                CustomerID = 2,
                TypeID = 1,
                BranchID = "1",
                Balance = 420420
            };

            // Act
            var result = await _controller.CreateAccount(accountInput);

            // Assert
            Assert.IsNotNull(result);
            if (result is OkObjectResult okResult)
            {
                Assert.AreEqual(200, okResult.StatusCode);
                Assert.AreEqual("Account created successfully.", okResult.Value);
            }
            else
            {
                Assert.Fail("Expected OkObjectResult, but got different result");
            }
        }

        [TestMethod]
        public async Task CreateAccount_ReturnsBadRequest_WhenInputIsNull()
        {
            // Act
            var result = await _controller.CreateAccount(null);

            // Assert
            Assert.IsNotNull(result);
            if (result is BadRequestObjectResult badRequestResult)
            {
                Assert.AreEqual(400, badRequestResult.StatusCode);
                Assert.AreEqual("Invalid parameters", badRequestResult.Value);
            }
            else
            {
                Assert.Fail("Expected BadRequestObjectResult, but got different result");
            }
        }


        [TestMethod]
        public async Task GetAccountById_ReturnsAccount_WhenAccountExists()
        {
            // Act
            var result = await _controller.GetAccountById(3);

            // Assert
            Assert.IsNotNull(result);

            if (result.Result is ObjectResult objectResult)
            {
              

                dynamic resultValue = objectResult.Value;
                Assert.IsNotNull(resultValue);

            }
         
        }

        [TestMethod]
        public async Task GetAccountById_ReturnsBadRequest_WhenExceptionIsThrown()
        {
            // Arrange
            var invalidId = -1;

            // Act
            var result = await _controller.GetAccountById(invalidId);

            // Assert
            Assert.IsNotNull(result);
            if (result.Result is BadRequestObjectResult badRequestResult)
            {
                Assert.AreEqual(400, badRequestResult.StatusCode);
            }
            else
            {
                Assert.Fail("Expected BadRequestObjectResult, but got different result");
            }
        }

        [TestMethod]
        public async Task DeleteAccount_ReturnsOk_WhenAccountIsDeleted()
        {
            // Act
            
            var result = await _controller.DeleteAccount(10003);

            // Assert
            Assert.IsNotNull(result);
            if (result is OkObjectResult okResult)
            {
                Assert.AreEqual(200, okResult.StatusCode);
                Assert.AreEqual("Account deleted successfully.", okResult.Value);
            }
          
        }

        [TestMethod]
        public async Task DeleteAccount_ReturnsBadRequest_WhenExceptionIsThrown()
        {
            // Arrange
            var invalidId = 100;

            // Act
            var result = await _controller.DeleteAccount(invalidId);

            // Assert
            Assert.IsNotNull(result);
            if (result is BadRequestObjectResult badRequestResult)
            {
                Assert.AreEqual(400, badRequestResult.StatusCode);
            }
          
        }

        //

        [TestMethod]
        public async Task GetAccountsByCustomerId_ValidCustomerId_ReturnsOk()
        {
            // Arrange
            int customerId = 3;

            // Act
            var result = await _controller.GetAccountsByCustomerId(customerId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            var accounts = okResult.Value as List<Account>;
            Assert.IsNotNull(accounts);
            Assert.AreEqual(2, accounts.Count);
        }
   
      
    }
}