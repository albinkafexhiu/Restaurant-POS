using Microsoft.AspNetCore.Mvc;
using RestaurantPOS.Domain.Entities;
using RestaurantPOS.Service.Interfaces;
using RestaurantPOS.Web.Infrastructure;

namespace RestaurantPOS.Web.Controllers
{
    [AdminAuthorize]
    public class WaitersController : Controller
    {
        private readonly IWaiterService _waiterService;

        public WaitersController(IWaiterService waiterService)
        {
            _waiterService = waiterService;
        }

        // GET: Waiters
        public IActionResult Index()
        {
            var waiters = _waiterService.GetAll();
            return View(waiters);
        }

        // GET: Waiters/Details/5
        public IActionResult Details(Guid id)
        {
            var waiter = _waiterService.GetById(id);
            if (waiter == null)
            {
                return NotFound();
            }

            return View(waiter);
        }

        // GET: Waiters/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Waiters/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Waiter waiter)
        {
            if (ModelState.IsValid)
            {
                waiter.Id = Guid.NewGuid();
                _waiterService.Create(waiter);
                return RedirectToAction(nameof(Index));
            }

            return View(waiter);
        }

        // GET: Waiters/Edit/5
        public IActionResult Edit(Guid id)
        {
            var waiter = _waiterService.GetById(id);
            if (waiter == null)
            {
                return NotFound();
            }

            return View(waiter);
        }

        // POST: Waiters/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, Waiter waiter)
        {
            if (id != waiter.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                _waiterService.Update(waiter);
                return RedirectToAction(nameof(Index));
            }

            return View(waiter);
        }

        // GET: Waiters/Delete/5
        public IActionResult Delete(Guid id)
        {
            var waiter = _waiterService.GetById(id);
            if (waiter == null)
            {
                return NotFound();
            }

            return View(waiter);
        }

        // POST: Waiters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            _waiterService.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
