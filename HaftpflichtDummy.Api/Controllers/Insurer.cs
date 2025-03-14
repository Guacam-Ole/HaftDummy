using Microsoft.AspNetCore.Mvc;

namespace HaftpflichtDummy.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class Insurer : Controller
{
    [HttpGet(Name = "GetInsurers")]
    public IActionResult Index()
    {
        return View();
    }
}