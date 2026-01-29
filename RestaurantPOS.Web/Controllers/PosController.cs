using Microsoft.AspNetCore.Mvc;
using RestaurantPOS.Domain.Entities;
using RestaurantPOS.Domain.Enums;
using RestaurantPOS.Service.Interfaces;
using RestaurantPOS.Web.Infrastructure;
using RestaurantPOS.Web.Models;
using System.Text;

namespace RestaurantPOS.Web.Controllers
{
    [PosAuthorize]
    public class PosController : Controller
    {
        private readonly ITableService _tableService;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly IProductCategoryService _categoryService;
        private readonly IWaiterService _waiterService;

        public PosController(
            ITableService tableService,
            IOrderService orderService,
            IProductService productService,
            IProductCategoryService categoryService,
            IWaiterService waiterService)
        {
            _tableService = tableService;
            _orderService = orderService;
            _productService = productService;
            _categoryService = categoryService;
            _waiterService = waiterService;
        }

        // POS tables grid (3B enhanced)
        public IActionResult Index()
        {
            var tables = _tableService.GetAll().OrderBy(t => t.TableNumber).ToList();

            var vm = new PosTablesViewModel();

            foreach (var t in tables)
            {
                var open = _orderService.GetOpenOrderForTable(t.Id);
                var itemsCount = 0;
                var runningTotal = 0;

                if (open != null)
                {
                    var items = _orderService.GetItemsForOrder(open.Id).ToList();
                    itemsCount = items.Sum(x => x.Quantity);
                    runningTotal = items.Sum(x => x.LineTotal);
                }

                vm.Tables.Add(new PosTableCardViewModel
                {
                    TableId = t.Id,
                    TableNumber = t.TableNumber,
                    StatusText = t.Status.ToString(),
                    StatusBadgeClass = GetTableBadgeClass(t.Status),
                    HasOpenOrder = open != null,
                    OpenOrderId = open?.Id,
                    ItemsCount = itemsCount,
                    RunningTotal = runningTotal
                });
            }

            return View(vm);
        }

