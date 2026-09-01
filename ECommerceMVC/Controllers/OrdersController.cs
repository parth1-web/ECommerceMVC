
using ECommerceMVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceMVC.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IOrderApiService _orderApiService;

        public OrdersController(
            IOrderApiService orderApiService)
        {
            _orderApiService = orderApiService;
        }

        // ==================================================
        // MY ORDERS
        // GET /Orders
        // API: GET /api/Orders
        // ==================================================

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var orders =
                await _orderApiService.GetOrdersAsync();

            return View(orders);
        }


        // ==================================================
        // ORDER DETAILS
        // GET /Orders/Details/{id}
        // API: GET /api/Orders/{id}
        // ==================================================

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
            {
                return RedirectToAction(nameof(Index));
            }

            var order =
                await _orderApiService.GetOrderByIdAsync(id);

            if (order == null)
            {
                TempData["ErrorMessage"] =
                    "Unable to load order details.";

                return RedirectToAction(nameof(Index));
            }

            return View(order);
        }


        // ==================================================
        // CANCEL ORDER
        // POST /Orders/Cancel/{id}
        // API: POST /api/Orders/{id}/cancel
        // ==================================================

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

            var success =
                await _orderApiService.CancelOrderAsync(id);

            if (!success)
            {
                TempData["ErrorMessage"] =
                    "Unable to cancel the order. " +
                    "Please try again.";

                return RedirectToAction(
                    nameof(Details),
                    new { id });
            }

            TempData["SuccessMessage"] =
                "Order cancelled successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}

