using System.Net.Http.Json;
using ECommerceMVC.Models.Api;

namespace ECommerceMVC.Services
{
    public class AuthApiService : IAuthApiService
    {
        private readonly HttpClient _httpClient;

        public AuthApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        // --------------------------------------------------
        // REGISTER
        // --------------------------------------------------

        public async Task<bool> RegisterAsync(
            RegisterRequestDto request)
        {
            try
            {
                var response =
                    await _httpClient.PostAsJsonAsync(
                        "api/Auth/register",
                        request);

                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }


        // --------------------------------------------------
        // LOGIN
        // --------------------------------------------------

        public async Task<AuthResponseDto?> LoginAsync(
            LoginRequestDto request)
        {
            try
            {
                var response =
                    await _httpClient.PostAsJsonAsync(
                        "api/Auth/login",
                        request);

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                return await response.Content
                    .ReadFromJsonAsync<AuthResponseDto>();
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }
    }
}