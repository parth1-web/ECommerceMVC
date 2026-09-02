using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using ECommerceMVC.Models.Api;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceMVC.Controllers;

[Authorize]
[Route("Payment")]
public class PaymentController : Controller
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(
        IHttpClientFactory httpClientFactory,
        ILogger<PaymentController> logger)
    {
        _httpClient =
            httpClientFactory.CreateClient("ECommerceApi");

        _logger = logger;
    }


    // =========================================================
    // PAYMENT SELECTION
    //
    // GET:
    // /Payment/Select/270001
    // =========================================================

    [HttpGet("Select/{id:int}")]
    public async Task<IActionResult> Select(
        int id,
        CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            TempData["ErrorMessage"] =
                "Invalid order selected.";

            return RedirectToAction(
                "Index",
                "Orders");
        }


        var token = GetJwtToken();

        if (string.IsNullOrWhiteSpace(token))
        {
            TempData["ErrorMessage"] =
                "Your session has expired. Please login again.";

            return RedirectToAction(
                "Login",
                "Account");
        }


        try
        {
            using var request =
                new HttpRequestMessage(
                    HttpMethod.Get,
                    $"api/Orders/{id}");

            request.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    token);


            using var response =
                await _httpClient.SendAsync(
                    request,
                    cancellationToken);


            // =================================================
            // UNAUTHORIZED
            // =================================================

            if (response.StatusCode ==
                HttpStatusCode.Unauthorized)
            {
                ClearJwtToken();

                TempData["ErrorMessage"] =
                    "Your session has expired. Please login again.";

                return RedirectToAction(
                    "Login",
                    "Account");
            }


            // =================================================
            // NOT FOUND
            // =================================================

            if (response.StatusCode ==
                HttpStatusCode.NotFound)
            {
                TempData["ErrorMessage"] =
                    "The selected order could not be found.";

                return RedirectToAction(
                    "Index",
                    "Orders");
            }


            // =================================================
            // OTHER API ERRORS
            // =================================================

            if (!response.IsSuccessStatusCode)
            {
                var errorContent =
                    await response.Content
                        .ReadAsStringAsync(
                            cancellationToken);

                _logger.LogWarning(
                    "Unable to load payment page for OrderId {OrderId}. " +
                    "StatusCode: {StatusCode}. Response: {Response}",
                    id,
                    response.StatusCode,
                    errorContent);

                TempData["ErrorMessage"] =
                    "Unable to load the selected order.";

                return RedirectToAction(
                    "Details",
                    "Orders",
                    new { id });
            }


            // =================================================
            // DESERIALIZE ORDER
            // =================================================

            var order =
                await response.Content
                    .ReadFromJsonAsync<OrderDto>(
                        cancellationToken:
                            cancellationToken);


            if (order == null)
            {
                TempData["ErrorMessage"] =
                    "The order could not be loaded.";

                return RedirectToAction(
                    "Index",
                    "Orders");
            }


            // =================================================
            // VALIDATE ORDER STATUS
            //
            // Only Pending orders should enter payment selection.
            // =================================================

            if (order.Status != 1)
            {
                TempData["ErrorMessage"] =
                    "This order is not available for payment.";

                return RedirectToAction(
                    "Details",
                    "Orders",
                    new
                    {
                        id = order.Id
                    });
            }


            return View(order);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(
                ex,
                "HTTP error while loading payment page for OrderId {OrderId}.",
                id);

            TempData["ErrorMessage"] =
                "Unable to connect to the payment service.";

            return RedirectToAction(
                "Details",
                "Orders",
                new { id });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error while loading payment page for OrderId {OrderId}.",
                id);

            TempData["ErrorMessage"] =
                "An unexpected error occurred while loading the payment page.";

            return RedirectToAction(
                "Details",
                "Orders",
                new { id });
        }
    }


    // =========================================================
    // PROCESS PAYMENT
    //
    // POST:
    // /Payment/Process
    // =========================================================

    [HttpPost("Process")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Process(
        int orderId,
        int method,
        CancellationToken cancellationToken)
    {
        if (orderId <= 0)
        {
            TempData["ErrorMessage"] =
                "Invalid order.";

            return RedirectToAction(
                "Index",
                "Orders");
        }


        var token = GetJwtToken();

        if (string.IsNullOrWhiteSpace(token))
        {
            TempData["ErrorMessage"] =
                "Your session has expired. Please login again.";

            return RedirectToAction(
                "Login",
                "Account");
        }


        return method switch
        {
            1 => await ProcessCashOnDelivery(
                orderId,
                token,
                cancellationToken),

            2 => await ProcessKhalti(
                orderId,
                token,
                cancellationToken),

            3 => await ProcessEsewa(
                orderId,
                token,
                cancellationToken),

            _ => InvalidPaymentMethod(orderId)
        };
    }


    // =========================================================
    // INVALID PAYMENT METHOD
    // =========================================================

    private IActionResult InvalidPaymentMethod(
        int orderId)
    {
        TempData["ErrorMessage"] =
            "Please select a valid payment method.";

        return RedirectToAction(
            nameof(Select),
            new
            {
                id = orderId
            });
    }


    // =========================================================
    // CASH ON DELIVERY
    // =========================================================

    private async Task<IActionResult> ProcessCashOnDelivery(
        int orderId,
        string token,
        CancellationToken cancellationToken)
    {
        try
        {
            using var request =
                new HttpRequestMessage(
                    HttpMethod.Post,
                    "api/Payments");

            request.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    token);


            request.Content =
                JsonContent.Create(
                    new
                    {
                        OrderId = orderId,
                        Method = 1
                    });


            using var response =
                await _httpClient.SendAsync(
                    request,
                    cancellationToken);


            if (response.StatusCode ==
                HttpStatusCode.Unauthorized)
            {
                ClearJwtToken();

                TempData["ErrorMessage"] =
                    "Your session has expired. Please login again.";

                return RedirectToAction(
                    "Login",
                    "Account");
            }


            if (!response.IsSuccessStatusCode)
            {
                var errorContent =
                    await response.Content
                        .ReadAsStringAsync(
                            cancellationToken);

                _logger.LogWarning(
                    "COD payment creation failed for OrderId {OrderId}. " +
                    "StatusCode: {StatusCode}. Response: {Response}",
                    orderId,
                    response.StatusCode,
                    errorContent);

                TempData["ErrorMessage"] =
                    "Unable to confirm your Cash on Delivery order.";

                return RedirectToAction(
                    nameof(Select),
                    new
                    {
                        id = orderId
                    });
            }


            TempData["SuccessMessage"] =
                "Your Cash on Delivery order has been confirmed successfully.";


            return RedirectToAction(
                "Details",
                "Orders",
                new
                {
                    id = orderId
                });
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(
                ex,
                "HTTP error while creating COD payment for OrderId {OrderId}.",
                orderId);

            TempData["ErrorMessage"] =
                "Unable to connect to the payment service.";

            return RedirectToAction(
                nameof(Select),
                new
                {
                    id = orderId
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error while processing COD payment for OrderId {OrderId}.",
                orderId);

            TempData["ErrorMessage"] =
                "An unexpected error occurred while processing your payment.";

            return RedirectToAction(
                nameof(Select),
                new
                {
                    id = orderId
                });
        }
    }


    // =========================================================
    // KHALTI PAYMENT
    // =========================================================

    private async Task<IActionResult> ProcessKhalti(
        int orderId,
        string token,
        CancellationToken cancellationToken)
    {
        try
        {
            using var request =
                new HttpRequestMessage(
                    HttpMethod.Post,
                    "api/Payments/khalti/initiate");

            request.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    token);


            request.Content =
                JsonContent.Create(
                    new
                    {
                        OrderId = orderId,
                        Method = 2
                    });


            using var response =
                await _httpClient.SendAsync(
                    request,
                    cancellationToken);


            if (response.StatusCode ==
                HttpStatusCode.Unauthorized)
            {
                ClearJwtToken();

                TempData["ErrorMessage"] =
                    "Your session has expired. Please login again.";

                return RedirectToAction(
                    "Login",
                    "Account");
            }


            if (!response.IsSuccessStatusCode)
            {
                var errorContent =
                    await response.Content
                        .ReadAsStringAsync(
                            cancellationToken);

                _logger.LogWarning(
                    "Khalti initiation failed for OrderId {OrderId}. " +
                    "StatusCode: {StatusCode}. Response: {Response}",
                    orderId,
                    response.StatusCode,
                    errorContent);

                TempData["ErrorMessage"] =
                    "Unable to initiate Khalti payment.";

                return RedirectToAction(
                    nameof(Select),
                    new
                    {
                        id = orderId
                    });
            }


            var khaltiResponse =
                await response.Content
                    .ReadFromJsonAsync<
                        KhaltiInitiateResponseDto>(
                        cancellationToken:
                            cancellationToken);


            if (khaltiResponse == null ||
                string.IsNullOrWhiteSpace(
                    khaltiResponse.PaymentUrl))
            {
                TempData["ErrorMessage"] =
                    "Invalid Khalti payment response received.";

                return RedirectToAction(
                    nameof(Select),
                    new
                    {
                        id = orderId
                    });
            }


            return Redirect(
                khaltiResponse.PaymentUrl);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(
                ex,
                "HTTP error while initiating Khalti payment for OrderId {OrderId}.",
                orderId);

            TempData["ErrorMessage"] =
                "Unable to connect to Khalti.";

            return RedirectToAction(
                nameof(Select),
                new
                {
                    id = orderId
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error while initiating Khalti payment for OrderId {OrderId}.",
                orderId);

            TempData["ErrorMessage"] =
                "An unexpected error occurred while initiating Khalti payment.";

            return RedirectToAction(
                nameof(Select),
                new
                {
                    id = orderId
                });
        }
    }


    // =========================================================
    // ESEWA PAYMENT
    // =========================================================

    private async Task<IActionResult> ProcessEsewa(
        int orderId,
        string token,
        CancellationToken cancellationToken)
    {
        try
        {
            using var request =
                new HttpRequestMessage(
                    HttpMethod.Post,
                    "api/Payments/esewa/initiate");

            request.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    token);


            request.Content =
                JsonContent.Create(
                    new
                    {
                        OrderId = orderId
                    });


            using var response =
                await _httpClient.SendAsync(
                    request,
                    cancellationToken);


            if (response.StatusCode ==
                HttpStatusCode.Unauthorized)
            {
                ClearJwtToken();

                TempData["ErrorMessage"] =
                    "Your session has expired. Please login again.";

                return RedirectToAction(
                    "Login",
                    "Account");
            }


            if (!response.IsSuccessStatusCode)
            {
                var errorContent =
                    await response.Content
                        .ReadAsStringAsync(
                            cancellationToken);

                _logger.LogWarning(
                    "eSewa initiation failed for OrderId {OrderId}. " +
                    "StatusCode: {StatusCode}. Response: {Response}",
                    orderId,
                    response.StatusCode,
                    errorContent);

                TempData["ErrorMessage"] =
                    "Unable to initiate eSewa payment.";

                return RedirectToAction(
                    nameof(Select),
                    new
                    {
                        id = orderId
                    });
            }


            var paymentResponse =
                await response.Content
                    .ReadFromJsonAsync<
                        EsewaInitiateResponseDto>(
                        cancellationToken:
                            cancellationToken);


            if (paymentResponse == null)
            {
                TempData["ErrorMessage"] =
                    "Invalid eSewa payment response received.";

                return RedirectToAction(
                    nameof(Select),
                    new
                    {
                        id = orderId
                    });
            }


            if (string.IsNullOrWhiteSpace(
                    paymentResponse.PaymentUrl))
            {
                TempData["ErrorMessage"] =
                    "eSewa payment URL is missing.";

                return RedirectToAction(
                    nameof(Select),
                    new
                    {
                        id = orderId
                    });
            }


            if (string.IsNullOrWhiteSpace(
                    paymentResponse.TransactionUuid))
            {
                TempData["ErrorMessage"] =
                    "eSewa transaction information is missing.";

                return RedirectToAction(
                    nameof(Select),
                    new
                    {
                        id = orderId
                    });
            }


            return View(
                "EsewaRedirect",
                paymentResponse);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(
                ex,
                "HTTP error while initiating eSewa payment for OrderId {OrderId}.",
                orderId);

            TempData["ErrorMessage"] =
                "Unable to connect to the eSewa payment service.";

            return RedirectToAction(
                nameof(Select),
                new
                {
                    id = orderId
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error while initiating eSewa payment for OrderId {OrderId}.",
                orderId);

            TempData["ErrorMessage"] =
                "An unexpected error occurred while initiating eSewa payment.";

            return RedirectToAction(
                nameof(Select),
                new
                {
                    id = orderId
                });
        }
    }


    // =========================================================
    // PAYMENT SUCCESS
    //
    // GET:
    // /Payment/Success
    // =========================================================

    [AllowAnonymous]
    [HttpGet("Success")]
    public async Task<IActionResult> Success(
        int orderId,
        string? transactionUuid,
        CancellationToken cancellationToken)
    {
        if (orderId <= 0)
        {
            TempData["ErrorMessage"] =
                "Invalid payment confirmation.";

            return RedirectToAction(
                "Index",
                "Products");
        }


        PaymentDto? payment = null;

        var token = GetJwtToken();


        // =====================================================
        // Try to load payment details when authenticated.
        //
        // The success page still works without the JWT because
        // eSewa redirects through an external browser flow.
        // =====================================================

        if (!string.IsNullOrWhiteSpace(token))
        {
            try
            {
                using var request =
                    new HttpRequestMessage(
                        HttpMethod.Get,
                        $"api/Payments/order/{orderId}");

                request.Headers.Authorization =
                    new AuthenticationHeaderValue(
                        "Bearer",
                        token);


                using var response =
                    await _httpClient.SendAsync(
                        request,
                        cancellationToken);


                if (response.IsSuccessStatusCode)
                {
                    payment =
                        await response.Content
                            .ReadFromJsonAsync<
                                PaymentDto>(
                                cancellationToken:
                                    cancellationToken);
                }
                else
                {
                    _logger.LogWarning(
                        "Unable to load payment details for OrderId {OrderId}. StatusCode: {StatusCode}",
                        orderId,
                        response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "Unable to load payment display information for OrderId {OrderId}.",
                    orderId);
            }
        }


        ViewBag.OrderId = orderId;

        ViewBag.TransactionUuid =
            transactionUuid;


        return View(payment);
    }


    // =========================================================
    // PAYMENT FAILURE
    //
    // GET:
    // /Payment/Failure
    // =========================================================

    [AllowAnonymous]
    [HttpGet("Failure")]
    public IActionResult Failure(
        string? message,
        string? transactionUuid)
    {
        ViewBag.Message =
            string.IsNullOrWhiteSpace(message)
                ? "Your payment could not be completed."
                : message;

        ViewBag.TransactionUuid =
            transactionUuid;

        return View();
    }


    // =========================================================
    // GET JWT TOKEN
    // =========================================================

    private string? GetJwtToken()
    {
        return HttpContext.Session.GetString(
            "JWToken");
    }


    // =========================================================
    // CLEAR JWT TOKEN
    // =========================================================

    private void ClearJwtToken()
    {
        HttpContext.Session.Remove(
            "JWToken");
    }
}