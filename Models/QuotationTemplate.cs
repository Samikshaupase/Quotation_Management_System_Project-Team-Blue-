using System;
using System.ComponentModel.DataAnnotations;


namespace QuotationManagement.Models
{
    public class QuotationTemplate
    {
        [Key]
        public int TemplateId { get; set; }

        public string TemplateName { get; set; }

        public string TemplateContent { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}