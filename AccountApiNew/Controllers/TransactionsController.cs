using AccountApiNew.Model;
using AccountApiNew.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Transactions;

namespace AccountApiNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionRepository _transactionRepository;
        public TransactionsController(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> TransferFunds(FundTransferModel transferModel)
        {
            try
            {
                if (transferModel.sourceAccountId <= 0 || transferModel.destinationAccountId <= 0 || transferModel.amount <= 0)
                {
                    return BadRequest("Invalid input parameters.");
                }

                var result = await _transactionRepository.FundTransfer(transferModel.sourceAccountId, transferModel.destinationAccountId, transferModel.amount);

                if (result)
                {
                    return Ok("Fund transfer successful.");
                }
                else
                {
                    return StatusCode(400, "Fund transfer failed. ");
                }
            }
            catch(Exception ex)
            {

                return StatusCode(500, "Fund transfer failed. "+ ex.Message);
            }
        
        }
        [HttpGet("transactions/{accountId}")]
        public async Task<IActionResult> GetTransactions(long accountId)
        {
            if (accountId <= 0)
            {
                return BadRequest("Invalid account ID.");
            }

            var  transactions = await _transactionRepository.TransactionsList(accountId);
            if (transactions == null || !transactions.Any())
            {
                return NotFound("No transactions found for the given account ID.");
            }
            return Ok(transactions);
        }




    }
}
