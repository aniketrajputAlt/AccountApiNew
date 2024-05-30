using AccountApiNew.Model;
using Microsoft.EntityFrameworkCore;

namespace AccountApiNew.Repository
{
    
        public class BeneficiaryRepository : IBeneficiaryRepository
        {
            private readonly BankingAppDbContext _context;

            public BeneficiaryRepository(BankingAppDbContext context)
            {
                _context = context;
            }


        public async Task<Beneficiary> Addbeneficiary(BeneficiaryInputModel beneficiaryInput)
        {
            try
            {
                if (beneficiaryInput == null)
                {
                    throw new ArgumentNullException(nameof(beneficiaryInput), "Beneficiary input model cannot be null.");
                }
                if (string.IsNullOrWhiteSpace(beneficiaryInput.BenefName))
                {
                    throw new ArgumentException("Beneficiary name is required.", nameof(beneficiaryInput.BenefName));
                }
             
                var accountExists = await _context.Accounts.AnyAsync(a => a.AccountId == beneficiaryInput.BenefAccount);
                if (!accountExists)
                {
                    throw new ArgumentException("Beneficiary Account does not exist in Accounts.", nameof(beneficiaryInput.BenefAccount));
                }

       
                var mainAccountExists = await _context.Accounts.AnyAsync(a => a.AccountId == beneficiaryInput.AccountId);
                if (!mainAccountExists)
                {
                    throw new ArgumentException("Main account does not exist in Accounts.", nameof(beneficiaryInput.AccountId));
                }



                var beneficiary = new Beneficiary
                {
                    BenefName = beneficiaryInput.BenefName,
                    BenefAccount = beneficiaryInput.BenefAccount,
                    BenefIFSC = beneficiaryInput.BenefIFSC,
                    AccountId = beneficiaryInput.AccountId,
                    IsActive = beneficiaryInput.IsActive
                };
                await _context.Beneficiaries.AddAsync(beneficiary);
                await _context.SaveChangesAsync();
                return beneficiary;
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(ex.Message);
            }
         
        }

        public async Task<bool> DeleteBenficiary(long beneficiaryId)
        {
            try
            {
                if (beneficiaryId <= 0)
                {
                    throw new ArgumentException("Beneficiary ID must be greater than zero.", nameof(beneficiaryId));
                }
                var beneficiary = await _context.Beneficiaries.FirstOrDefaultAsync(b => b.BenefAccount == beneficiaryId && b.IsActive == true);
                if (beneficiary == null)
                {
                    throw new KeyNotFoundException("Beneficiary not found or is already inactive.");
                }

                beneficiary.IsActive = false;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                throw new KeyNotFoundException(ex.Message);
            }
        
          
        }

        public async Task<IEnumerable<Beneficiary>> ListBeneficiary(long accountId)
        {
            try
            {
               
                if (accountId <= 0)
                {
                    throw new ArgumentException("Account ID must be greater than zero.", nameof(accountId));
                }

            
                var accountExists = await _context.Accounts.AnyAsync(a => a.AccountId == accountId);
                if (!accountExists)
                {
                    throw new KeyNotFoundException("Account does not exist.");
                }

            
                var beneficiaries = await _context.Beneficiaries
                    .Where(b => b.AccountId == accountId && b.IsActive)
                    .ToListAsync();

                
                if (!beneficiaries.Any())
                {
                    throw new InvalidOperationException("No beneficiaries found for the given account ID.");
                }


                return beneficiaries;
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                throw new KeyNotFoundException(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
           
        }


    }
}
