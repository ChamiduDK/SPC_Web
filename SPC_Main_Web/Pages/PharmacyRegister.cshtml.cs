using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;
using SPC_Main_Web.Model;

namespace SPC_Main_Web.Pages
{
    public class PharmacyRegisterModel : PageModel
    {
        [BindProperty]
        public Pharmacy Pharmacy { get; set; }

        private readonly HttpClient _httpClient;

        public PharmacyRegisterModel()
        {
            _httpClient = new HttpClient();
        }

        public async Task<IActionResult> OnPost()
        {
            string apiBaseUrl = "https://localhost:7226/api/Pharmacy"; 

            // Check if the email already exists
            string checkEmailUrl = $"https://localhost:7226/api/Pharmacy/CheckEmail?email={Pharmacy.Email}";

            HttpResponseMessage checkResponse = await _httpClient.GetAsync(checkEmailUrl);

            if (checkResponse.IsSuccessStatusCode)
            {
                bool isEmailExists = JsonSerializer.Deserialize<bool>(await checkResponse.Content.ReadAsStringAsync());

                if (isEmailExists)
                {
                    TempData["failure"] = "This email is already registered. Please use a different email.";
                    return Page();
                }
            }
            else
            {
                TempData["failure"] = "Error checking email availability.";
                return Page();
            }

            // Proceed with registration if email is not taken
            var content = new StringContent(JsonSerializer.Serialize(Pharmacy), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(apiBaseUrl, content);

            if (response.IsSuccessStatusCode)
            {
                TempData["success"] = "Pharmacy Registered Successfully!";
                return RedirectToPage("Login");
            }
            else
            {
                TempData["failure"] = "Failed to Register Pharmacy.";
                return Page();
            }
        }
    }
}