        public IActionResult Table(Guid id)
        {
            var table = _tableService.GetById(id);
            if (table == null)
                return NotFound();

            var vm = BuildOrderViewModel(table);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult StartOrder(Guid tableId)
        {
            var waiterIdStr = HttpContext.Session.GetString(SessionKeys.WaiterId);
            if (string.IsNullOrWhiteSpace(waiterIdStr) || !Guid.TryParse(waiterIdStr, out var waiterId))
                return RedirectToAction("Login", "Auth");

            _orderService.OpenOrderForTable(tableId, waiterId);

            return RedirectToAction("Table", new { id = tableId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddItem(Guid tableId, Guid orderId, Guid productId, int quantity)
        {
            if (orderId == Guid.Empty || productId == Guid.Empty || quantity <= 0)
                return RedirectToAction("Table", new { id = tableId });

            _orderService.AddItem(orderId, productId, quantity);

            return RedirectToAction("Table", new { id = tableId });
        }

        // 3B: Remove now decrements in service
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveItem(Guid tableId, Guid orderItemId)
        {
            if (orderItemId != Guid.Empty)
                _orderService.RemoveItem(orderItemId);

            return RedirectToAction("Table", new { id = tableId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelOrder(Guid tableId, Guid orderId)
        {
            if (orderId == Guid.Empty)
                return RedirectToAction("Table", new { id = tableId });

            _orderService.CancelOrder(orderId);

            TempData["Success"] = "Order cancelled. Table is free.";
            return RedirectToAction("Index");
        }

        // user-initiated download: POST + iframe target
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PrintReceipt(Guid tableId, Guid orderId, PaymentMethod paymentMethod)
        {
            if (orderId == Guid.Empty)
                return RedirectToAction("Index");

            var items = _orderService.GetItemsForOrder(orderId).ToList();
            if (!items.Any())
            {
                TempData["Error"] = "Cannot print receipt for an empty order. Add items or cancel the order.";
                return RedirectToAction("Table", new { id = tableId });
            }

            _orderService.CloseOrder(orderId, paymentMethod);

            return BuildReceiptFile(tableId, orderId);
        }

        private FileResult BuildReceiptFile(Guid tableId, Guid orderId)
        {
            var order = _orderService.GetById(orderId);
            var table = _tableService.GetById(tableId);

            Waiter? waiter = null;
            if (order != null)
                waiter = _waiterService.GetById(order.WaiterId);

            var products = _productService.GetAll().ToList();
            var items = _orderService.GetItemsForOrder(orderId).ToList();

            var sb = new StringBuilder();

            sb.AppendLine("RestaurantPOS Receipt");
            sb.AppendLine("--------------------------------");
            sb.AppendLine($"Table: {(table != null ? table.TableNumber.ToString() : "N/A")}");
            sb.AppendLine($"Waiter: {waiter?.FullName ?? "N/A"}");

            if (order != null)
            {
                sb.AppendLine($"Opened: {order.OpenedAt:yyyy-MM-dd HH:mm}");
                if (order.ClosedAt.HasValue)
                    sb.AppendLine($"Closed: {order.ClosedAt.Value:yyyy-MM-dd HH:mm}");
                sb.AppendLine($"Status: {order.Status}");
                sb.AppendLine($"Payment: {order.PaymentMethod}");
            }

            sb.AppendLine("--------------------------------");

            int total = 0;

            foreach (var it in items)
            {
                var productName = products.FirstOrDefault(p => p.Id == it.ProductId)?.Name ?? "Unknown";
                total += it.LineTotal;

                sb.AppendLine(productName);
                sb.AppendLine($"  {it.Quantity} x {it.UnitPrice} MKD = {it.LineTotal} MKD");
            }

            sb.AppendLine("--------------------------------");
            sb.AppendLine($"TOTAL: {total} MKD");
            sb.AppendLine("--------------------------------");

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            var fileName = $"receipt_table_{(table?.TableNumber.ToString() ?? "NA")}_{DateTime.Now:yyyyMMdd_HHmm}.txt";

            return File(bytes, "text/plain", fileName);
        }

        private OrderDetailsViewModel BuildOrderViewModel(RestaurantTable table)
        {
            var openOrder = _orderService.GetOpenOrderForTable(table.Id);

            var products = _productService.GetAll()
                .Where(p => p.IsAvailable)
                .ToList();

            var categories = _categoryService.GetAll()
                .OrderBy(c => c.DisplayOrder)
                .ToList();

            var vm = new OrderDetailsViewModel
            {
                TableId = table.Id,
                TableNumber = table.TableNumber,
                Products = products,
                Categories = categories.Select(c => new CategoryFilterItem
                {
                    Id = c.Id,
                    Name = c.Name
                }).ToList()
            };

            if (openOrder != null)
            {
                vm.OrderId = openOrder.Id;

                var items = _orderService.GetItemsForOrder(openOrder.Id).ToList();

                vm.Items = items.Select(i =>
                {
                    var product = products.FirstOrDefault(p => p.Id == i.ProductId);
                    return new OrderItemDisplay
                    {
                        OrderItemId = i.Id,
                        ProductId = i.ProductId,
                        ProductName = product?.Name ?? "Unknown",
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                        LineTotal = i.LineTotal
                    };
                }).ToList();

                vm.Total = vm.Items.Sum(x => x.LineTotal);
            }

            return vm;
        }

        private static string GetTableBadgeClass(TableStatus status)
        {
            return status switch
            {
                TableStatus.Free => "bg-success-lt",
                TableStatus.Occupied => "bg-danger-lt",
                TableStatus.Reserved => "bg-warning-lt",
                _ => "bg-secondary-lt"
            };
        }
    }
}
