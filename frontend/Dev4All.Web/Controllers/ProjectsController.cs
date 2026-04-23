using Dev4All.Web.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dev4All.Web.Controllers;

/// <summary>
/// Serves basic project browsing pages for the web frontend.
/// </summary>
 [Authorize]
public sealed class ProjectsController : Controller
{
    /// <summary>
    /// Displays the projects listing page.
    /// </summary>
    [Authorize(Policy = AppPolicies.CustomerOrAdmin)]
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    /// Displays the developer explore projects page.
    /// </summary>
    [Authorize(Policy = AppPolicies.DeveloperOrAdmin)]
    public IActionResult Explore()
    {
        return View();
    }

    /// <summary>
    /// Displays the new project creation page.
    /// </summary>
    [Authorize(Policy = AppPolicies.CustomerOrAdmin)]
    public IActionResult Create()
    {
        return View();
    }

    /// <summary>
    /// Displays the project bid page.
    /// </summary>
    [Authorize(Policy = AppPolicies.DeveloperOrAdmin)]
    public IActionResult Bid()
    {
        return View();
    }

    /// <summary>
    /// Displays the digital contract page.
    /// </summary>
    [Authorize(Policy = AppPolicies.CustomerOrAdmin)]
    public IActionResult Contract()
    {
        return View();
    }
}
