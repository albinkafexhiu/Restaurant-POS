using RestaurantPOS.Domain.Entities;
using RestaurantPOS.Domain.Enums;
using RestaurantPOS.Repository.Interfaces;
using RestaurantPOS.Service.Interfaces;

namespace RestaurantPOS.Service.Implementation
{
    public class OrderService : IOrderService
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<RestaurantTable> _tableRepository;

        public OrderService(
            IRepository<Order> orderRepository,
            IRepository<OrderItem> orderItemRepository,
            IRepository<Product> productRepository,
            IRepository<RestaurantTable> tableRepository)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _productRepository = productRepository;
            _tableRepository = tableRepository;
        }

        public IEnumerable<Order> GetAll()
        {
            return _orderRepository.GetAll();
        }

        public Order? GetById(Guid id)
        {
            return _orderRepository.Get(id);
        }

        public Order? GetOpenOrderForTable(Guid tableId)
        {
            return _orderRepository
                .GetAll()
                .FirstOrDefault(o => o.RestaurantTableId == tableId && o.Status == OrderStatus.Open);
        }

        public IEnumerable<OrderItem> GetItemsForOrder(Guid orderId)
        {
            return _orderItemRepository
                .GetAll()
                .Where(oi => oi.OrderId == orderId);
        }

        public Order OpenOrderForTable(Guid tableId, Guid waiterId)
        {
            var existingOpenOrder = GetOpenOrderForTable(tableId);

            if (existingOpenOrder != null)
            {
                return existingOpenOrder;
            }

            var order = new Order
            {
                Id = Guid.NewGuid(),
                RestaurantTableId = tableId,
                WaiterId = waiterId,
                OpenedAt = DateTime.UtcNow,
                Status = OrderStatus.Open,
                PaymentMethod = PaymentMethod.Unknown,
                TotalAmount = 0
            };

            _orderRepository.Insert(order);

            var table = _tableRepository.Get(tableId);
            if (table != null)
            {
                table.Status = TableStatus.Occupied;
                _tableRepository.Update(table);
            }

            return order;
        }

        public void AddItem(Guid orderId, Guid productId, int quantity)
        {
            var order = _orderRepository.Get(orderId);
            var product = _productRepository.Get(productId);

            if (order == null || product == null || quantity <= 0)
            {
                return;
            }

            var existingItem = _orderItemRepository
                .GetAll()
                .FirstOrDefault(oi => oi.OrderId == orderId && oi.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                existingItem.LineTotal = existingItem.Quantity * existingItem.UnitPrice;
                _orderItemRepository.Update(existingItem);
            }
            else
            {
                var orderItem = new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = orderId,
                    ProductId = productId,
                    Quantity = quantity,
                    UnitPrice = product.Price,
                    LineTotal = quantity * product.Price
                };

                _orderItemRepository.Insert(orderItem);
            }
        }

        // 3B CHANGE:
        // RemoveItem now DECREMENTS quantity by 1, and deletes only if quantity hits 0.
        public void RemoveItem(Guid orderItemId)
        {
            var item = _orderItemRepository.Get(orderItemId);
            if (item == null)
                return;

            if (item.Quantity > 1)
            {
                item.Quantity -= 1;
                item.LineTotal = item.Quantity * item.UnitPrice;
                _orderItemRepository.Update(item);
            }
            else
            {
                _orderItemRepository.Delete(item);
            }
        }

        public void CloseOrder(Guid orderId, PaymentMethod paymentMethod)
        {
            var order = _orderRepository.Get(orderId);
            if (order == null)
            {
                return;
            }

            var items = GetItemsForOrder(orderId).ToList();
            var total = items.Sum(i => i.LineTotal);

            order.TotalAmount = total;
            order.Status = OrderStatus.Closed;
            order.PaymentMethod = paymentMethod;
            order.ClosedAt = DateTime.UtcNow;

            _orderRepository.Update(order);

            var table = _tableRepository.Get(order.RestaurantTableId);
            if (table != null)
            {
                table.Status = TableStatus.Free;
                _tableRepository.Update(table);
            }
        }

        public void CancelOrder(Guid orderId)
        {
            var order = _orderRepository.Get(orderId);
            if (order == null)
                return;

            order.Status = OrderStatus.Cancelled;
            order.PaymentMethod = PaymentMethod.Unknown;
            order.TotalAmount = 0;
            order.ClosedAt = DateTime.UtcNow;

            _orderRepository.Update(order);

            var table = _tableRepository.Get(order.RestaurantTableId);
            if (table != null)
            {
                table.Status = TableStatus.Free;
                _tableRepository.Update(table);
            }
        }
    }
}
