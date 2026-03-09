using System.ComponentModel.DataAnnotations;

namespace QuotationManagementApp.Models
{
    public class Quotation
    {
        [Key]
        public int QuotationId { get; set; }

        public string QuoteNumber { get; set; } = string.Empty;

        public DateTime QuoteDate { get; set; }

        public DateTime ExpiryDate { get; set; }

        public QuoteStatus Status { get; set; } = QuoteStatus.Draft;

        public decimal DiscountAmount { get; set; }

        public decimal TotalAmount { get; set; }



        public bool IsDeleted { get; set; } = false;

        public int Version { get; set; } 
        public int ParentQuotationId { get; set; }

        public int? LeadId { get; set; }
        public int? CustomerId { get; set; }

        public List<QuotationLineItem> LineItems { get; set; }
            = new List<QuotationLineItem>();
    }
}   