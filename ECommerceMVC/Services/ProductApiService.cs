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

        public async Task<ProductListResponseDto> GetProductsAsync(
            string? search = null,
            int? categoryId = null,
            string sortBy = "createdAt",
            string sortOrder = "desc",
            int page = 1,
            int pageSize = 100)
        {
            try
            {
                var queryParams = new List<string>
                {
                    $"page={page}",
                    $"pageSize={pageSize}",
                    $"sortBy={Uri.EscapeDataString(sortBy)}",
                    $"sortOrder={Uri.EscapeDataString(sortOrder)}"
                };

                if (!string.IsNullOrWhiteSpace(search))
                {
                    queryParams.Add($"search={Uri.EscapeDataString(search.Trim())}");
                }

                if (categoryId.HasValue && categoryId.Value > 0)
                {
                    queryParams.Add($"categoryId={categoryId.Value}");
                }

                var url = $"api/Products/search?{string.Join("&", queryParams)}";
                var response =
                    await _httpClient.GetFromJsonAsync<ProductListResponseDto>(url);

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