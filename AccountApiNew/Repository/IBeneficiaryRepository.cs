using AccountApiNew.Model;

namespace AccountApiNew.Repository
{
    public interface IBeneficiaryRepository
    {
       public  Task<Beneficiary> Addbeneficiary(BeneficiaryInputModel beneficiaryInput);

       public Task<IEnumerable<Beneficiary>> ListBeneficiary(long accountId);

       public Task<bool>DeleteBenficiary(long beneficiaryId);
    }
}
