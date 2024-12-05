using Microsoft.AspNetCore.Mvc;

namespace AlloK8.PL.Controllers;

public class Error500Controller : Controller
{
    [HttpGet("/error-500")]
    public IActionResult Error500()
    {
        return View();
    }
}