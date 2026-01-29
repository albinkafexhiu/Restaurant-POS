using Microsoft.AspNetCore.Mvc;
using RestaurantPOS.Domain.Entities;
using RestaurantPOS.Service.Interfaces;
using RestaurantPOS.Web.Infrastructure;
using RestaurantPOS.Web.Models;

namespace RestaurantPOS.Web.Controllers
{
    [AdminAuthorize]
    public class MealsController : Controller
    {
        private readonly IExternalMealService _externalMealService;
        private readonly IProductService _productService;
        private readonly IProductCategoryService _categoryService;

        public MealsController(
            IExternalMealService externalMealService,
            IProductService productService,
            IProductCategoryService categoryService)
        {
            _externalMealService = externalMealService;
            _productService = productService;
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? q)
        {
            q = (q ?? "").Trim();

            var vm = new MealsIndexViewModel
            {
                Query = q
            };

            // build imported set (for disable button)
            vm.ImportedExternalIds = _productService.GetAll()
                .Where(p => !string.IsNullOrWhiteSpace(p.ExternalSourceId))
                .Select(p => p.ExternalSourceId!)
                .ToHashSet();

            if (!string.IsNullOrWhiteSpace(q))
            {
                vm.Meals = await _externalMealService.SearchMealsAsync(q);
            }
            else
            {
                // default: show random suggestions (nice landing)
                vm.Meals = await _externalMealService.GetRandomMealsAsync(6);
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(string mealId, string? q)
        {
            mealId = (mealId ?? "").Trim();
            q = (q ?? "").Trim();

            if (string.IsNullOrWhiteSpace(mealId))
            {
                TempData["Error"] = "Invalid meal id.";
                return RedirectToAction("Index", new { q });
            }

            // prevent duplicates
            var already = _productService.GetAll().Any(p => p.ExternalSourceId == mealId);
            if (already)
            {
                TempData["Error"] = "This meal is already imported.";
                return RedirectToAction("Index", new { q });
            }

            var meal = await _externalMealService.GetMealByIdAsync(mealId);
            if (meal == null)
            {
                TempData["Error"] = "Could not load meal from API.";
                return RedirectToAction("Index", new { q });
            }

            // ensure category exists (from API)
            var catName = string.IsNullOrWhiteSpace(meal.Category) ? "Food" : meal.Category.Trim();

            var category = _categoryService.GetAll()
                .FirstOrDefault(c => c.Name.ToLower() == catName.ToLower());

            if (category == null)
            {
                category = new ProductCategory
                {
                    Id = Guid.NewGuid(),
                    Name = catName,
                    DisplayOrder = 999
                };
                _categoryService.Create(category);
            }

            // create product
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = meal.Name,
                Description = meal.ShortInstructions,
                ProductCategoryId = category.Id,
                Price = 250, // default; manager can edit after
                IsAvailable = true,
                ExternalSourceId = meal.ExternalId
            };

            _productService.Create(product);

            TempData["Success"] = $"Imported: {meal.Name}";
            return RedirectToAction("Index", new { q });
        }
    }
}
