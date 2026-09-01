using ECommerceMVC.Models.Api;

namespace ECommerceMVC.Services
{
    public interface ICategoryApiService
    {
        Task<List<CategoryDto>> GetCategoriesAsync();

        Task<CategoryDto?> GetCategoryByIdAsync(int id);
    }
}