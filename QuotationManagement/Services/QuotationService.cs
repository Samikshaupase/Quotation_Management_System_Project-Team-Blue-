using Microsoft.Extensions.Configuration;
using QuotationManagementApp.Interfaces;
using QuotationManagementApp.Models;
using QuotationManagementApp.Services;

namespace QuotationManagementApp.Logic
{
    public class QuotationService : IQuotationService
    {
        private readonly IQuotationRepository _repository;
        private readonly decimal _taxRate;

        public QuotationService(IQuotationRepository repository, IConfiguration config)
        {
            _repository = repository;
            _taxRate = config.GetValue<decimal>("TaxSettings:DefaultTaxRate");
        }
        public async Task<int> CreateAsync(Quotation quotation)
        {
            if (quotation.ExpiryDate <= DateTime.UtcNow)
                throw new ArgumentException("Expiry date must be in the future.");

            quotation.QuoteDate = DateTime.UtcNow;

            decimal subTotal = 0;

            foreach (var item in quotation.LineItems)
            {
                item.LineTotal = item.Quantity * item.UnitPrice;
                subTotal += item.LineTotal;
            }

            decimal tax = subTotal * _taxRate; // use config rate
            quotation.TotalAmount = subTotal + tax - quotation.DiscountAmount;
            quotation.QuoteNumber = await GenerateQuoteNumber();

            await _repository.AddWithTransactionAsync(quotation);

            return quotation.QuotationId;
        }
        public async Task<int> CreateRevisionAsync(int id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                throw new Exception("Quotation not found");

            var revision = new Quotation
            {
                QuoteNumber = existing.QuoteNumber,
                Version = existing.Version + 1,
                ParentQuotationId = existing.QuotationId,
                ExpiryDate = existing.ExpiryDate,
                DiscountAmount = existing.DiscountAmount,
                LineItems = existing.LineItems
                    .Select(li => new QuotationLineItem
                    {
                        ProductName = li.ProductName,
                        Quantity = li.Quantity,
                        UnitPrice = li.UnitPrice
                    }).ToList()
            };

            await _repository.AddWithTransactionAsync(revision);
            return revision.QuotationId;
        }
        public async Task<List<Quotation>> GetAllAsync()
        {
            var quotations = await _repository.GetAllAsync();
            return quotations.Where(q => !q.IsDeleted).ToList();
        }


        private async Task<string> GenerateQuoteNumber()
        {
            var quotations = await _repository.GetAllAsync();

            int nextNumber = quotations.Any()
                ? quotations.Max(q => int.Parse(q.QuoteNumber.Split('-').Last())) + 1
                : 1;

            return $"Q-{DateTime.Now.Year}-{nextNumber:D4}";
        }
        public async Task<Quotation?> GetByIdAsync(int id)
        {
            var quotation = await _repository.GetByIdAsync(id);

            if (quotation == null)
                return null;

            if (quotation.ExpiryDate < DateTime.UtcNow &&
                quotation.Status != QuoteStatus.Accepted &&
                quotation.Status != QuoteStatus.Rejected &&
                quotation.Status != QuoteStatus.Expired)
            {
                quotation.Status = QuoteStatus.Expired;
                await _repository.SaveChangesAsync();
            }

            return quotation;
        }



        public async Task UpdateStatusAsync(int id, QuoteStatus newStatus)
        {
            var quotation = await _repository.GetByIdAsync(id);

            if (quotation == null)
                throw new ArgumentException("Quotation not found.");

            if (!QuoteStatusValidator.CanTransition(quotation.Status, newStatus))
                throw new InvalidOperationException("Invalid status transition.");

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

        public Task UpdateStatusAsync(int id, string newStatus)
        {
            throw new NotImplementedException();
        }
    }
}