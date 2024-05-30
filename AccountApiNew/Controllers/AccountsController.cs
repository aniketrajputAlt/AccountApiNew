using AccountApiNew.Model;
using AccountApiNew.Repository;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Contracts;

namespace AccountApiNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;
        public AccountsController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        [HttpGet("Customer/{id}")]
        public async Task<IActionResult> GetAccountsByCustomerId(int id)
        {
            try
            {
                var account = await _accountRepository.GetAccountsByCustomerId(id);
                if (account is null || account.Count == 0)
                {
                    return NotFound(new { message = "Accounts not found." });
                }
                return Ok(account);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error retrieving account data corresponding to the customer Id", error = ex.Message });
            }
        }
        // POST: api/Accounts
        [HttpPost("CreateAccount")]
        public async Task<IActionResult> CreateAccount([FromBody] AccountInputModel input)
        {
            if (input == null)
            {
                return BadRequest("Invalid parameters");
            }
               var result = await _accountRepository.CreateAccount(input);
                if (result)
                {
                    return Ok("Account created successfully.");
                }
                else
                {
                    return StatusCode(500, "Failed to create account.");
                }
           
        }


        // GET: api/Accounts/{id}
        [HttpGet("Detail/{id}")]
        public async Task<ActionResult<object>> GetAccountById(long id)
        {
            try
            {
                var account = await _accountRepository.GetAccountById(id);


                var simplifiedAccount = new
                {
                    account.AccountId,
                    account.Balance,
                    account.wd_Quota,
                    account.dp_Quota,
                    account.isActive,
                    CustomerID = account.CustomerID,
                    TypeID = account.TypeID,
                    BranchID = account.BranchID
                };

                return simplifiedAccount;
            }
            catch (Exception ex)
            {
                // If any other exception occurs, return a 500 Internal Server Error response with the exception message
                return BadRequest(ex.Message);
            }
        }


        // DELETE: api/Accounts/{id}
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteAccount(long id)
        {
            try
            {
                var result = await _accountRepository.DeleteAccount(id);
                if (result)
                {
                    return Ok("Account deleted successfully.");
                }
                else
                {
                    // If DeleteAccount throws an exception, it means the account with the provided ID was not found
                    return StatusCode(500, "Failed to delete account.");
                }
            }
            catch (Exception ex)
            {
                // If any other exception occurs, return 500 Internal Server Error
                return BadRequest(ex.Message);
            }
        }
    }
}
