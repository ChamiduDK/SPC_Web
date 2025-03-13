using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace SPC_Main_Web.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        [BindProperty]
        public string UserType { get; set; } // "Supplier" or "Pharmacy"

        private readonly IHttpClientFactory _httpClientFactory;

        public LoginModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> OnPost()
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(UserType))
            {
                TempData["failure"] = "Please enter valid credentials.";
                return Page();
            }

            // Validate UserType
            if (UserType != "Supplier" && UserType != "Pharmacy")
            {
                TempData["failure"] = "Invalid user type.";
                return Page();
            }

            // Determine the correct API endpoint based on the user type
            string apiUrl = UserType == "Supplier"
                ? "https://localhost:7226/api/Supplier/Login"
                : "https://localhost:7226/api/Pharmacy/Login";

            var loginDto = new { Email, Password };
            var content = new StringContent(JsonSerializer.Serialize(loginDto), Encoding.UTF8, "application/json");

            var client = _httpClientFactory.CreateClient();
            HttpResponseMessage response = await client.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                try
                {
                    var responseData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseContent);

                    if (responseData == null)
                    {
                        TempData["failure"] = "Invalid response from server.";
                        return Page();
                    }

                    if (UserType == "Supplier")
                    {
                        if (responseData.ContainsKey("supplierId"))
                        {
                            var supplierId = responseData["supplierId"].GetInt32();
                            HttpContext.Session.SetInt32("SupplierId", supplierId);
                        }
                        else
                        {
                            TempData["failure"] = "Supplier Id is missing in API response.";
                            return Page();
                        }
                    }
                    else if (UserType == "Pharmacy")
                    {
                        if (responseData.ContainsKey("pharmacyId"))
                        {
                            var pharmacyId = responseData["pharmacyId"].GetInt32();
                            HttpContext.Session.SetInt32("PharmacyId", pharmacyId);
                        }
                        else
                        {
                            TempData["failure"] = "Pharmacy Id is missing in API response.";
                            return Page();
                        }
                    }

                    // Store common session data
                    HttpContext.Session.SetString("UserType", UserType);
                    HttpContext.Session.SetString("Email", Email);

                    TempData["success"] = $"{UserType} Login Successful!";
                    return RedirectToPage($"{UserType}Dashboard");

                }
                catch (JsonException)
                {
                    TempData["failure"] = "Error parsing the response from the server.";
                    return Page();
                }
            }
            else
            {
                TempData["failure"] = "Invalid Email or Password.";
                return Page();
            }
        }
    }
}
