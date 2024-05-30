using AccountApiNew.Model;
using AccountApiNew.Repository;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountApiNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomersController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [HttpGet("GetActiveCustomer/{id}")]
        public async Task<IActionResult> GetActiveCustomer(int id)
        {
          
                var customer = await _customerRepository.GetActiveCustomerByIdAsync(id);
                if (customer == null)
                {
                    return NotFound(new { message = "Customer not found." });
                }

                var customerDto = new CustomerDto
                {
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    AddressLine1 = customer.AddressLine1,
                    AddressLine2 = customer.AddressLine2,
                    AddressLine3 = customer.AddressLine3,
                    Pincode = customer.Pincode.ToString(),
                    PhoneNumber = customer.PhoneNumber,
                    EmailAddress = customer.EmailAddress,
                    DateOfBirth = customer.DateOfBirth,
                    City = customer.City,
                    Country = customer.Country
                };

                return Ok(customerDto);
         
        }



        [HttpPut("updateCustomer/{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] CustomerUpdateDto customerUpdateDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customer = await _customerRepository.GetActiveCustomerByIdAsync(id);
            if (customer == null)
            {
                return NotFound(new { message = "Customer not found." });
            }

            // Map DTO to the existing customer entity
            customer.FirstName = !string.IsNullOrWhiteSpace(customerUpdateDTO.FirstName) ? customerUpdateDTO.FirstName : customer.FirstName;
            customer.LastName = !string.IsNullOrWhiteSpace(customerUpdateDTO.LastName) ? customerUpdateDTO.LastName : customer.LastName;
            customer.AddressLine1 = !string.IsNullOrWhiteSpace(customerUpdateDTO.AddressLine1) ? customerUpdateDTO.AddressLine1 : customer.AddressLine1;
            customer.AddressLine2 = !string.IsNullOrWhiteSpace(customerUpdateDTO.AddressLine2) ? customerUpdateDTO.AddressLine2 : customer.AddressLine2;
            customer.AddressLine3 = !string.IsNullOrWhiteSpace(customerUpdateDTO.AddressLine3) ? customerUpdateDTO.AddressLine3 : customer.AddressLine3;
            customer.Pincode = customerUpdateDTO.Pincode.HasValue ? customerUpdateDTO.Pincode.Value : customer.Pincode;
            customer.PhoneNumber = !string.IsNullOrWhiteSpace(customerUpdateDTO.PhoneNumber) ? customerUpdateDTO.PhoneNumber : customer.PhoneNumber;
            customer.EmailAddress = !string.IsNullOrWhiteSpace(customerUpdateDTO.EmailAddress) ? customerUpdateDTO.EmailAddress : customer.EmailAddress;
            customer.DateOfBirth = customerUpdateDTO.DateOfBirth != default ? customerUpdateDTO.DateOfBirth : customer.DateOfBirth;
            customer.City = !string.IsNullOrWhiteSpace(customerUpdateDTO.City) ? customerUpdateDTO.City : customer.City;
            customer.Country = !string.IsNullOrWhiteSpace(customerUpdateDTO.Country) ? customerUpdateDTO.Country : customer.Country;

            bool updated = await _customerRepository.UpdateCustomerAsync(customer);
            if (!updated)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error updating customer" });
            }

            var customerDto = new CustomerDto
            {
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                AddressLine1 = customer.AddressLine1,
                AddressLine2 = customer.AddressLine2,
                AddressLine3 = customer.AddressLine3,
                Pincode = customer.Pincode.ToString(),
                PhoneNumber = customer.PhoneNumber,
                EmailAddress = customer.EmailAddress,
                DateOfBirth = customer.DateOfBirth,
                City = customer.City,
                Country = customer.Country
            };

            return Ok(customerDto);
        }




        [HttpPost("CreateCustomer")]
        public async Task<IActionResult> CreateCustomer([FromBody] Customer customer)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);  // Returns all validation errors

            try
            {
                var createdCustomer = await _customerRepository.CreateCustomerAsync(customer);
                return CreatedAtAction(nameof(GetActiveCustomer), new { id = createdCustomer.CustomerId }, createdCustomer);
            }
            catch (Exception ex)
            {
                // Log the exception here
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while creating the customer", error = ex.Message });
            }
        }
    }
}
