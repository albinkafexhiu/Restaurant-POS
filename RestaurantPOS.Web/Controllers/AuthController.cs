using Microsoft.AspNetCore.Mvc;
using RestaurantPOS.Service.Interfaces;
using RestaurantPOS.Web.Infrastructure;

namespace RestaurantPOS.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IWaiterService _waiterService;

        public AuthController(IWaiterService waiterService)
        {
            _waiterService = waiterService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            var waiterId = HttpContext.Session.GetString(SessionKeys.WaiterId);
            if (!string.IsNullOrWhiteSpace(waiterId))
            {
                // If already logged in, send based on role
                var isManager = HttpContext.Session.GetString(SessionKeys.IsManager) == "1";
                return isManager
                    ? RedirectToAction("Index", "Home")
                    : RedirectToAction("Index", "Pos");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string pin, string mode) // mode = "waiter" or "manager"
        {
            if (string.IsNullOrWhiteSpace(pin))
            {
                TempData["Error"] = "Enter your PIN.";
                return View();
            }

            mode = (mode ?? "waiter").Trim().ToLower();

            var user = mode == "manager"
                ? _waiterService.LoginManagerWithPin(pin)
                : _waiterService.LoginWithPin(pin);

            if (user == null)
            {
                TempData["Error"] = mode == "manager"
                    ? "Invalid manager PIN."
                    : "Invalid waiter PIN.";
                return View();
            }

            HttpContext.Session.SetString(SessionKeys.WaiterId, user.Id.ToString());
            HttpContext.Session.SetString(SessionKeys.WaiterName, user.FullName);
            HttpContext.Session.SetString(SessionKeys.IsManager, user.IsManager ? "1" : "0");

            return user.IsManager
                ? RedirectToAction("Index", "Home")
                : RedirectToAction("Index", "Pos");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
