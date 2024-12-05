using Microsoft.AspNetCore.Mvc;

namespace AlloK8.PL.Controllers;

public class RegisterController : Controller
{
    [HttpGet("/register")]
    public IActionResult Register()
    {
        return View();
    }
}