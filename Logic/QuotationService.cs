using QuotationManagementApp.Interfaces;
using QuotationManagementApp.Models;

namespace QuotationManagementApp.Logic
{
    public class QuotationService : IQuotationService
    {
        private readonly IQuotationRepository _repository;

        public QuotationService(IQuotationRepository repository)
        {
            _repository = repository;
        }

        public async Task<int> CreateAsync(Quotation quotation)
        {
            decimal subTotal = 0;

            foreach (var item in quotation.LineItems)
            {
                item.LineTotal = item.Quantity * item.UnitPrice;
                subTotal += item.LineTotal;
            }

            decimal tax = subTotal * 0.18m;
            quotation.TotalAmount = subTotal + tax - quotation.DiscountAmount;

            await _repository.AddAsync(quotation);
            await _repository.SaveChangesAsync();

            return quotation.QuotationId;
        }

        public async Task<List<Quotation>> GetAllAsync()
        {
            var quotations = await _repository.GetAllAsync();
            return quotations.Where(q => !q.IsDeleted).ToList();
        }

        public async Task<Quotation?> GetByIdAsync(int id)
        {
            var quotation = await _repository.GetByIdAsync(id);

            if (quotation == null || quotation.IsDeleted)
                return null;

            return quotation;
        }

        public async Task UpdateStatusAsync(int id, string newStatus)
        {
            var quotation = await _repository.GetByIdAsync(id);
            if (quotation == null) return;

            quotation.Status = newStatus;
            await _repository.SaveChangesAsync();
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var quotation = await _repository.GetByIdAsync(id);
            if (quotation == null) return false;

            quotation.IsDeleted = true;
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}