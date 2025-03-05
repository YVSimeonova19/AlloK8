using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlloK8.PL.Controllers;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class HomeController : Controller
{
    [HttpGet("/")]
    public IActionResult Index()
    {
        return this.View();
    }
}
