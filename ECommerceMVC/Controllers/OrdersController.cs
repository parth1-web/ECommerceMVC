using ECommerceMVC.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceMVC.Controllers;

[Authorize]
public class OrdersController : Controller
{
    private readonly IOrderApiService _orderApiService;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(
        IOrderApiService orderApiService,
        ILogger<OrdersController> logger)
    {
        _orderApiService = orderApiService;
        _logger = logger;
    }


    // =========================================================
    // MY ORDERS
    //
    // GET: /Orders
    // API: GET /api/Orders
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            var orders =
                await _orderApiService.GetOrdersAsync();

            return View(orders);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while loading orders.");

            TempData["ErrorMessage"] =
                "Unable to load your orders. Please try again.";

            return View(
                Enumerable.Empty<
                    ECommerceMVC.Models.Api.OrderDto>());
        }
    }


    // =========================================================
    // ORDER DETAILS
    //
    // GET: /Orders/Details/{id}
    // API: GET /api/Orders/{id}
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        if (id <= 0)
        {
            TempData["ErrorMessage"] =
                "Invalid order.";

            return RedirectToAction(nameof(Index));
        }

        try
        {
            var order =
                await _orderApiService.GetOrderByIdAsync(id);

            if (order == null)
            {
                TempData["ErrorMessage"] =
                    "The requested order could not be found.";

                return RedirectToAction(nameof(Index));
            }

            return View(order);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while loading OrderId {OrderId}.",
                id);

            TempData["ErrorMessage"] =
                "Unable to load order details. Please try again.";

            return RedirectToAction(nameof(Index));
        }
    }


    // =========================================================
    // CANCEL ORDER
    //
    // POST: /Orders/Cancel/{id}
    // API: POST /api/Orders/{id}/cancel
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        if (id <= 0)
        {
            TempData["ErrorMessage"] =
                "Invalid order.";

            return RedirectToAction(nameof(Index));
        }

        try
        {
            var success =
                await _orderApiService.CancelOrderAsync(id);

            if (!success)
            {
                TempData["ErrorMessage"] =
                    "Unable to cancel the order. " +
                    "The order may already be processed or cancelled.";

                return RedirectToAction(
                    nameof(Details),
                    new { id });
            }

            TempData["SuccessMessage"] =
                "Your order has been cancelled successfully.";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while cancelling OrderId {OrderId}.",
                id);

            TempData["ErrorMessage"] =
                "An unexpected error occurred while cancelling your order.";

            return RedirectToAction(
                nameof(Details),
                new { id });
        }
    }
}