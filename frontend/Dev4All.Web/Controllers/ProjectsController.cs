using Microsoft.AspNetCore.Mvc;

namespace Dev4All.Web.Controllers;

public sealed class ProjectsController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
