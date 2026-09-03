using System.Net.Http.Json;

using ECommerceMVC.Models.Api;

namespace ECommerceMVC.Services;

public class PaymentApiService : IPaymentApiService
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

    public async Task<PaymentDto?> CreatePaymentAsync(
        PaymentRequestDto request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                "api/Payments",
                request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                _logger.LogWarning(
                    "Create payment failed. StatusCode: {StatusCode}. Response: {Response}",
                    response.StatusCode,
                    error);

                return null;
            }

            return await response.Content
                .ReadFromJsonAsync<PaymentDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error creating payment for OrderId {OrderId}.",
                request.OrderId);

            return null;
        }
    }

    public async Task<PaymentDto?> GetPaymentByOrderIdAsync(
        int orderId)
    {
        try
        {
            var response = await _httpClient.GetAsync(
                $"api/Payments/order/{orderId}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Unable to retrieve payment for OrderId {OrderId}. StatusCode: {StatusCode}",
                    orderId,
                    response.StatusCode);

                return null;
            }

            return await response.Content
                .ReadFromJsonAsync<PaymentDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error retrieving payment for OrderId {OrderId}.",
                orderId);

            return null;
        }
    }

    public async Task<KhaltiInitiateResponseDto?>
        InitiateKhaltiAsync(
            PaymentRequestDto request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                "api/Payments/khalti/initiate",
                request);

            if (!response.IsSuccessStatusCode)
            {
                var error =
                    await response.Content.ReadAsStringAsync();

                _logger.LogWarning(
                    "Khalti initiation failed. StatusCode: {StatusCode}. Response: {Response}",
                    response.StatusCode,
                    error);

                return null;
            }

            return await response.Content
                .ReadFromJsonAsync<KhaltiInitiateResponseDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error initiating Khalti payment for OrderId {OrderId}.",
                request.OrderId);

            return null;
        }
    }

    public async Task<bool> KhaltiCallbackAsync(
        string pidx)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(pidx))
            {
                return false;
            }

            var response = await _httpClient.GetAsync(
                $"api/Payments/khalti/callback?" +
                $"pidx={Uri.EscapeDataString(pidx)}");

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error processing Khalti callback.");

            return false;
        }
    }

    public async Task<EsewaInitiateResponseDto?>
        InitiateEsewaAsync(
            int orderId)
    {
        try
        {
            var request = new
            {
                OrderId = orderId
            };

            var response = await _httpClient.PostAsJsonAsync(
                "api/Payments/esewa/initiate",
                request);

            if (!response.IsSuccessStatusCode)
            {
                var error =
                    await response.Content.ReadAsStringAsync();

                _logger.LogWarning(
                    "eSewa initiation failed. StatusCode: {StatusCode}. Response: {Response}",
                    response.StatusCode,
                    error);

                return null;
            }

            return await response.Content
                .ReadFromJsonAsync<EsewaInitiateResponseDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error initiating eSewa payment for OrderId {OrderId}.",
                orderId);

            return null;
        }
    }
}