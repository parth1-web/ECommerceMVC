using ECommerceMVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceMVC.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartApiService _cartApiService;

        public CartController(
            ICartApiService cartApiService)
        {
            _cartApiService = cartApiService;
        }

        // ==================================================
        // CART INDEX
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

                return View();
            }

            return View(cart);
        }

        // ==================================================
        // ADD TO CART
        // ==================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(
            int productId,
            int quantity = 1)
        {
            if (productId <= 0)
            {
                TempData["ErrorMessage"] =
                    "Invalid product.";

                return RedirectToAction(
                    "Index",
                    "Products");
            }

            if (quantity < 1)
            {
                quantity = 1;
            }

            var success =
                await _cartApiService.AddToCartAsync(
                    productId,
                    quantity);

            if (!success)
            {
                TempData["ErrorMessage"] =
                    "Unable to add product to cart.";

                return RedirectToAction(
                    "Index",
                    "Products");
            }

            TempData["SuccessMessage"] =
                "Product added to cart successfully.";

            return RedirectToAction(
                nameof(Index));
        }

        // ==================================================
        // UPDATE
        // ==================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(
            int productId,
            int quantity)
        {
            if (productId <= 0)
            {
                TempData["ErrorMessage"] =
                    "Invalid product.";

                return RedirectToAction(
                    nameof(Index));
            }

            if (quantity < 1)
            {
                quantity = 1;
            }

            var success =
                await _cartApiService
                    .UpdateCartItemAsync(
                        productId,
                        quantity);

            if (!success)
            {
                TempData["ErrorMessage"] =
                    "Unable to update cart item.";

                return RedirectToAction(
                    nameof(Index));
            }

            TempData["SuccessMessage"] =
                "Cart updated successfully.";

            return RedirectToAction(
                nameof(Index));
        }

        // ==================================================
        // REMOVE
        // ==================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(
            int productId)
        {
            var success =
                await _cartApiService
                    .RemoveFromCartAsync(productId);

            if (!success)
            {
                TempData["ErrorMessage"] =
                    "Unable to remove item from cart.";

                return RedirectToAction(
                    nameof(Index));
            }

            TempData["SuccessMessage"] =
                "Item removed from cart.";

            return RedirectToAction(
                nameof(Index));
        }

        // ==================================================
        // CLEAR
        // ==================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Clear()
        {
            var success =
                await _cartApiService
                    .ClearCartAsync();

            if (!success)
            {
                TempData["ErrorMessage"] =
                    "Unable to clear cart.";

                return RedirectToAction(
                    nameof(Index));
            }

            TempData["SuccessMessage"] =
                "Cart cleared successfully.";

            return RedirectToAction(
                nameof(Index));
        }
    }
}