using ECommerceMVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceMVC.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly ICartApiService _cartApiService;
        private readonly IOrderApiService _orderApiService;

        public CheckoutController(
            ICartApiService cartApiService,
            IOrderApiService orderApiService)
        {
            _cartApiService = cartApiService;
            _orderApiService = orderApiService;
        }


        // ==================================================
        // CHECKOUT PAGE
        // GET: /Checkout
        // ==================================================

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var cart =
                await _cartApiService.GetCartAsync();

            if (cart == null)
            {
                TempData["ErrorMessage"] =
                    "Unable to load your cart.";

                return RedirectToAction(
                    "Index",
                    "Cart");
            }

            if (cart.Items == null ||
                !cart.Items.Any())
            {
                TempData["ErrorMessage"] =
                    "Your cart is empty.";

                return RedirectToAction(
                    "Index",
                    "Cart");
            }

            return View(cart);
        }


        // ==================================================
        // PLACE ORDER
        // POST: /Checkout/PlaceOrder
        // ==================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder()
        {
            var cart =
                await _cartApiService.GetCartAsync();

            if (cart == null)
            {
                TempData["ErrorMessage"] =
                    "Unable to load your cart.";

                return RedirectToAction(
                    "Index",
                    "Cart");
            }

            if (cart.Items == null ||
                !cart.Items.Any())
            {
                TempData["ErrorMessage"] =
                    "Your cart is empty.";

                return RedirectToAction(
                    "Index",
                    "Cart");
            }


            // ==============================================
            // CREATE ORDER
            // ==============================================

            var order =
                await _orderApiService.CreateOrderAsync();

            if (order == null)
            {
                TempData["ErrorMessage"] =
                    "Unable to place your order. " +
                    "Please try again.";

                return RedirectToAction(
                    nameof(Index));
            }


            // ==============================================
            // GO TO PAYMENT SELECTION
            // ==============================================

            return RedirectToAction(
                "Select",
                "Payment",
                new
                {
                    id = order.Id
                });
        }


        // ==================================================
        // ORDER CONFIRMATION
        // GET: /Checkout/Confirmation/{id}
        // ==================================================

        [HttpGet]
        public async Task<IActionResult> Confirmation(int id)
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
                    "Unable to load order confirmation.";

                return RedirectToAction(
                    "Index",
                    "Orders");
            }

            return View(order);
        }
    }
}