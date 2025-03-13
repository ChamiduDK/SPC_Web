using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SPC_Main_Web.Model;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace SPC_Main_Web.Pages
{
    public class EditPharmacy1Model : PageModel
    {
        private readonly HttpClient _httpClient;

        public EditPharmacy1Model()
        {
            _httpClient = new HttpClient();
        }

        [BindProperty]
        public Pharmacy Pharmacy { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            string url = $"https://localhost:7226/api/Pharmacy/{id}";
            HttpResponseMessage response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                Pharmacy = JsonSerializer.Deserialize<Pharmacy>(
                    await response.Content.ReadAsStringAsync(),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return Page();
            }
            else
            {
                TempData["failure"] = "Failed to load pharmacy.";
                return RedirectToPage("/AdminDB");
            }
        }

        public async Task<IActionResult> OnPostUpdateAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            string url = $"https://localhost:7226/api/Pharmacy/{Pharmacy.Id}";
            string jsonData = JsonSerializer.Serialize(Pharmacy);
            HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PutAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                TempData["success"] = "Pharmacy updated successfully.";
                return RedirectToPage("/AdminDB");
            }
            else
            {
                TempData["failure"] = "Failed to update pharmacy.";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            string url = $"https://localhost:7226/api/Pharmacy/{id}";
            HttpResponseMessage response = await _httpClient.DeleteAsync(url);

            if (response.IsSuccessStatusCode)
            {
                TempData["success"] = "Pharmacy deleted successfully.";
                return RedirectToPage("/AdminDB");
            }
            else
            {
                TempData["failure"] = "Failed to delete pharmacy.";
                return Page();
            }
        }
    }
}