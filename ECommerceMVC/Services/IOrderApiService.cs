using ECommerceMVC.Models.Api;

namespace ECommerceMVC.Services
{
    public interface IOrderApiService
    {
        Task<OrderDto?> CreateOrderAsync();

        Task<List<OrderDto>> GetOrdersAsync();

        Task<OrderDto?> GetOrderByIdAsync(int id);

        Task<bool> CancelOrderAsync(int id);
    }
}