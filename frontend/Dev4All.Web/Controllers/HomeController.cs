using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Dev4All.Web.Models;
using Dev4All.Web.Infrastructure;

namespace Dev4All.Web.Controllers;

public sealed class HomeController : Controller
{
    public IActionResult Index()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            if (User.IsInRole(AppRoles.Developer))
            {
                return RedirectToAction("Explore", "Projects");
            }

            return RedirectToAction("Index", "Projects");
        }

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

    [HttpGet("/Home/StatusCode")]
    public IActionResult StatusCodePage(int code)
    {
        ViewData["StatusCode"] = code;
        return View("StatusCode");
    }
}
