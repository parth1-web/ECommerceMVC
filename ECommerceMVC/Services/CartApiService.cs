using System.Net.Http.Json;
using ECommerceMVC.Models.Api;

namespace ECommerceMVC.Services
{
    public class CartApiService : ICartApiService
    {
        private readonly HttpClient _httpClient;

        public CartApiService(
            HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // ==================================================
        // GET CART
        // ==================================================

        public async Task<CartDto?> GetCartAsync()
        {
            var response =
                await _httpClient.GetAsync(
                    "api/Cart");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content
                .ReadFromJsonAsync<CartDto>();
        }

        // ==================================================
        // ADD TO CART
        // ==================================================

        public async Task<bool> AddToCartAsync(
            int productId,
            int quantity)
        {
            var request = new
            {
                productId,
                quantity
            };

            var response =
                await _httpClient.PostAsJsonAsync(
                    "api/Cart/items",
                    request);

            if (!response.IsSuccessStatusCode)
            {
                var responseBody =
                    await response.Content
                        .ReadAsStringAsync();

                Console.WriteLine(
                    "====================================");

                Console.WriteLine(
                    "CART API ADD FAILED");

                Console.WriteLine(
                    $"Request: POST api/Cart/items");

                Console.WriteLine(
                    $"ProductId: {productId}");

                Console.WriteLine(
                    $"Quantity: {quantity}");

                Console.WriteLine(
                    $"Status Code: {(int)response.StatusCode}");

                Console.WriteLine(
                    $"Status: {response.StatusCode}");

                Console.WriteLine(
                    $"Response: {responseBody}");

                Console.WriteLine(
                    "====================================");
            }

            return response.IsSuccessStatusCode;
        }

        // ==================================================
        // UPDATE CART ITEM
        // ==================================================

        public async Task<bool> UpdateCartItemAsync(
            int productId,
            int quantity)
        {
            var request = new
            {
                quantity
            };

            var response =
                await _httpClient.PutAsJsonAsync(
                    $"api/Cart/items/{productId}",
                    request);

            if (!response.IsSuccessStatusCode)
            {
                var responseBody =
                    await response.Content
                        .ReadAsStringAsync();

                Console.WriteLine(
                    $"Cart update failed. " +
                    $"Status: {response.StatusCode}. " +
                    $"Response: {responseBody}");
            }

            return response.IsSuccessStatusCode;
        }

        // ==================================================
        // REMOVE CART ITEM
        // ==================================================

        public async Task<bool> RemoveFromCartAsync(
            int productId)
        {
            var response =
                await _httpClient.DeleteAsync(
                    $"api/Cart/items/{productId}");

            if (!response.IsSuccessStatusCode)
            {
                var responseBody =
                    await response.Content
                        .ReadAsStringAsync();

                Console.WriteLine(
                    $"Cart remove failed. " +
                    $"Status: {response.StatusCode}. " +
                    $"Response: {responseBody}");
            }

            return response.IsSuccessStatusCode;
        }

        // ==================================================
        // CLEAR CART
        // ==================================================

        public async Task<bool> ClearCartAsync()
        {
            var response =
                await _httpClient.DeleteAsync(
                    "api/Cart");

            if (!response.IsSuccessStatusCode)
            {
                var responseBody =
                    await response.Content
                        .ReadAsStringAsync();

                Console.WriteLine(
                    $"Cart clear failed. " +
                    $"Status: {response.StatusCode}. " +
                    $"Response: {responseBody}");
            }

            return response.IsSuccessStatusCode;
        }
    }
}