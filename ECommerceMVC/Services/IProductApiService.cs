using ECommerceMVC.Models.Api;

namespace ECommerceMVC.Services
{
    public interface IProductApiService
    {
        Task<ProductListResponseDto> GetProductsAsync();

        Task<ProductDto?> GetProductByIdAsync(int id);
    }
}