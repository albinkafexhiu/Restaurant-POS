using Microsoft.AspNetCore.Mvc.Rendering;

namespace RestaurantPOS.Web.Models
{
    public class OrderItemDisplay
    {
        public Guid OrderItemId { get; set; }
        public Guid ProductId { get; set; }   
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int UnitPrice { get; set; }
        public int LineTotal { get; set; }
    }

    public class CategoryFilterItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class OrderDetailsViewModel
    {
        public Guid TableId { get; set; }
        public int TableNumber { get; set; }

        public Guid? OrderId { get; set; }
        public bool HasOpenOrder => OrderId.HasValue;

        public List<OrderItemDisplay> Items { get; set; } = new();
        public List<RestaurantPOS.Domain.Entities.Product> Products { get; set; } = new();

        public List<CategoryFilterItem> Categories { get; set; } = new();

        public int Total { get; set; }
    }


    public class PosTableCardViewModel
    {
        public Guid TableId { get; set; }
        public int TableNumber { get; set; }
        public string StatusText { get; set; } = "Free";
        public string StatusBadgeClass { get; set; } = "bg-success-lt";
        public bool HasOpenOrder { get; set; }
        public Guid? OpenOrderId { get; set; }
        public int RunningTotal { get; set; }
        public int ItemsCount { get; set; }
    }

    public class PosTablesViewModel
    {
        public List<PosTableCardViewModel> Tables { get; set; } = new();
    }
}