using HaftpflichtDummy.BusinessLogic.Services.WebApiServices.Interfaces;
using HaftpflichtDummy.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace HaftpflichtDummy.Api.Controllers;

[ApiController]
[Route("api/tarife")]
public class TariffController : Controller
{
    private readonly ILogger<TariffController> _logger;
    private readonly ITariffService _tariffService;

    public TariffController(ILogger<TariffController> logger, ITariffService tariffService)
    {
        _logger = logger;
        _tariffService = tariffService;
    }

    [SwaggerOperation("Liefert alle gespeicherten Tarife")]
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok(await _tariffService.GetAllTariffs());
    }

    // todo: Unterschiedliche Responsecodes je nach result exemplarisch nur hier. Wäre dann natürlich überall
    [SwaggerOperation("Ein einzelner Tarif")]
    [SwaggerResponse(200, "OK")]
    [SwaggerResponse(404, "Tarif ist in der Datenbank nicht vorhanden")]
    [SwaggerResponse(500, "Server-Fehler")]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
 
        var response = await _tariffService.GetSingleTariffById(id);
        if (response.Success) return Ok(response);
        return NotFound(response); // Je nach error dann natürlich ggf. 500 etc.
    }

    [SwaggerOperation("Fügt eine neue Gesellschaft hinzu")]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Tariff tariff)
    {
        return Ok(await _tariffService.CreateTariff(tariff));
    }

    [SwaggerOperation("Fügt eine neue Gesellschaft hinzu")]
    [HttpPut]
    public async Task<IActionResult> Put(int id, [FromBody] Tariff tariff)
    {
        return Ok(await _tariffService.UpdateSingleTariff(id, tariff));
    }

    [SwaggerOperation("Fügt eine neue Gesellschaft hinzu")]
    [HttpPost("berechnen")]
    public async Task<IActionResult> Calculate([FromBody] List<int> requiredFeatures)
    {
        return Ok(await _tariffService.CalculateAllTariffs(requiredFeatures));
    }
}