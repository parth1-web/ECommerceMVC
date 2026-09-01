using ECommerceMVC.Models;
using ECommerceMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceMVC.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ICategoryApiService _categoryApiService;

        public CategoriesController(
            ICategoryApiService categoryApiService)
        {
            _categoryApiService = categoryApiService;
        }

        public async Task<IActionResult> Index()
        {
            var categories =
                await _categoryApiService.GetCategoriesAsync();

            var viewModels = categories
                .Where(category => category.IsActive)
                .Select(category =>
                    new CategoryViewModel
                    {
                        Id = category.Id,
                        Name = category.Name,
                        Description = category.Description,
                        IsActive = category.IsActive,
                        CreatedAt = category.CreatedAt
                    })
                .ToList();

            return View(viewModels);
        }

        public async Task<IActionResult> Details(int id)
        {
            var category =
                await _categoryApiService.GetCategoryByIdAsync(id);

            if (category == null || !category.IsActive)
            {
                return NotFound();
            }

            var viewModel = new CategoryViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt
            };

            return View(viewModel);
        }
    }
}