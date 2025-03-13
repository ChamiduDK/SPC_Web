using System.ComponentModel.DataAnnotations;

namespace SPC_Main_Web.Model
{
    public class TenderAd
    {
        public string Tender_Code { get; set; }
        public string Drug_Name { get; set; }
        public string Description { get; set; }
        public string Quantity { get; set; }
        public DateTime Due_Date { get; set; }
        public bool Status { get; set; }
    }
}
