using System.Net.Http.Json;
using ECommerceMVC.Models.Api;

namespace ECommerceMVC.Services
{
    public class ProductApiService : IProductApiService
    {
        private readonly HttpClient _httpClient;

        public ProductApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ProductListResponseDto> GetProductsAsync()
        {
            try
            {
                var response =
                    await _httpClient.GetFromJsonAsync<ProductListResponseDto>(
                        "api/Products/search");

                return response ?? new ProductListResponseDto();
            }
            catch (HttpRequestException)
            {
                return new ProductListResponseDto();
            }
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<ProductDto>(
                    $"api/Products/{id}");
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }
    }
}