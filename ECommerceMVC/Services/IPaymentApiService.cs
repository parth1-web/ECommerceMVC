using ECommerceMVC.Models.Api;

namespace ECommerceMVC.Services;

public interface IPaymentApiService
{
    Task<PaymentDto?> CreatePaymentAsync(
        PaymentRequestDto request);

    Task<PaymentDto?> GetPaymentByOrderIdAsync(
        int orderId);

    Task<KhaltiInitiateResponseDto?>
        InitiateKhaltiAsync(
            PaymentRequestDto request);

    Task<bool> KhaltiCallbackAsync(
        string pidx);

    Task<EsewaInitiateResponseDto?>
        InitiateEsewaAsync(
            int orderId);
}