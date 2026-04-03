using Microsoft.AspNetCore.Mvc;

namespace Dev4All.Web.Controllers;

/// <summary>
/// Serves basic project browsing pages for the web frontend.
/// </summary>
public sealed class ProjectsController : Controller
{
    /// <summary>
    /// Displays the projects listing page.
    /// </summary>
    public Task<IActionResult> Index(CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        return Task.FromResult<IActionResult>(View());
    }
}
