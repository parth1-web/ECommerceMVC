using ECommerceMVC.Models;
using ECommerceMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceMVC.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductApiService _productApiService;

        public ProductsController(
            IProductApiService productApiService)
        {
            _productApiService = productApiService;
        }

        public async Task<IActionResult> Index()
        {
            var response =
                await _productApiService.GetProductsAsync();

            var viewModels = response.Items
                .Where(product => product.IsActive)
                .Select(product =>
                    new ProductViewModel
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Description = product.Description,
                        Price = product.Price,
                        Stock = product.Stock,
                        ImageUrl = product.ImageUrl
                    })
                .ToList();

            return View(viewModels);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product =
                await _productApiService.GetProductByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            var viewModel = new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                ImageUrl = product.ImageUrl
            };

            return View(viewModel);
        }
    }
}