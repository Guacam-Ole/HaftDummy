using Microsoft.AspNetCore.Mvc;

namespace HaftpflichtDummy.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class Tariff : Controller
{
    [HttpGet(Name = "GetTariffs")]
    public IActionResult Index()
    {
        return View();
    }
}