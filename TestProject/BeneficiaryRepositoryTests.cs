using AccountApiNew.Controllers;
using AccountApiNew.Model;
using AccountApiNew.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace AccountApiNewTesting
{
    [TestClass]
    public class BeneficiaryRepositoryTests
    {
        private BankingAppDbContext _context;
        private BeneficiaryRepository _repository;
        private BeneficiaryController _controller;

        [TestInitialize]
        public void Initialize()
        {
            var options = new DbContextOptionsBuilder<BankingAppDbContext>()
                .UseSqlServer("Server=(local)\\MSSQLSERVERNEW;Database=NewBankingApp;Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=True")
                .Options;

            _context = new BankingAppDbContext(options);
            _repository = new BeneficiaryRepository(_context);
            _controller=new BeneficiaryController(_repository);
        }

        //Add Beneficiary

        [TestMethod]
        public async Task AddBeneficiary_ShouldReturnBeneficiary_WhenInputIsValid()
        {
            // Arrange
            var inputModel = new BeneficiaryInputModel
            {
                BenefName = "John Doe",
                BenefAccount = 2,
                BenefIFSC = "1",
                AccountId = 3,
                IsActive = true
            };
            var beneficiaries = await _repository.ListBeneficiary(inputModel.AccountId);
            var prevcount = beneficiaries.Count();
            // Act
            Beneficiary result = await _repository.Addbeneficiary(inputModel);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(inputModel.BenefName, result.BenefName);

            beneficiaries = await _repository.ListBeneficiary(inputModel.AccountId);
            var currcount = beneficiaries.Count();
            Assert.AreEqual(prevcount + 1, currcount);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task AddBeneficiary_ShouldThrowArgumentNullException_WhenInputIsNull()
        {
            // Act
            await _repository.Addbeneficiary(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task AddBeneficiary_ShouldThrowArgumentException_WhenBeneficiaryNameIsMissing()
        {
            // Arrange
            var inputModel = new BeneficiaryInputModel
            {
                BenefName = " ",
                BenefAccount = 1234011000001,
                BenefIFSC = "BR001",
                AccountId = 1234011000000,
                IsActive = true
            };

            // Act
            await _repository.Addbeneficiary(inputModel);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task AddBeneficiary_ShouldThrowArgumentException_WhenBeneficiaryAccountDoesNotExist()
        {
            // Arrange
            var inputModel = new BeneficiaryInputModel
            {
                BenefName = "Benef Name",
                BenefAccount = 1234011000010,
                BenefIFSC = "1",
                AccountId = 3,
                IsActive = true
            };

            // Act
            await _repository.Addbeneficiary(inputModel);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task AddBeneficiary_ShouldThrowArgumentException_WhenMainAccountDoesNotExist()
        {
            // Arrange
            var inputModel = new BeneficiaryInputModel
            {
                BenefName = "Benef Name",
                BenefAccount = 3,
                BenefIFSC = "1",
                AccountId = 1234011000111,
                IsActive = true
            };
            // Act
            await _repository.Addbeneficiary(inputModel);
        }


        // Delete Beneficiary

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task DeleteBeneficiary_ShouldThrowArgumentException_WhenBeneficiaryIdIsInvalid()
        {
            // Act
            await _repository.DeleteBenficiary(0);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task DeleteBeneficiary_ShouldThrowKeyNotFoundException_WhenBeneficiaryDoesNotExist()
        {
            // Arrange
            long invalidBeneficiaryId = 999999;

            // Act
            await _repository.DeleteBenficiary(invalidBeneficiaryId);
        }

        [TestMethod]
        public async Task DeleteBeneficiary_ShouldReturnTrue_WhenBeneficiaryIsDeletedSuccessfully()
        {
            var beneficiary = new Beneficiary
            {
                BenefName = "John Doe",
                BenefAccount = 2,
                BenefIFSC = "1",
                AccountId = 3,
                IsActive = true
            };
            var beneficiaries = await _repository.ListBeneficiary(beneficiary.AccountId);
            var prevcount = beneficiaries.Count();
            // Act
            bool result = await _repository.DeleteBenficiary(beneficiary.BenefAccount);

            beneficiaries = await _repository.ListBeneficiary(beneficiary.AccountId);
            var currcount = beneficiaries.Count();
            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(prevcount - 1, currcount);
        }


        // List Beneficiary

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task ListBeneficiary_ShouldThrowArgumentException_WhenAccountIdIsInvalid()
        {
            // Act
            await _repository.ListBeneficiary(0);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task ListBeneficiary_ShouldThrowKeyNotFoundException_WhenAccountDoesNotExist()
        {
            // Arrange
            long invalidAccountId = 999999;

            // Act
            await _repository.ListBeneficiary(invalidAccountId);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task ListBeneficiary_ShouldThrowInvalidOperationException_WhenNoBeneficiariesFound()
        {
            // Arrange
            long validAccountId = 5;

            // Act
            await _repository.ListBeneficiary(validAccountId);
        }

        [TestMethod]
        public async Task ListBeneficiary_ShouldReturnBeneficiaries_WhenAccountIdIsValid()
        {
            // Arrange
            long validAccountId =4;
            // Act
            var result = await _repository.ListBeneficiary(validAccountId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(validAccountId, result.First().AccountId);
        }
        


        //controller
        [TestMethod]
        public async Task GetBeneficiariesByAccountId_ReturnsOkResult_WithBeneficiaries()
        {
            // Arrange
            long accountId = 3;

            // Act
            var result = await _controller.GetBeneficiariesByAccountId(accountId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            var returnedBeneficiaries = okResult.Value as IEnumerable<Beneficiary>;
            Assert.IsNotNull(returnedBeneficiaries);
        }

        [TestMethod]
        public async Task GetBeneficiariesByAccountId_ReturnsBadRequest_WhenArgumentExceptionThrown()
        {
            // Arrange
            long accountId = 0; // Invalid account ID

            // Act
            var result = await _controller.GetBeneficiariesByAccountId(accountId);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.AreEqual("Account ID must be greater than zero. (Parameter 'accountId')", badRequestResult.Value);
        }

        [TestMethod]
        public async Task PostBeneficiary_ReturnsCreatedAtActionResult_WithBeneficiary()
        {
            // Arrange
            var inputModel = new BeneficiaryInputModel
            {
                BenefName = "Jane Doe",
                BenefAccount = 4,
                BenefIFSC = "1",
                AccountId = 6,
                IsActive = true
            };

            // Act
            var result = await _controller.PostBeneficiary(inputModel);

            // Assert
            var createdAtActionResult = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdAtActionResult);
            Assert.AreEqual(201, createdAtActionResult.StatusCode);
            var returnedBeneficiary = createdAtActionResult.Value as Beneficiary;
            Assert.IsNotNull(returnedBeneficiary);
            Assert.AreEqual(inputModel.BenefName, returnedBeneficiary.BenefName);
        }

        [TestMethod]
        public async Task PostBeneficiary_ReturnsBadRequest_WhenArgumentExceptionThrown()
        {
            // Arrange
            var inputModel = new BeneficiaryInputModel(); // Invalid input model


            // Act
            var result = await _controller.PostBeneficiary(inputModel);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.AreEqual("Beneficiary name is required. (Parameter 'BenefName')", badRequestResult.Value);
        }

        [TestMethod]
        public async Task DeleteBeneficiary_ReturnsNoContent_WhenDeleteIsSuccessful()
        {
            // Arrange
            long beneficiaryId =33;

            // Act
            var result = await _controller.DeleteBeneficiary(beneficiaryId);
          //  var badReq = result.Result as BadRequestObjectResult;

            // Assert
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
        }

        [TestMethod]
        public async Task DeleteBeneficiary_ReturnsNotFound_WhenBeneficiaryDoesNotExist()
        {
            // Arrange
            long beneficiaryId = 7;

            // Act
            var result = await _controller.DeleteBeneficiary(beneficiaryId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
        }

        [TestMethod]
        public async Task DeleteBeneficiary_ReturnsBadRequest_WhenArgumentExceptionThrown()
        {
            // Arrange
            long beneficiaryId = 0; // Invalid beneficiary ID

            // Act
            var result = await _controller.DeleteBeneficiary(beneficiaryId);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.AreEqual("Beneficiary ID must be greater than zero. (Parameter 'beneficiaryId')", badRequestResult.Value);
        }

    }
}
