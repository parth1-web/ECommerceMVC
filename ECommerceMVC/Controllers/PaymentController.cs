using ECommerceMVC.Models.Api;
using ECommerceMVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceMVC.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly IPaymentApiService _paymentApiService;
        private readonly IOrderApiService _orderApiService;


    public PaymentController(
        IPaymentApiService paymentApiService,
        IOrderApiService orderApiService)
        {
            _paymentApiService = paymentApiService;
            _orderApiService = orderApiService;
        }

        // ==================================================
        // PAYMENT SELECTION
        // GET /Payment/Select/{id}
        // ==================================================

        [HttpGet]
        public async Task<IActionResult> Select(int id)
        {
            if (id <= 0)
            {
                return RedirectToAction(
                    "Index",
                    "Orders");
            }

            var order =
                await _orderApiService
                    .GetOrderByIdAsync(id);

            if (order == null)
            {
                TempData["ErrorMessage"] =
                    "Unable to load the order.";

                return RedirectToAction(
                    "Index",
                    "Orders");
            }

            return View(order);
        }

        // ==================================================
        // PROCESS PAYMENT
        // POST /Payment/Process
        // ==================================================

        [HttpPost]
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

            var order =
                await _orderApiService
                    .GetOrderByIdAsync(orderId);

            if (order == null)
            {
                TempData["ErrorMessage"] =
                    "Order not found.";

                return RedirectToAction(
                    "Index",
                    "Orders");
            }

            // ==================================================
            // CASH ON DELIVERY
            // ==================================================

            if (method == 1)
            {
                var payment =
                    await _paymentApiService
                        .CreatePaymentAsync(
                            new PaymentRequestDto
                            {
                                OrderId = orderId,
                                Method = method
                            });

                if (payment == null)
                {
                    TempData["ErrorMessage"] =
                        "Unable to create the payment.";

                    return RedirectToAction(
                        nameof(Select),
                        new { id = orderId });
                }

                TempData["SuccessMessage"] =
                    "Cash on Delivery selected successfully.";

                return RedirectToAction(
                    "Confirmation",
                    "Checkout",
                    new { id = orderId });
            }

            // ==================================================
            // KHALTI
            // ==================================================

            if (method == 2)
            {
                var result =
                    await _paymentApiService
                        .InitiateKhaltiAsync(
                            new PaymentRequestDto
                            {
                                OrderId = orderId,
                                Method = method
                            });

                if (result == null ||
                    string.IsNullOrWhiteSpace(
                        result.PaymentUrl))
                {
                    TempData["ErrorMessage"] =
                        "Unable to initiate Khalti payment.";

                    return RedirectToAction(
                        nameof(Select),
                        new { id = orderId });
                }

                return Redirect(result.PaymentUrl);
            }

            // ==================================================
            // ESEWA
            // ==================================================

            if (method == 3)
            {
                var result =
                    await _paymentApiService
                        .InitiateEsewaAsync(orderId);

                if (result == null)
                {
                    TempData["ErrorMessage"] =
                        "Unable to initiate eSewa payment.";

                    return RedirectToAction(
                        nameof(Select),
                        new { id = orderId });
                }

                if (string.IsNullOrWhiteSpace(
                    result.PaymentUrl))
                {
                    TempData["ErrorMessage"] =
                        "eSewa payment URL was not returned.";

                    return RedirectToAction(
                        nameof(Select),
                        new { id = orderId });
                }

                if (result.FormData == null)
                {
                    TempData["ErrorMessage"] =
                        "eSewa payment form data was not returned.";

                    return RedirectToAction(
                        nameof(Select),
                        new { id = orderId });
                }

                return View(
                    "EsewaRedirect",
                    result);
            }

            // ==================================================
            // INVALID PAYMENT METHOD
            // ==================================================

            TempData["ErrorMessage"] =
                "Invalid payment method.";

            return RedirectToAction(
                nameof(Select),
                new { id = orderId });
        }

        // ==================================================
        // PAYMENT SUCCESS
        // GET /Payment/Success?orderId={id}
        //
        // This endpoint is anonymous because the API performs
        // secure server-side payment verification before the
        // browser is redirected here.
        // ==================================================

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Success(int? orderId)
        {
            return View(orderId);
        }

        // ==================================================
        // PAYMENT FAILURE
        // GET /Payment/Failure?orderId={id}
        //
        // This endpoint only displays the payment result.
        // It does not verify or trust provider payment data.
        // ==================================================

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Failure(int? orderId)
        {
            return View(orderId);
        }
    }


}
