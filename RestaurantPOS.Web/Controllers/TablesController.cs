using Microsoft.AspNetCore.Mvc;
using RestaurantPOS.Domain.Entities;
using RestaurantPOS.Service.Interfaces;
using RestaurantPOS.Web.Infrastructure;

namespace RestaurantPOS.Web.Controllers
{
    [AdminAuthorize]
    public class TablesController : Controller
    {
        private readonly ITableService _tableService;

        public TablesController(ITableService tableService)
        {
            _tableService = tableService;
        }

        // GET: Tables
        public IActionResult Index()
        {
            var tables = _tableService.GetAll();
            return View(tables);
        }

        // GET: Tables/Details/5
        public IActionResult Details(Guid id)
        {
            var table = _tableService.GetById(id);
            if (table == null)
            {
                return NotFound();
            }

            return View(table);
        }

        // GET: Tables/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tables/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(RestaurantTable table)
        {
            if (ModelState.IsValid)
            {
                table.Id = Guid.NewGuid();
                _tableService.Create(table);
                return RedirectToAction(nameof(Index));
            }

            return View(table);
        }

        // GET: Tables/Edit/5
        public IActionResult Edit(Guid id)
        {
            var table = _tableService.GetById(id);
            if (table == null)
            {
                return NotFound();
            }

            return View(table);
        }

        // POST: Tables/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, RestaurantTable table)
        {
            if (id != table.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                _tableService.Update(table);
                return RedirectToAction(nameof(Index));
            }

            return View(table);
        }

        // GET: Tables/Delete/5
        public IActionResult Delete(Guid id)
        {
            var table = _tableService.GetById(id);
            if (table == null)
            {
                return NotFound();
            }

            return View(table);
        }

        // POST: Tables/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            _tableService.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
