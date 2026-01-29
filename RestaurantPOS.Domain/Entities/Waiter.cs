using RestaurantPOS.Domain.Enums;

namespace RestaurantPOS.Domain.Entities
{
    public class Waiter : BaseEntity
    {
        public string FullName { get; set; } = string.Empty;
        public string PinCode { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        
        public bool IsManager { get; set; } = false;

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}