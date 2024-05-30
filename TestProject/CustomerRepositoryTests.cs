using AccountApiNew.Controllers;
using AccountApiNew.Model;
using AccountApiNew.Repository;
using Microsoft.AspNetCore.Http;
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


    public class CustomerRepositoryTests
    {
        private CustomersController _controller;
        private BankingAppDbContext _context;
        private CustomerRepository _repository;

        [TestInitialize]
        public void Initialize()
        {
            var options = new DbContextOptionsBuilder<BankingAppDbContext>()
                .UseSqlServer("Server=(local)\\MSSQLSERVERNEW;Database=NewBankingApp;Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=True")
                .Options;
            _context = new BankingAppDbContext(options);
            _repository = new CustomerRepository(_context);
            _controller = new CustomersController(_repository);

        }
     

        [TestMethod]
        public async Task GetActiveCustomer_ReturnsOk_WhenCustomerExists()
        {
            // Act
            var result = await _controller.GetActiveCustomer(1028);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.AreEqual(200, okResult.StatusCode);
            var customerDto = okResult.Value as CustomerDto;
            Assert.IsNotNull(customerDto);
            Assert.AreEqual("New", customerDto.FirstName);
        }

        [TestMethod]
        public async Task GetActiveCustomer_ReturnsNotFound_WhenCustomerDoesNotExist()
        {
            // Act
            var result = await _controller.GetActiveCustomer(18);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [TestMethod]
        public async Task UpdateCustomer_ReturnsBadRequest_WhenModelIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("FirstName", "Required");
            var customerUpdateDTO = new CustomerUpdateDto { FirstName = "" };

            // Act
            var result = await _controller.UpdateCustomer(1, customerUpdateDTO);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task UpdateCustomer_ReturnsOk_WhenUpdateIsSuccessful()
        {
            // Arrange
            var customerUpdateDTO = new CustomerUpdateDto { FirstName = "Updated John", LastName = "Doe" };

            // Act
            var result = await _controller.UpdateCustomer(1, customerUpdateDTO);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.AreEqual(200, okResult?.StatusCode);
            var customerDto = okResult?.Value as CustomerDto;
            Assert.AreEqual("Updated John", customerDto?.FirstName);
        }

        [TestMethod]
        public async Task UpdateCustomer_ReturnsNotFound_WhenCustomerDoesNotExist()
        {
            // Arrange
            var customerUpdateDTO = new CustomerUpdateDto { FirstName = "New Name" };

            // Act
            var result = await _controller.UpdateCustomer(19, customerUpdateDTO);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task CreateCustomer_ReturnsCreated_WhenDataIsValid()
        {
            // Arrange
            var newCustomer = new Customer
            {
                FirstName = "New",
                LastName = "Customer",
                AddressLine1 = "456 New St",
                AddressLine2 = "Apt 4",
                AddressLine3 = "Box 2",
                Pincode = 234566,
                PhoneNumber = "9876543210",
                EmailAddress = "newcustomer@example.com",
                DateOfBirth = new DateTime(1990, 2, 2),
                City = "New City",
                Country = "USA",
                IsActive = true,
                UserId = 1011  // Ensure this UserId exists in your test setup
            };

            // Act
            var result = await _controller.CreateCustomer(newCustomer);

            // Assert
            Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
            var createdResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(201, createdResult.StatusCode);
            var customer = createdResult.Value as Customer;
            Assert.IsNotNull(customer);
            Assert.AreEqual("New", customer.FirstName);
        }

        [TestMethod]
        public async Task CreateCustomer_ReturnsBadRequest_WhenDataIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("FirstName", "Required");
            var customer = new Customer();  // Missing required fields

            // Act
            var result = await _controller.CreateCustomer(customer);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        [TestMethod]
        public async Task CreateCustomer_ThrowsException_WhenUnexpectedErrorOccurs()
        {
            // Arrange
            var customer = new Customer
            {
                FirstName = "Error",
                LastName = "Customer",
                AddressLine1 = "123 Fail St",
                Pincode = 123456,
                PhoneNumber = "1234567890",
                EmailAddress = "error.customer@example.com",
                DateOfBirth = DateTime.Now.AddYears(-20),
                City = "Fail City",
                Country = "Fail Country",
                IsActive = true,
                UserId = 1  // Valid user ID for the context
            };

            // Simulate an exception
            _context.Customers.Add(customer);
            _context.SaveChanges();
            _context.Database.CloseConnection();  // Close the database connection to simulate a failure

            // Act
            var result = await _controller.CreateCustomer(customer);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var objectResult = result as ObjectResult;
            Assert.AreEqual(StatusCodes.Status500InternalServerError, objectResult?.StatusCode);
            Assert.IsTrue(objectResult?.Value?.ToString()?.Contains("An error occurred"));
        }

        [TestMethod]
        public async Task GetActiveCustomerByIdAsync_ReturnsCustomer_WithAllProperties()
        {
            // Act
            var customer = await _repository.GetActiveCustomerByIdAsync(1);

            // Assert
            Assert.IsNotNull(customer);
            Assert.AreEqual("Updated John", customer.FirstName);
            Assert.AreEqual("Doe", customer.LastName);
            Assert.AreEqual("999 Updated St", customer.AddressLine1);
            Assert.AreEqual("Suite 100", customer.AddressLine2);
            Assert.AreEqual("Mailbox 3", customer.AddressLine3);
            Assert.AreEqual(123456, customer.Pincode);
            Assert.AreEqual("1234567890", customer.PhoneNumber);
            Assert.AreEqual("john.doe@example.com", customer.EmailAddress);
            Assert.AreEqual(new DateTime(1980, 1, 1), customer.DateOfBirth);
            Assert.AreEqual("New Metropolis", customer.City);
            Assert.AreEqual("New Freedonia", customer.Country);
            Assert.IsTrue(customer.IsActive);
        }

        [TestMethod]
        public async Task CreateCustomerAsync_EnsuresFullPropertyPersistence()
        {
            // Arrange
            var newCustomer = new Customer
            {
                FirstName = "New",
                LastName = "User",
                AddressLine1 = "456 New St",
                AddressLine2 = "Apt 9",
                AddressLine3 = "Building 12",
                Pincode = 100003,
                PhoneNumber = "1122334455",
                EmailAddress = "new.user@example.com",
                DateOfBirth = new DateTime(1992, 8, 21),
                City = "Rapture",
                Country = "Atlantis",
                IsActive = true,
                UserId = 3  // Assuming UserId 3 is valid for this context
            };

            // Act
            var createdCustomer = await _repository.CreateCustomerAsync(newCustomer);

            // Assert
            Assert.IsNotNull(createdCustomer);
            Assert.AreEqual("New", createdCustomer.FirstName);
            Assert.AreEqual("456 New St", createdCustomer.AddressLine1);
            Assert.AreEqual("Apt 9", createdCustomer.AddressLine2);
            Assert.AreEqual("Building 12", createdCustomer.AddressLine3);
            Assert.AreEqual(100003, createdCustomer.Pincode);
            Assert.AreEqual("1122334455", createdCustomer.PhoneNumber);
            Assert.AreEqual("new.user@example.com", createdCustomer.EmailAddress);
            Assert.AreEqual(new DateTime(1992, 8, 21), createdCustomer.DateOfBirth);
            Assert.AreEqual("Rapture", createdCustomer.City); Assert.AreEqual("Atlantis", createdCustomer.Country);
            Assert.IsTrue(createdCustomer.IsActive); Assert.AreEqual(newCustomer.UserId, createdCustomer.UserId);
            // Verify that UserId is correctly associated
        }
        [TestMethod]
        public async Task UpdateCustomerAsync_UpdatesAndReturnsTrue_WhenCustomerExistsWithFullDetails()
        {
            // Arrange
            var customerToUpdate = await _context.Customers.FindAsync(1);
            if (customerToUpdate == null)
            {
                Assert.Fail("Customer with ID 1 does not exist.");
                return;
            }
            customerToUpdate.FirstName = "Updated";
            customerToUpdate.AddressLine1 = "999 Updated St";
            customerToUpdate.City = "New Metropolis";
            customerToUpdate.Country = "New Freedonia";

            // Act
            bool result = await _repository.UpdateCustomerAsync(customerToUpdate);

            // Assert
            Assert.IsTrue(result);
            var updatedCustomer = await _repository.GetActiveCustomerByIdAsync(1);
            Assert.AreEqual("Updated", updatedCustomer.FirstName);
            Assert.AreEqual("999 Updated St", updatedCustomer.AddressLine1);
            Assert.AreEqual("New Metropolis", updatedCustomer.City);
            Assert.AreEqual("New Freedonia", updatedCustomer.Country);
        }

        [TestMethod]
        public async Task UpdateCustomerAsync_ReturnsFalse_WhenCustomerDoesNotExist()
        {
            // Arrange
            var customerToUpdate = new Customer { CustomerId = 99, FirstName = "Nonexistent", IsActive = true, UserId = 4 };

            // Act
            bool result = await _repository.UpdateCustomerAsync(customerToUpdate);

            // Assert
            Assert.IsFalse(result);
        }

       


    }
}
