using Microsoft.EntityFrameworkCore;
using QuotationManagementApp.Data;
using QuotationManagementApp.Interfaces;
using QuotationManagementApp.Models;

namespace QuotationManagementApp.Logic
{
    public class QuotationRepository : IQuotationRepository
    {
        private readonly CRMDbContext _context;

        public QuotationRepository(CRMDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Quotation quotation)
        {
            await _context.Quotations.AddAsync(quotation);
        }
 
 public async Task AddWithTransactionAsync(Quotation quotation)
{
    using var transaction = await _context.Database.BeginTransactionAsync();
    try
    {
        await _context.Quotations.AddAsync(quotation);
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
    }
    catch
    {
        await transaction.RollbackAsync();
        throw;
    }
}
        public async Task<List<Quotation>> GetAllAsync()
        {
            return await _context.Quotations
                .Include(q => q.LineItems)
                .ToListAsync();
        }

        public async Task<Quotation?> GetByIdAsync(int id)
        {
            return await _context.Quotations
                .Include(q => q.LineItems)
                .FirstOrDefaultAsync(q => q.QuotationId == id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}