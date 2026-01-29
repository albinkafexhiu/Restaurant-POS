using Microsoft.AspNetCore.Mvc;
using RestaurantPOS.Domain.Entities;
using RestaurantPOS.Service.Interfaces;
using RestaurantPOS.Web.Infrastructure;

namespace RestaurantPOS.Web.Controllers
{
    [AdminAuthorize]
    public class ProductCategoriesController : Controller
    {
        private readonly IProductCategoryService _categoryService;

        public ProductCategoriesController(IProductCategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: ProductCategories
        public IActionResult Index()
        {
            var categories = _categoryService.GetAll();
            return View(categories);
        }

        // GET: ProductCategories/Details/5
        public IActionResult Details(Guid id)
        {
            var category = _categoryService.GetById(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: ProductCategories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ProductCategories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductCategory category)
        {
            if (ModelState.IsValid)
            {
                category.Id = Guid.NewGuid();
                _categoryService.Create(category);
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        // GET: ProductCategories/Edit/5
        public IActionResult Edit(Guid id)
        {
            var category = _categoryService.GetById(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: ProductCategories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, ProductCategory category)
        {
            if (id != category.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                _categoryService.Update(category);
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        // GET: ProductCategories/Delete/5
        public IActionResult Delete(Guid id)
        {
            var category = _categoryService.GetById(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: ProductCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            _categoryService.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
