using AccountApiNew.Controllers;
using AccountApiNew.Model;
using AccountApiNew.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountApiNewTesting
{
    [TestClass]
    public class TransactionRepositoryTesting
    {
        private BankingAppDbContext _context;
        private TransactionRepository _repository;
        private TransactionsController _controller;

        [TestInitialize]
        public void Initialize()
        {
            var options = new DbContextOptionsBuilder<BankingAppDbContext>()
                .UseSqlServer("Server=(local)\\MSSQLSERVERNEW;Database=NewBankingApp;Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=True")
                .Options;
           
            _context = new BankingAppDbContext(options);
            _repository = new TransactionRepository(_context);
            _controller = new TransactionsController(_repository);
        }
       
        [TestMethod]
        public async Task FundTransfer_ValidTransfer_ReturnsTrue()
        {
            long aid = 2, bid = 3;
            decimal amount = 10.0m;
            // Act
            var result = await _repository.FundTransfer(aid, bid, amount);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(result);
        }
        [TestMethod]
        public async Task FundTransfer_SourceAccountDoesNotExist_ThrowsException()
        {
            // Act & Assert
            var exception = await Assert.ThrowsExceptionAsync<Exception>(() => _repository.FundTransfer(55, 6, 1000));
            Assert.AreEqual("Source account does not exist.", exception.Message);
        }

        [TestMethod]
        public async Task FundTransfer_DestinationAccountDoesNotExist_ThrowsException()
        {
            // Act & Assert
            var exception = await Assert.ThrowsExceptionAsync<Exception>(() => _repository.FundTransfer(5, 66, 1000));
            Assert.AreEqual("Destination account does not exist.", exception.Message);
        }

        [TestMethod]
        public async Task FundTransfer_InsufficientBalanceSavings_ThrowsException()
        {
            // Act & Assert
            var exception = await Assert.ThrowsExceptionAsync<Exception>(() => _repository.FundTransfer(4, 6, 400500));
            Assert.AreEqual("Insufficient balance in Savings account.", exception.Message);
        }

        [TestMethod]
        public async Task FundTransfer_InsufficientBalanceCurrent_ThrowsException()
        {
            // Act & Assert
            var exception = await Assert.ThrowsExceptionAsync<Exception>(() => _repository.FundTransfer(5, 6, 400500));
            Assert.AreEqual("Insufficient balance in Current account.", exception.Message);
        }

        [TestMethod]
        public async Task FundTransfer_SameSourceAndDestinationAccount_ThrowsException()
        {
            // Act & Assert
            var exception = await Assert.ThrowsExceptionAsync<Exception>(() => _repository.FundTransfer(5, 5, 1));
            Assert.AreEqual("Source account same as Destination account.", exception.Message);
        }

        [TestMethod]
        public async Task FundTransfer_ZeroAmount_ThrowsException()
        {
            // Act & Assert
            var exception = await Assert.ThrowsExceptionAsync<Exception>(() => _repository.FundTransfer(5, 6, 0));
            Assert.AreEqual("Amount must be greater than zero.", exception.Message);
        }

        [TestMethod]
        public async Task FundTransfer_NegativeAmount_ThrowsException()
        {
            // Act & Assert
            var exception = await Assert.ThrowsExceptionAsync<Exception>(() => _repository.FundTransfer(5, 6, -1000));
            Assert.AreEqual("Amount must be greater than zero.", exception.Message);
        }

      



        [TestMethod]
        public async Task TransactionsList_ShouldReturnTransactions_ForGivenAccount()
        {
            // Act
            var result = await _repository.TransactionsList(10006);

            // Assert
            Assert.AreEqual(1, result.Count());
         
        }

        [TestMethod]
        public async Task TransactionsList_ShouldReturnEmpty_ForAccountWithNoTransactions()
        {
            // Act
            var result = await _repository.TransactionsList(10000001);

            // Assert
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public async Task TransactionsList_ShouldHandleInvalidAccountId()
        {
            // Act
            var result = await _repository.TransactionsList(-1);

            // Assert
            Assert.AreEqual(0, result.Count());
        }

        //controller testing
        [TestMethod]
        public async Task TransferFunds_ValidInput_ReturnsOk()
        {
            // Arrange
            long sourceAccountId = 2, destinationAccountId = 3;
            decimal amount = 10.0m;
            FundTransferModel model = new FundTransferModel();
            model.sourceAccountId = sourceAccountId;
            model.destinationAccountId = destinationAccountId;
            model.amount = amount;

            // Act
            var result = await _controller.TransferFunds(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.AreEqual("Fund transfer successful.", okResult.Value);
        }

        [TestMethod]
        public async Task TransferFunds_InvalidInput_ReturnsBadRequest()
        {
            // Arrange
            long sourceAccountId = -1, destinationAccountId = 3;
            decimal amount = 100.0m;
            FundTransferModel model = new FundTransferModel();
            model.sourceAccountId = sourceAccountId;
            model.destinationAccountId = destinationAccountId;
            model.amount = amount;

            // Act
            var result = await _controller.TransferFunds(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Invalid input parameters.", badRequestResult.Value);
        }

     


        [TestMethod]
        public async Task GetTransactions_ValidAccountId_ReturnsTransactions()
        {
            // Arrange
            long accountId = 10006;
            // Assuming some transactions exist for accountId = 1
          
            // Act
            var result = await _controller.GetTransactions(accountId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            var transactions = okResult.Value as IEnumerable<Transaction>;
            Assert.IsNotNull(transactions);
            Assert.AreEqual(1, transactions.Count());
        }

        [TestMethod]
        public async Task GetTransactions_InvalidAccountId_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.GetTransactions(-1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Invalid account ID.", badRequestResult.Value);
        }

        [TestMethod]
        public async Task GetTransactions_NoTransactionsFound_ReturnsNotFound()
        {
            // Arrange
            long accountId = 6;

            // Act
            var result = await _controller.GetTransactions(accountId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;
            Assert.AreEqual("No transactions found for the given account ID.", notFoundResult.Value);
        }


    }
}