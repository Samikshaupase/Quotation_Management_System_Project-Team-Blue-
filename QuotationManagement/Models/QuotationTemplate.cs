using System;
using System.ComponentModel.DataAnnotations;


namespace QuotationManagement.Models
{
    public class QuotationTemplate
    {
        [Key]
        public int TemplateId { get; set; }

        public string TemplateName { get; set; }=string.Empty;

        public string TemplateContent { get; set; }=string.Empty;

        public DateTime CreatedDate { get; set; }
    }
}