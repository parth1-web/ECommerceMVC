using ECommerceMVC.Models.Api;

namespace ECommerceMVC.Services
{
    public interface IProductApiService
    {
        Task<ProductListResponseDto> GetProductsAsync(
            string? search = null,
            int? categoryId = null,
            string sortBy = "createdAt",
            string sortOrder = "desc",
            int page = 1,
            int pageSize = 100);

        Task<ProductDto?> GetProductByIdAsync(int id);
    }
}