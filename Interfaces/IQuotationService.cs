using QuotationManagementApp.Models;

namespace QuotationManagementApp.Interfaces
{
    public interface IQuotationService
    {
        Task<int> CreateAsync(Quotation quotation);
        Task<List<Quotation>> GetAllAsync();
        Task<Quotation?> GetByIdAsync(int id);
        Task UpdateStatusAsync(int id, string newStatus);
        Task<bool> SoftDeleteAsync(int id);
    }
}