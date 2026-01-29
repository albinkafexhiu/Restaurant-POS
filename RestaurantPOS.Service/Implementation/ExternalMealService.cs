using System.Text.Json;
using RestaurantPOS.Domain.Dto;
using RestaurantPOS.Service.Interfaces;

namespace RestaurantPOS.Service.Implementation
{
    public class ExternalMealService : IExternalMealService
    {
        private readonly HttpClient _httpClient;

        public ExternalMealService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress ??= new Uri("https://www.themealdb.com/");
        }

        public async Task<List<ExternalMealDto>> GetRandomMealsAsync(int count = 5)
        {
            var result = new List<ExternalMealDto>();

            for (int i = 0; i < count; i++)
            {
                var response = await _httpClient.GetAsync("api/json/v1/1/random.php");
                if (!response.IsSuccessStatusCode) continue;

                var json = await response.Content.ReadAsStringAsync();
                var data = Deserialize(json);

                var meal = data?.Meals?.FirstOrDefault();
                if (meal == null) continue;

                result.Add(MapToDto(meal));
            }

            return result;
        }

        public async Task<List<ExternalMealDto>> SearchMealsAsync(string query)
        {
            query = (query ?? "").Trim();
            if (string.IsNullOrWhiteSpace(query))
                return new List<ExternalMealDto>();

            var response = await _httpClient.GetAsync($"api/json/v1/1/search.php?s={Uri.EscapeDataString(query)}");
            if (!response.IsSuccessStatusCode)
                return new List<ExternalMealDto>();

            var json = await response.Content.ReadAsStringAsync();
            var data = Deserialize(json);

            if (data?.Meals == null || data.Meals.Count == 0)
                return new List<ExternalMealDto>();

            // return a clean list for UI
            return data.Meals
                .Where(m => !string.IsNullOrWhiteSpace(m.idMeal))
                .Select(MapToDto)
                .ToList();
        }

        public async Task<ExternalMealDto?> GetMealByIdAsync(string mealId)
        {
            mealId = (mealId ?? "").Trim();
            if (string.IsNullOrWhiteSpace(mealId))
                return null;

            var response = await _httpClient.GetAsync($"api/json/v1/1/lookup.php?i={Uri.EscapeDataString(mealId)}");
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            var data = Deserialize(json);

            var meal = data?.Meals?.FirstOrDefault();
            if (meal == null)
                return null;

            return MapToDto(meal);
        }

        private static TheMealDbResponse? Deserialize(string json)
        {
            return JsonSerializer.Deserialize<TheMealDbResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        private static ExternalMealDto MapToDto(MealItem meal)
        {
            return new ExternalMealDto
            {
                ExternalId = meal.idMeal ?? string.Empty,
                Name = meal.strMeal ?? string.Empty,
                Category = meal.strCategory ?? string.Empty,
                Area = meal.strArea ?? string.Empty,
                ShortInstructions = BuildShortInstructions(meal.strInstructions),
                Ingredients = ExtractIngredients(meal)
            };
        }

        private static List<string> ExtractIngredients(MealItem meal)
        {
            var list = new List<string?>()
            {
                meal.strIngredient1, meal.strIngredient2, meal.strIngredient3,
                meal.strIngredient4, meal.strIngredient5, meal.strIngredient6,
                meal.strIngredient7, meal.strIngredient8, meal.strIngredient9,
                meal.strIngredient10, meal.strIngredient11, meal.strIngredient12,
                meal.strIngredient13, meal.strIngredient14, meal.strIngredient15,
                meal.strIngredient16, meal.strIngredient17, meal.strIngredient18,
                meal.strIngredient19, meal.strIngredient20
            };

            return list
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x!.Trim())
                .ToList();
        }

        private static string BuildShortInstructions(string? full)
        {
            if (string.IsNullOrWhiteSpace(full))
                return string.Empty;

            var trimmed = full.Trim();
            if (trimmed.Length <= 180)
                return trimmed;

            return trimmed.Substring(0, 180) + "...";
        }

        // internal classes for JSON mapping (service only)
        private class TheMealDbResponse
        {
            public List<MealItem>? Meals { get; set; }
        }

        private class MealItem
        {
            public string? idMeal { get; set; }
            public string? strMeal { get; set; }
            public string? strCategory { get; set; }
            public string? strArea { get; set; }
            public string? strInstructions { get; set; }

            public string? strIngredient1 { get; set; }
            public string? strIngredient2 { get; set; }
            public string? strIngredient3 { get; set; }
            public string? strIngredient4 { get; set; }
            public string? strIngredient5 { get; set; }
            public string? strIngredient6 { get; set; }
            public string? strIngredient7 { get; set; }
            public string? strIngredient8 { get; set; }
            public string? strIngredient9 { get; set; }
            public string? strIngredient10 { get; set; }
            public string? strIngredient11 { get; set; }
            public string? strIngredient12 { get; set; }
            public string? strIngredient13 { get; set; }
            public string? strIngredient14 { get; set; }
            public string? strIngredient15 { get; set; }
            public string? strIngredient16 { get; set; }
            public string? strIngredient17 { get; set; }
            public string? strIngredient18 { get; set; }
            public string? strIngredient19 { get; set; }
            public string? strIngredient20 { get; set; }
        }
    }
}
