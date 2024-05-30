using AccountApiNew.Model;
using Microsoft.EntityFrameworkCore;

namespace AccountApiNew.Repository
{
    public class DocumentRepository:IDocumentRepository
    {
        private readonly BankingAppDbContext _context;

        public DocumentRepository(BankingAppDbContext context)
        {
            _context = context;
        }

       /* public async Task InsertDocumentAsync(int customerId, byte[] document, int docType)
        {
            var newDocument = new Document
            {
                CustomerId = customerId,
                DocTypeId = docType,
                Documents = document,
                IsActive = true
            };
            _context.Documents.Add(newDocument);
            await _context.SaveChangesAsync();
        }*/

        public async Task<IEnumerable<Document>> GetDocumentsByCustomerIdAsync(int customerId)
        {

            return await _context.Documents
                .Where(d => d.CustomerId == customerId && d.IsActive)
                .ToListAsync();

        }





       
    }
}
