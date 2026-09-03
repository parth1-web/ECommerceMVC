using System.Net.Http.Json;

using ECommerceMVC.Models.Api;

namespace ECommerceMVC.Services;

public class PaymentApiService
    : IPaymentApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PaymentApiService> _logger;

    public PaymentApiService(
        HttpClient httpClient,
        ILogger<PaymentApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }


    // =========================================================
    // CREATE PAYMENT
    //
    // POST /api/Payments
    // =========================================================

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
                await response.Content
                    .ReadAsStringAsync();

            _logger.LogWarning(
                "Create payment failed. StatusCode: {StatusCode}. Response: {Response}",
                response.StatusCode,
                error);

            return null;
        }

        return await response.Content
            .ReadFromJsonAsync<PaymentDto>();
    }


    // =========================================================
    // GET PAYMENT BY ORDER
    //
    // GET /api/Payments/order/{orderId}
    // =========================================================

    public async Task<PaymentDto?>
        GetPaymentByOrderIdAsync(
            int orderId)
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


    // =========================================================
    // KHALTI INITIATE
    //
    // POST /api/Payments/khalti/initiate
    // =========================================================

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
                await response.Content
                    .ReadAsStringAsync();

            _logger.LogWarning(
                "Khalti initiation failed. StatusCode: {StatusCode}. Response: {Response}",
                response.StatusCode,
                error);

            return null;
        }

        return await response.Content
            .ReadFromJsonAsync<
                KhaltiInitiateResponseDto>();
    }


    // =========================================================
    // KHALTI CALLBACK
    //
    // GET /api/Payments/khalti/callback
    // =========================================================

    public async Task<bool>
        KhaltiCallbackAsync(
            string pidx)
    {
        var response =
            await _httpClient.GetAsync(
                $"api/Payments/khalti/callback?" +
                $"pidx={Uri.EscapeDataString(pidx)}");

        return response.IsSuccessStatusCode;
    }


    // =========================================================
    // ESEWA INITIATE
    //
    // POST /api/Payments/esewa/initiate
    // =========================================================

    public async Task<EsewaInitiateResponseDto?>
        InitiateEsewaAsync(
            int orderId)
    {
        var request =
            new
            {
                OrderId = orderId
            };

        var response =
            await _httpClient.PostAsJsonAsync(
                "api/Payments/esewa/initiate",
                request);

        if (!response.IsSuccessStatusCode)
        {
            var error =
                await response.Content
                    .ReadAsStringAsync();

            _logger.LogWarning(
                "eSewa initiation failed. StatusCode: {StatusCode}. Response: {Response}",
                response.StatusCode,
                error);

            return null;
        }

        return await response.Content
            .ReadFromJsonAsync<
                EsewaInitiateResponseDto>();
    }
}