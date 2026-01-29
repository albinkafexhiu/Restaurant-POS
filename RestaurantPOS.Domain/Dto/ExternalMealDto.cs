namespace RestaurantPOS.Domain.Dto
{
    public class ExternalMealDto
    {
        public string ExternalId { get; set; } = string.Empty; 

        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string ShortInstructions { get; set; } = string.Empty;

        public List<string> Ingredients { get; set; } = new();
    }
}