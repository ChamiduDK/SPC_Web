using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SPC_Main_Web.Model;

namespace SPC_Main_Web.Pages
{
    public class EditSupplierModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public EditSupplierModel()
        {
            _httpClient = new HttpClient();
        }

        [BindProperty]
        public Supplier Supplier { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            string url = $"https://localhost:7226/api/Supplier/{id}";
            HttpResponseMessage response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                Supplier = JsonSerializer.Deserialize<Supplier>(
                    await response.Content.ReadAsStringAsync(),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return Page();
            }
            else
            {
                TempData["failure"] = "Failed to load supplier.";
                return RedirectToPage("/AdminDB");
            }
        }

        public async Task<IActionResult> OnPostUpdateAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            string url = $"https://localhost:7226/api/Supplier/{Supplier.Id}";
            string jsonData = JsonSerializer.Serialize(Supplier);
            HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PutAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                TempData["success"] = "Supplier updated successfully.";
                return RedirectToPage("/AdminDB");
            }
            else
            {
                TempData["failure"] = "Failed to update supplier.";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            string url = $"https://localhost:7226/api/Supplier/{id}";
            HttpResponseMessage response = await _httpClient.DeleteAsync(url);

            if (response.IsSuccessStatusCode)
            {
                TempData["success"] = "Supplier deleted successfully.";
                return RedirectToPage("/AdminDB");
            }
            else
            {
                TempData["failure"] = "Failed to delete supplier.";
                return Page();
            }
        }
    }

  
}
