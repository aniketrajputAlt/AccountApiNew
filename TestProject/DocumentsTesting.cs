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
    public class DocumentsControllerTests
    {
        private DocumentsController _controller;
        private DocumentRepository _repository;
        private DbContextOptions<BankingAppDbContext> _options;

        [TestInitialize]
        public void Setup()
        {
            // Setup in-memory database
            _options = new DbContextOptionsBuilder<BankingAppDbContext>()
                .UseSqlServer("Server=(local)\\MSSQLSERVERNEW;Database=NewBankingApp;Integrated Security=SSPI;TrustServerCertificate=True;")
                .Options;

            var context = new BankingAppDbContext(_options);



            _repository = new DocumentRepository(context);
            _controller = new DocumentsController(_repository);
        }

        [TestMethod]
        public async Task GetCustomerDocuments_ReturnsDocuments_WhenDocumentsExist()
        {
            // Act
            var result = await _controller.GetCustomerDocuments(2) as OkObjectResult;
            var resultValue = result.Value as IEnumerable<object>;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual(1, resultValue.Count());
        }

        [TestMethod]
        public async Task GetCustomerDocuments_ReturnsNotFound_WhenNoDocumentsExist()
        {
            // Act
            var result = await _controller.GetCustomerDocuments(99); // Assuming ID 99 does not exist

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }


    }
}
