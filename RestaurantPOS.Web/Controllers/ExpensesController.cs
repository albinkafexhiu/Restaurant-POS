using Microsoft.AspNetCore.Mvc;
using RestaurantPOS.Domain.Entities;
using RestaurantPOS.Service.Interfaces;
using RestaurantPOS.Web.Infrastructure;

namespace RestaurantPOS.Web.Controllers
{
    
    [AdminAuthorize]
    public class ExpensesController : Controller
    {
        private readonly IExpenseService _expenseService;

        public ExpensesController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        // GET: Expenses
        public IActionResult Index()
        {
            var expenses = _expenseService.GetAll()
                .OrderByDescending(e => e.Date);

            return View(expenses);
        }

        // GET: Expenses/Details/5
        public IActionResult Details(Guid id)
        {
            var expense = _expenseService.GetById(id);
            if (expense == null)
            {
                return NotFound();
            }

            return View(expense);
        }

        // GET: Expenses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Expenses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Expense expense)
        {
            if (ModelState.IsValid)
            {
                expense.Id = Guid.NewGuid();
                _expenseService.Create(expense);
                return RedirectToAction(nameof(Index));
            }

            return View(expense);
        }

        // GET: Expenses/Edit/5
        public IActionResult Edit(Guid id)
        {
            var expense = _expenseService.GetById(id);
            if (expense == null)
            {
                return NotFound();
            }

            return View(expense);
        }

        // POST: Expenses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, Expense expense)
        {
            if (id != expense.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                _expenseService.Update(expense);
                return RedirectToAction(nameof(Index));
            }

            return View(expense);
        }

        // GET: Expenses/Delete/5
        public IActionResult Delete(Guid id)
        {
            var expense = _expenseService.GetById(id);
            if (expense == null)
            {
                return NotFound();
            }

            return View(expense);
        }

        // POST: Expenses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            _expenseService.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
