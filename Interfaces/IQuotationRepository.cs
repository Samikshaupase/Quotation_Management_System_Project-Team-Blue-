using QuotationManagementApp.Models;

namespace QuotationManagementApp.Interfaces
{
    public interface IQuotationRepository
    {
        Task AddAsync(Quotation quotation);
        Task<List<Quotation>> GetAllAsync();
        Task<Quotation?> GetByIdAsync(int id);
        Task SaveChangesAsync();
    }
}