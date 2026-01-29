using RestaurantPOS.Domain.Entities;
using RestaurantPOS.Domain.Enums;

namespace RestaurantPOS.Service.Interfaces
{
    public interface IOrderService
    {
        IEnumerable<Order> GetAll();
        Order? GetById(Guid id);

        Order? GetOpenOrderForTable(Guid tableId);
        IEnumerable<OrderItem> GetItemsForOrder(Guid orderId);

        Order OpenOrderForTable(Guid tableId, Guid waiterId);

        void AddItem(Guid orderId, Guid productId, int quantity);
        void RemoveItem(Guid orderItemId);

        void CloseOrder(Guid orderId, PaymentMethod paymentMethod);
        void CancelOrder(Guid orderId);
    }
}