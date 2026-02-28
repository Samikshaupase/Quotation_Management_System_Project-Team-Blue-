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

        public string Status { get; set; } = "Draft";

        public decimal DiscountAmount { get; set; }

        public decimal TotalAmount { get; set; }

        

        public bool IsDeleted { get; set; } = false;

        public List<QuotationLineItem> LineItems { get; set; }
            = new List<QuotationLineItem>();
    }
}