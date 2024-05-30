using AccountApiNew.Model;

namespace AccountApiNew.Repository
{
    public interface ITransactionRepository
    {
       

        public Task<bool>FundTransfer(long sourceAccountId, long destinationAccountId, decimal amount);

       
        public Task<IEnumerable<Transaction>>TransactionsList(long accountId);
    }
}
