using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RestaurantPOS.Web.Infrastructure;
using RestaurantPOS.Web.Models;

namespace RestaurantPOS.Web.Controllers;

[AdminAuthorize]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}