using ECommerceMVC.Models;
using ECommerceMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceMVC.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductApiService _productApiService;
        private readonly ICategoryApiService _categoryApiService;

        public ProductsController(
            IProductApiService productApiService,
            ICategoryApiService categoryApiService)
        {
            _productApiService = productApiService;
            _categoryApiService = categoryApiService;
        }

        public async Task<IActionResult> Index(
            [FromQuery] string? search,
            [FromQuery] int? categoryId,
            [FromQuery] string? sortBy)
        {
            var response =
                await _productApiService.GetProductsAsync(
                    search: search,
                    categoryId: categoryId,
                    sortBy: string.IsNullOrWhiteSpace(sortBy) ? "createdAt" : sortBy,
                    pageSize: 100);

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

            ViewBag.SearchTerm = search;
            ViewBag.CategoryId = categoryId;
            ViewBag.SortBy = sortBy;

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                var category = await _categoryApiService.GetCategoryByIdAsync(categoryId.Value);
                ViewBag.CategoryName = category?.Name;
            }

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