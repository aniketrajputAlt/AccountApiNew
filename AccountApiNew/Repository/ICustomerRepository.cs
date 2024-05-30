using AccountApiNew.Model;

namespace AccountApiNew.Repository
{
    public interface ICustomerRepository
    {
        public Task<Customer> GetActiveCustomerByIdAsync(int customerId);
     //   public Task<IEnumerable<Customer>> GetActiveCustomersAsync();
     //  public  Task<bool> DeactivateCustomerAsync(int customerId);
       public  Task<bool> UpdateCustomerAsync(Customer customer);
       public Task<Customer> CreateCustomerAsync(Customer customer);
    }
}
