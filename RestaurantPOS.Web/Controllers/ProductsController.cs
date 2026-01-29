using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RestaurantPOS.Domain.Entities;
using RestaurantPOS.Service.Interfaces;
using RestaurantPOS.Web.Infrastructure;

namespace RestaurantPOS.Web.Controllers
{
    [AdminAuthorize]
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly IProductCategoryService _categoryService;

        public ProductsController(
            IProductService productService,
            IProductCategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        // GET: Products
        public IActionResult Index()
        {
            var products = _productService.GetAll();
            return View(products);
        }

        // GET: Products/Details/5
        public IActionResult Details(Guid id)
        {
            var product = _productService.GetById(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create(string? name, string? description)
        {
            LoadCategories();

            var model = new RestaurantPOS.Domain.Entities.Product();

            if (!string.IsNullOrWhiteSpace(name))
            {
                model.Name = name;
            }

            if (!string.IsNullOrWhiteSpace(description))
            {
                model.Description = description;
            }

            return View(model);
        }


        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                product.Id = Guid.NewGuid();
                _productService.Create(product);
                return RedirectToAction(nameof(Index));
            }

            LoadCategories();
            return View(product);
        }

        // GET: Products/Edit/5
        public IActionResult Edit(Guid id)
        {
            var product = _productService.GetById(id);
            if (product == null)
            {
                return NotFound();
            }

            LoadCategories(product.ProductCategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                _productService.Update(product);
                return RedirectToAction(nameof(Index));
            }

            LoadCategories(product.ProductCategoryId);
            return View(product);
        }

        // GET: Products/Delete/5
        public IActionResult Delete(Guid id)
        {
            var product = _productService.GetById(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            _productService.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        private void LoadCategories(Guid? selectedCategoryId = null)
        {
            var categories = _categoryService.GetAll();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", selectedCategoryId);
        }
    }
}
