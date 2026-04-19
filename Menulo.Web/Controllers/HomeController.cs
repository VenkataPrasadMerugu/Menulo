using Microsoft.AspNetCore.Mvc;

namespace Menulo.Web.Controllers;

public sealed class HomeController : Controller
{
    [HttpGet("/")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet("/home/error")]
    public IActionResult Error()
    {
        return View();
    }
}
