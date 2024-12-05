using Microsoft.AspNetCore.Mvc;

namespace AlloK8.PL.Controllers;

public class LoginController : Controller
{
    [HttpGet("/login")]
    public IActionResult Login()
    {
        return View();
    }
}