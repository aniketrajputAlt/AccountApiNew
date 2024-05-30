using AccountApiNew.Model;

namespace AccountApiNew.Repository
{
    public interface IDocumentRepository
    {
       //public Task InsertDocumentAsync(int customerId, byte[] document, int docType);

       public Task<IEnumerable<Document>> GetDocumentsByCustomerIdAsync(int customerId);
      
    }
}
