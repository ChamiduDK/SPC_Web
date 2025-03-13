using System.ComponentModel.DataAnnotations;

namespace SPC_Main_Web.Model
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public string PharmacyId { get; set; }  
        public int DrugId { get; set; } 
        public int Quantity { get; set; }  
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; } = "Pending";
        public virtual Drug Drug { get; set; }

    }
}
