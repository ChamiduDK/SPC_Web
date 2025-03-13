using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SPC_Main_Web.Model;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SPC_Main_Web.Pages
{
    public class AdminDBModel : PageModel
    {
        private readonly ILogger<AdminDBModel> _logger;
        private readonly HttpClient _httpClient;
        public List<Supplier> Suppliers { get; set; }
        public List<Pharmacy> Pharmacies { get; set; }

        public AdminDBModel(ILogger<AdminDBModel> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task OnGetAsync()
        {
            string suppliersUrl = "https://localhost:7226/api/Supplier";
            HttpResponseMessage suppliersResponse = await _httpClient.GetAsync(suppliersUrl);
            if (suppliersResponse.IsSuccessStatusCode)
            {
                var content = await suppliersResponse.Content.ReadAsStringAsync();
                Suppliers = JsonSerializer.Deserialize<List<Supplier>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            string pharmaciesUrl = "https://localhost:7226/api/Pharmacy";
            HttpResponseMessage pharmaciesResponse = await _httpClient.GetAsync(pharmaciesUrl);
            if (pharmaciesResponse.IsSuccessStatusCode)
            {
                var content = await pharmaciesResponse.Content.ReadAsStringAsync();
                Pharmacies = JsonSerializer.Deserialize<List<Pharmacy>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
        }
    }
}
