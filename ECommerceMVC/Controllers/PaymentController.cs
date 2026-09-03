using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using ECommerceMVC.Models.Api;
using ECommerceMVC.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceMVC.Controllers;

[Authorize]
[Route("Payment")]
public class PaymentController : Controller
{
    private readonly IPaymentApiService _paymentApiService;
    private readonly IOrderApiService _orderApiService;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(
        IPaymentApiService paymentApiService,
        IOrderApiService orderApiService,
        ILogger<PaymentController> logger)
    {
        _paymentApiService = paymentApiService;
        _orderApiService = orderApiService;
        _logger = logger;
    }


    // =========================================================
    // PAYMENT SELECTION
    //
    // GET: /Payment/Select/{id}
    // =========================================================

    [HttpGet("Select/{id:int}")]
    public async Task<IActionResult> Select(
        int id)
    {
        if (id <= 0)
        {
            TempData["ErrorMessage"] =
                "Invalid order selected.";

            return RedirectToAction(
                "Index",
                "Orders");
        }

        try
        {
            var order =
                await _orderApiService
                    .GetOrderByIdAsync(id);

            if (order == null)
            {
                TempData["ErrorMessage"] =
                    "The selected order could not be found.";

                return RedirectToAction(
                    "Index",
                    "Orders");
            }

            // =================================================
            // VALIDATE ORDER STATUS
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
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error while loading payment page for OrderId {OrderId}.",
                id);

            TempData["ErrorMessage"] =
                "Unable to load the payment page.";

            return RedirectToAction(
                "Details",
                "Orders",
                new
                {
                    id
                });
        }
    }


    // =========================================================
    // PROCESS PAYMENT
    //
    // POST: /Payment/Process
    // =========================================================

    [HttpPost("Process")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Process(
        int orderId,
        int method)
    {
        if (orderId <= 0)
        {
            TempData["ErrorMessage"] =
                "Invalid order.";

            return RedirectToAction(
                "Index",
                "Orders");
        }

        return method switch
        {
            1 => await ProcessCashOnDelivery(orderId),

            2 => await ProcessKhalti(orderId),

            3 => await ProcessEsewa(orderId),

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

    private async Task<IActionResult> ProcessCashOnDelivery(int orderId)
    {
        try
        {
            var request = new PaymentRequestDto
            {
                OrderId = orderId,
                Method = 1
            };

            var payment =
                await _paymentApiService.CreatePaymentAsync(request);

            if (payment == null)
            {
                TempData["ErrorMessage"] =
                    "Unable to confirm your Cash on Delivery order.";

                return RedirectToAction(
                    nameof(Select),
                    new { id = orderId });
            }

            TempData["SuccessMessage"] =
                "Your Cash on Delivery order has been confirmed successfully.";

            return RedirectToAction(
                "Details",
                "Orders",
                new { id = orderId });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error while processing COD payment for OrderId {OrderId}.",
                orderId);

            TempData["ErrorMessage"] =
                "An unexpected error occurred while processing your payment.";

            return RedirectToAction(
                nameof(Select),
                new { id = orderId });
        }
    }


    // =========================================================
    // KHALTI PAYMENT
    // =========================================================

    private async Task<IActionResult> ProcessKhalti(
        int orderId)
    {
        try
        {
            var request =
                new PaymentRequestDto
                {
                    OrderId = orderId,
                    Method = 2
                };

            var response =
                await _paymentApiService
                    .InitiateKhaltiAsync(request);

            if (response == null ||
                string.IsNullOrWhiteSpace(
                    response.PaymentUrl))
            {
                TempData["ErrorMessage"] =
                    "Unable to initiate Khalti payment.";

                return RedirectToAction(
                    nameof(Select),
                    new
                    {
                        id = orderId
                    });
            }

            return Redirect(
                response.PaymentUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error while initiating Khalti payment for OrderId {OrderId}.",
                orderId);

            TempData["ErrorMessage"] =
                "Unable to initiate Khalti payment.";

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
        int orderId)
    {
        try
        {
            var response =
                await _paymentApiService
                    .InitiateEsewaAsync(orderId);

            if (response == null)
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
                    response.PaymentUrl))
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
                    response.TransactionUuid))
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
                response);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error while initiating eSewa payment for OrderId {OrderId}.",
                orderId);

            TempData["ErrorMessage"] =
                "Unable to initiate eSewa payment.";

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
    // GET: /Payment/Success
    // =========================================================

    [AllowAnonymous]
    [HttpGet("Success")]
    public async Task<IActionResult> Success(
        int orderId,
        string? transactionUuid)
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

        try
        {
            payment =
                await _paymentApiService
                    .GetPaymentByOrderIdAsync(
                        orderId);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Unable to load payment details for OrderId {OrderId}.",
                orderId);
        }

        ViewBag.OrderId = orderId;

        ViewBag.TransactionUuid =
            transactionUuid;

        return View(payment);
    }


    // =========================================================
    // PAYMENT FAILURE
    //
    // GET: /Payment/Failure
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
}