using AccountApiNew.Model;

namespace AccountApiNew.Repository
{
    public interface IAccountRepository
    {
        public Task<bool> CreateAccount(AccountInputModel input);
        public Task<Account> GetAccountById(long id);
        public Task<bool> DeleteAccount(long id);



        public Task<List<Account>> GetAccountsByCustomerId(int customerId);
    }
}
