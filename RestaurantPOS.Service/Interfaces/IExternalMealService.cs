using RestaurantPOS.Domain.Dto;

namespace RestaurantPOS.Service.Interfaces
{
    public interface IExternalMealService
    {
        Task<List<ExternalMealDto>> GetRandomMealsAsync(int count = 5);
        Task<List<ExternalMealDto>> SearchMealsAsync(string query);
        Task<ExternalMealDto?> GetMealByIdAsync(string mealId);
    }
}