using Microsoft.AspNetCore.Mvc;
using RestaurantPOS.Domain.Entities;
using RestaurantPOS.Service.Interfaces;
using RestaurantPOS.Web.Infrastructure;

namespace RestaurantPOS.Web.Controllers
{
    [PosAuthorize]
    public class PosExpensesController : Controller
    {
        private readonly IExpenseService _expenseService;

        public PosExpensesController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        public IActionResult Index()
        {
            var expenses = _expenseService.GetAll()
                .OrderByDescending(x => x.Date);

            return View(expenses);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Expense { Date = DateTime.UtcNow });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Expense expense)
        {
            if (!ModelState.IsValid)
                return View(expense);

            expense.Id = Guid.NewGuid();
            _expenseService.Create(expense);

            TempData["Success"] = "Expense added.";
            return RedirectToAction(nameof(Index));
        }
    }
}