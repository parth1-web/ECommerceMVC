using System.Net.Http.Json;
using ECommerceMVC.Models.Api;

namespace ECommerceMVC.Services
{
    public class CategoryApiService : ICategoryApiService
    {
        private readonly HttpClient _httpClient;

        public CategoryApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<CategoryDto>> GetCategoriesAsync()
        {
            try
            {
                var categories =
                    await _httpClient.GetFromJsonAsync<List<CategoryDto>>(
                        "api/Categories");

                return categories ?? new List<CategoryDto>();
            }
            catch (HttpRequestException)
            {
                return new List<CategoryDto>();
            }
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<CategoryDto>(
                    $"api/Categories/{id}");
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }
    }
}