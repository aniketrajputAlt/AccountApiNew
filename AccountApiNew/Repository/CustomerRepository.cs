using AccountApiNew.Model;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace AccountApiNew.Repository
{

    public class CustomerRepository : ICustomerRepository
    {
        private readonly BankingAppDbContext _context;


        public CustomerRepository(BankingAppDbContext context)
        {
            _context = context;
        }

        public async Task<Customer> GetActiveCustomerByIdAsync(int customerId)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == customerId && c.IsActive);
        }


        public async Task<Customer> CreateCustomerAsync(Customer customer)
        {
            
            ArgumentNullException.ThrowIfNull(customer);

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer;
        }


        public async Task<bool> UpdateCustomerAsync(Customer customer)
        {
            var existingCustomer = await _context.Customers.FindAsync(customer.CustomerId);
            if (existingCustomer == null || !existingCustomer.IsActive)
            {
                return false;
            }

            _context.Entry(existingCustomer).CurrentValues.SetValues(customer);
            await _context.SaveChangesAsync();
            return true;
        }



    }
}
