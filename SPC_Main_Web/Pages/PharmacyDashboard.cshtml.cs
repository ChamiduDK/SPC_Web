using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using SPC_Main_Web.Model;

namespace SPC_Main_Web.Pages
{
    public class PharmacyDashboardModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public List<Drug> AvailableDrugs { get; set; } = new List<Drug>();

        [BindProperty]
        public List<Order> NewOrders { get; set; } = new List<Order>();

        [BindProperty]
        public string PhoneNumber { get; set; }

        [BindProperty]
        public string Address { get; set; }

        public PharmacyDashboardModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!ValidateSession(out string pharmacyId)) return RedirectToPage("/Login");

            try
            {
                string drugsUrl = "https://localhost:7226/api/Drug";
                AvailableDrugs = await _httpClient.GetFromJsonAsync<List<Drug>>(drugsUrl) ?? new List<Drug>();
            }
            catch (Exception ex)
            {
                TempData["failure"] = "Error loading drug data.";
                Console.WriteLine($"Error fetching drugs: {ex.Message}");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostBuyAsync()
        {
            if (!ValidateSession(out string pharmacyId)) return RedirectToPage("/Login");

            if (NewOrders == null || !NewOrders.Any(o => o.DrugId > 0 && o.Quantity > 0))
            {
                TempData["failure"] = "No valid order submitted.";
                return RedirectToPage("/PharmacyDashboard");
            }

            decimal totalPrice = 0;
            try
            {
                foreach (var order in NewOrders)
                {
                    var drug = await FetchDrugAsync(order.DrugId);
                    if (drug == null || drug.Quantity < order.Quantity)
                    {
                        TempData["failure"] = $"Insufficient stock for {drug?.Name ?? "Unknown Drug"}.";
                        continue;
                    }

                    // Update the drug quantity after the order
                    drug.Quantity -= order.Quantity;

                    // Call the method to update the drug quantity in the database
                    var updateSuccess = await UpdateDrugQuantityAsync(drug);
                    if (!updateSuccess)
                    {
                        TempData["failure"] = $"Failed to update drug quantity for {drug?.Name ?? "Unknown Drug"}.";
                        return RedirectToPage("/PharmacyDashboard");
                    }

                    // Add order details
                    order.PharmacyId = pharmacyId;
                    order.PhoneNumber = PhoneNumber;
                    order.Address = Address;
                    order.Price = drug.Price * order.Quantity;
                    totalPrice += order.Price;

                    // Place the order
                    var result = await PlaceOrderAsync(order);
                    if (!result)
                    {
                        TempData["failure"] = $"Failed to place order for {drug?.Name ?? "Unknown Drug"}.";
                        return RedirectToPage("/PharmacyDashboard");
                    }
                }

                TempData["success"] = $"Order placed successfully. Total Price: ${totalPrice}";
            }
            catch (Exception ex)
            {
                TempData["failure"] = "An unexpected error occurred.";
                Console.WriteLine($"Order processing error: {ex.Message}");
            }

            return RedirectToPage("/PharmacyDashboard");
        }

        private async Task<bool> UpdateDrugQuantityAsync(Drug drug)
        {
            try
            {
                string updateDrugUrl = $"https://localhost:7226/api/Drug/{drug.Id}";
                var drugContent = new StringContent(JsonSerializer.Serialize(drug),
                                                    Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync(updateDrugUrl, drugContent);

                // Log response for debugging
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Failed to update drug quantity. Status code: {response.StatusCode}");
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating drug quantity: {ex.Message}");
                return false;
            }
        }


        private async Task<bool> PlaceOrderAsync(Order order)
        {
            try
            {
                string orderUrl = "https://localhost:7226/api/Order";
                var orderContent = new StringContent(JsonSerializer.Serialize(order),
                                                     Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(orderUrl, orderContent);

                // Log response for debugging
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Failed to place order. Status code: {response.StatusCode}");
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error placing order: {ex.Message}");
                return false;
            }
        }


        private bool ValidateSession(out string pharmacyId)
        {
            pharmacyId = HttpContext.Session.GetString("PharmacyId");
            if (HttpContext.Session.GetString("UserType") != "Pharmacy" || string.IsNullOrEmpty(pharmacyId))
            {
                TempData["failure"] = "Unauthorized access.";
                return false;
            }
            return true;
        }

        private async Task<Drug?> FetchDrugAsync(int drugId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<Drug>($"https://localhost:7226/api/Drug/{drugId}");
            }
            catch
            {
                return null;
            }
        }
    }

}
