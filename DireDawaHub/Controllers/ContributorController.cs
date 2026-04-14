using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DireDawaHub.Data;

namespace DireDawaHub.Controllers;

[Authorize(Roles = "Contributor")]
public class ContributorController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
