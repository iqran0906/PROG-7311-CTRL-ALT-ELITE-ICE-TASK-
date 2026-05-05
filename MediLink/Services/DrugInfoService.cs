using System.Text.Json;
using MediLink.Dtos;
using MediLink.Interfaces;

namespace MediLink.Services
{
    public class DrugInfoService : IDrugInfoService
    {
        private readonly HttpClient _httpClient;  //Sxends the request to the API.
        private readonly IConfiguration _configuration;

        public DrugInfoService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<List<DrugInfoDto>> SearchDrugInfoAsync(string drugName)
        {
            var apiKey = _configuration["RapidApi:ApiKey"];
            var host = _configuration["RapidApi:Host"];
            var baseUrl = _configuration["RapidApi:BaseUrl"];

            if (string.IsNullOrWhiteSpace(apiKey) ||
                string.IsNullOrWhiteSpace(host) ||
                string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new Exception("RapidAPI settings are missing in appsettings.json.");
            }

            var requestUrl = $"{baseUrl}?drug={Uri.EscapeDataString(drugName)}";

            using var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Add("x-rapidapi-key", apiKey);
            request.Headers.Add("x-rapidapi-host", host);

            using var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new Exception($"API request failed: {response.StatusCode} - {errorMessage}");
            }

            var json = await response.Content.ReadAsStringAsync();

            var results = new List<DrugInfoDto>();

            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;

            if (root.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in root.EnumerateArray())
                {
                    var drugInfo = new DrugInfoDto
                    {
                        ProductNdc = item.TryGetProperty("product_ndc", out var productNdc)
                            ? productNdc.GetString() ?? string.Empty
                            : string.Empty,

                        GenericName = item.TryGetProperty("generic_name", out var genericName)
                            ? genericName.GetString() ?? string.Empty
                            : string.Empty,

                        LabelerName = item.TryGetProperty("labeler_name", out var labelerName)
                            ? labelerName.GetString() ?? string.Empty
                            : string.Empty,

                        BrandName = item.TryGetProperty("brand_name", out var brandName)
                            ? brandName.GetString() ?? string.Empty
                            : string.Empty
                    };

                    if (item.TryGetProperty("active_ingredients", out var activeIngredients) &&
                        activeIngredients.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var ingredient in activeIngredients.EnumerateArray())
                        {
                            drugInfo.ActiveIngredients.Add(new ActiveIngredientDto
                            {
                                Name = ingredient.TryGetProperty("name", out var name)
                                    ? name.GetString() ?? string.Empty
                                    : string.Empty,

                                Strength = ingredient.TryGetProperty("strength", out var strength)
                                    ? strength.GetString() ?? string.Empty
                                    : string.Empty
                            });
                        }
                    }

                    results.Add(drugInfo);
                }
            }

            return results;
        }
    }
}