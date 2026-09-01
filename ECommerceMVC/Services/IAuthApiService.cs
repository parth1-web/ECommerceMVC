using ECommerceMVC.Models.Api;

namespace ECommerceMVC.Services
{
    public interface IAuthApiService
    {
        Task<AuthResponseDto?> LoginAsync(
            LoginRequestDto request);

        Task<bool> RegisterAsync(
            RegisterRequestDto request);
    }
}