
using System.Net.Http.Json;
using ECommerceMVC.Models.Api;

namespace ECommerceMVC.Services
{
    public class PaymentApiService : IPaymentApiService
    {
        private readonly HttpClient _httpClient;

        public PaymentApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        // ==================================================
        // CREATE PAYMENT
        // POST /api/Payments
        // ==================================================

        public async Task<PaymentDto?> CreatePaymentAsync(
            PaymentRequestDto request)
        {
            var response =
                await _httpClient.PostAsJsonAsync(
                    "api/Payments",
                    request);

            if (!response.IsSuccessStatusCode)
            {
                var error =
                    await response.Content.ReadAsStringAsync();

                Console.WriteLine(
                    $"Create payment failed: {response.StatusCode}");

                Console.WriteLine(error);

                return null;
            }

            return await response.Content
                .ReadFromJsonAsync<PaymentDto>();
        }


        // ==================================================
        // GET PAYMENT BY ORDER
        // GET /api/Payments/order/{orderId}
        // ==================================================

        public async Task<PaymentDto?>
            GetPaymentByOrderIdAsync(int orderId)
        {
            var response =
                await _httpClient.GetAsync(
                    $"api/Payments/order/{orderId}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content
                .ReadFromJsonAsync<PaymentDto>();
        }


        // ==================================================
        // KHALTI INITIATE
        // POST /api/Payments/khalti/initiate
        // ==================================================

        public async Task<KhaltiInitiateResponseDto?>
            InitiateKhaltiAsync(
                PaymentRequestDto request)
        {
            var response =
                await _httpClient.PostAsJsonAsync(
                    "api/Payments/khalti/initiate",
                    request);

            if (!response.IsSuccessStatusCode)
            {
                var error =
                    await response.Content.ReadAsStringAsync();

                Console.WriteLine(
                    $"Khalti initiation failed: " +
                    $"{response.StatusCode}");

                Console.WriteLine(error);

                return null;
            }

            return await response.Content
                .ReadFromJsonAsync<
                    KhaltiInitiateResponseDto>();
        }


        // ==================================================
        // KHALTI CALLBACK
        // GET /api/Payments/khalti/callback
        // ==================================================

        public async Task<bool> KhaltiCallbackAsync(
            string pidx)
        {
            var response =
                await _httpClient.GetAsync(
                    $"api/Payments/khalti/callback?pidx={Uri.EscapeDataString(pidx)}");

            return response.IsSuccessStatusCode;
        }


        // ==================================================
        // ESEWA INITIATE
        // POST /api/Payments/esewa/initiate
        // ==================================================

        public async Task<EsewaInitiateResponseDto?>
            InitiateEsewaAsync(int orderId)
        {
            var response =
                await _httpClient.PostAsJsonAsync(
                    "api/Payments/esewa/initiate",
                    new
                    {
                        orderId
                    });

            if (!response.IsSuccessStatusCode)
            {
                var error =
                    await response.Content.ReadAsStringAsync();

                Console.WriteLine(
                    $"eSewa initiation failed: " +
                    $"{response.StatusCode}");

                Console.WriteLine(error);

                return null;
            }

            return await response.Content
                .ReadFromJsonAsync<
                    EsewaInitiateResponseDto>();
        }
    }
}

