using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuotationManagementApp.Models
{
    public class QuotationLineItem
    {
        [Key]
        public int LineItemId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal LineTotal { get; set; }

        [ForeignKey("Quotation")]
        public int QuotationId { get; set; }

        public Quotation? Quotation { get; set; }
    }
}