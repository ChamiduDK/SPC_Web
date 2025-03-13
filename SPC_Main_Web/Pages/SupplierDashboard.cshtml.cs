using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using SPC_Main_Web.Model;

namespace SPC_Main_Web.Pages
{
    public class SupplierDashboardModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public List<TenderAd> TenderAd { get; set; } = new List<TenderAd>();
        public List<Tender> TenderData { get; set; } = new List<Tender>();

        [BindProperty]
        public Tender tender { get; set; }

        public SupplierDashboardModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Check if the user is a Supplier from Session
            var userType = HttpContext.Session.GetString("UserType");
            if (userType != "Supplier")
            {
                return RedirectToPage("/Login");
            }

            // Fetch TenderAd Data
            string tenderAdUrl = "https://localhost:7226/api/SpcTenderAd";
            HttpResponseMessage tenderAdResponse = await _httpClient.GetAsync(tenderAdUrl);
            if (tenderAdResponse.IsSuccessStatusCode)
            {
                TenderAd = await tenderAdResponse.Content.ReadFromJsonAsync<List<TenderAd>>() ?? new List<TenderAd>();
            }

            // Fetch Tender Data
            string tenderUrl = "https://localhost:7226/api/Tender";
            HttpResponseMessage tenderResponse = await _httpClient.GetAsync(tenderUrl);
            if (tenderResponse.IsSuccessStatusCode)
            {
                var allTenderData = await tenderResponse.Content.ReadFromJsonAsync<List<Tender>>() ?? new List<Tender>();

                // Retrieve the SupplierId from session
                var supplierId = HttpContext.Session.GetString("SupplierId");
                if (string.IsNullOrEmpty(supplierId))
                {
                    TempData["failure"] = "Supplier Id is missing in session.";
                    return RedirectToPage("/Login");
                }

                // Filter TenderData based on the SupplierId
                TenderData = allTenderData.Where(t => t.SupplierId == supplierId).ToList();
            }

            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            // Check if the user is a Supplier from Session
            var userType = HttpContext.Session.GetString("UserType");
            if (userType != "Supplier")
            {
                return RedirectToPage("/Login");
            }

            if (tender == null)
            {
                TempData["failure"] = "Tender data is missing";
                return RedirectToPage("/SupplierDashboard");
            }

            // Retrieve the SupplierId from session and assign it to the tender
            var supplierId = HttpContext.Session.GetString("SupplierId");
            if (string.IsNullOrEmpty(supplierId))
            {
                TempData["failure"] = "Supplier Id is missing in session.";
                return RedirectToPage("/SupplierDashboard");
            }
            tender.SupplierId = supplierId;

            string url = "https://localhost:7226/api/Tender";
            var content = new StringContent(JsonSerializer.Serialize(tender), Encoding.UTF8, "application/json");
            HttpResponseMessage message = await _httpClient.PostAsync(url, content);

            if (message.IsSuccessStatusCode)
            {
                TempData["success"] = "Tender Added Successfully";
            }
            else
            {
                string errorResponse = await message.Content.ReadAsStringAsync();
                TempData["failure"] = $"Failed to add tender: {errorResponse}";
            }

            return RedirectToPage("/SupplierDashboard");
        }
    }
}
