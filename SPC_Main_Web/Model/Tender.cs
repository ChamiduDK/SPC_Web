using System.ComponentModel.DataAnnotations;

namespace SPC_Main_Web.Model
{
    public class Tender
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string SupplierId { get; set; }
        public string Tender_Code { get; set; }
        [Required]
        public string Drug { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int Quantity { get; set; }

        public bool Status { get; set; }
    }
}
