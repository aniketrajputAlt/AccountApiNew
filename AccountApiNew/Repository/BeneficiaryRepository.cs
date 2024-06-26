﻿using AccountApiNew.Model;
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
                var bExists=await _context.Beneficiaries.FirstOrDefaultAsync(c=>c.AccountId==beneficiaryInput.AccountId && c.BenefAccount==beneficiaryInput.BenefAccount && c.IsActive);

                if(bExists is not null)
                {
                    throw new ArgumentException("Beneficiary already exist for this account.");
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

                var benefIfsc = await _context.Branches.AnyAsync(a => a.BranchID == beneficiaryInput.BenefIFSC);
                if (!benefIfsc)
                {
                    throw new ArgumentException("Beneficiary Ifsc does not exist in Branches.");
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

        public async Task<bool> DeleteBenficiary(int beneficiaryId)
        {
            try
            {
                if (beneficiaryId <= 0)
                {
                    throw new ArgumentException("Beneficiary ID must be greater than zero.", nameof(beneficiaryId));
                }
                var beneficiary = await _context.Beneficiaries.FirstOrDefaultAsync(b => b.BenefID == beneficiaryId && b.IsActive);
                if (beneficiary == null)
                {
                    throw new ArgumentException("Beneficiary not found or is already inactive.");
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
                    throw new ArgumentException("Account does not exist.");
                }

            
                var beneficiaries = await _context.Beneficiaries
                    .Where(b => b.AccountId == accountId && b.IsActive)
                    .ToListAsync();

                
                if (!beneficiaries.Any())
                {
                    throw new ArgumentException("No beneficiaries found for the given account ID.");
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
