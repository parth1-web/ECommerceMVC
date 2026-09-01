using ECommerceMVC.Models.Api;

namespace ECommerceMVC.Services
{
    public interface ICartApiService
    {
        Task<CartDto?> GetCartAsync();

        Task<bool> AddToCartAsync(
            int productId,
            int quantity);

        Task<bool> UpdateCartItemAsync(
            int productId,
            int quantity);

        Task<bool> RemoveFromCartAsync(
            int productId);

        Task<bool> ClearCartAsync();
    }
}