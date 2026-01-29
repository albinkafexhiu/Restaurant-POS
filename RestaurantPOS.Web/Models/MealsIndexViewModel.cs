using RestaurantPOS.Domain.Dto;

namespace RestaurantPOS.Web.Models
{
    public class MealsIndexViewModel
    {
        public string Query { get; set; } = string.Empty;
        public List<ExternalMealDto> Meals { get; set; } = new();
        
        public HashSet<string> ImportedExternalIds { get; set; } = new();
    }
}