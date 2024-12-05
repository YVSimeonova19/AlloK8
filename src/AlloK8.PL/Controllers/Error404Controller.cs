using Microsoft.AspNetCore.Mvc;

namespace AlloK8.PL.Controllers;

public class Error404Controller : Controller
{
    [HttpGet("/error-404")]
    public IActionResult Error404()
    {
        return View();
    }
}