using System.Net.Http.Json;
using ECommerceMVC.Models.Api;

namespace ECommerceMVC.Services
{
    public class OrderApiService : IOrderApiService
    {
        private readonly HttpClient _httpClient;

        public OrderApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // ==================================================
        // CREATE ORDER
        // POST /api/Orders
        // ==================================================

        public async Task<OrderDto?> CreateOrderAsync()
        {
            var response = await _httpClient.PostAsync(
                "api/Orders",
                null);

            if (!response.IsSuccessStatusCode)
            {
                var error =
                    await response.Content.ReadAsStringAsync();

                Console.WriteLine(
                    $"Create order failed: {response.StatusCode}");

                Console.WriteLine(error);

                return null;
            }

            return await response.Content
                .ReadFromJsonAsync<OrderDto>();
        }


        // ==================================================
        // GET ALL ORDERS
        // GET /api/Orders
        // ==================================================

        public async Task<List<OrderDto>> GetOrdersAsync()
        {
            var response = await _httpClient.GetAsync(
                "api/Orders");

            if (!response.IsSuccessStatusCode)
            {
                return new List<OrderDto>();
            }

            var orders =
                await response.Content
                    .ReadFromJsonAsync<List<OrderDto>>();

            return orders ?? new List<OrderDto>();
        }


        // ==================================================
        // GET ORDER BY ID
        // GET /api/Orders/{id}
        // ==================================================

        public async Task<OrderDto?> GetOrderByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync(
                $"api/Orders/{id}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content
                .ReadFromJsonAsync<OrderDto>();
        }


        // ==================================================
        // CANCEL ORDER
        // POST /api/Orders/{id}/cancel
        // ==================================================

        public async Task<bool> CancelOrderAsync(int id)
        {
            var response = await _httpClient.PostAsync(
                $"api/Orders/{id}/cancel",
                null);

            if (!response.IsSuccessStatusCode)
            {
                var error =
                    await response.Content.ReadAsStringAsync();

                Console.WriteLine(
                    $"Cancel order failed: {response.StatusCode}");

                Console.WriteLine(error);

                return false;
            }

            return true;
        }
    }
}