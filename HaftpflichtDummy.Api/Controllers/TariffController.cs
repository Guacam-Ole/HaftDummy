using HaftpflichtDummy.BusinessLogic.Services.WebApiServices.Interfaces;
using HaftpflichtDummy.Models.InputModels;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace HaftpflichtDummy.Api.Controllers;

[ApiController]
[Route("api/tarife")]
public class TariffsController : Controller
{
    private readonly ILogger<TariffsController> _logger;
    private readonly ITariffService _tariffService;

    public TariffsController(ILogger<TariffsController> logger, ITariffService tariffService)
    {
        _logger = logger;
        _tariffService = tariffService;
    }

    [SwaggerOperation("Liefert alle gespeicherten Tarife")]
    [SwaggerResponse(200, "OK")]
    [SwaggerResponse(500, "Server-Fehler")]
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await _tariffService.GetAllTariffs();
        return result.Success ? Ok(result) : Problem(result.ErrorMessage);
    }

    [SwaggerOperation("Liefere einen einzelnen Tarif für die Id")]
    [SwaggerResponse(200, "OK")]
    [SwaggerResponse(404, "Tarif ist in der Datenbank nicht vorhanden")]
    [SwaggerResponse(500, "Server-Fehler")]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var result = await _tariffService.GetSingleTariffById(id);
        if (result.Success) return Ok(result);
        return NotFound(result); // Je nach error dann natürlich ggf. 500 etc.
    }

    
    [SwaggerOperation("Liefere alle Features eines einen einzelnen Tarifs")]
    [SwaggerResponse(200, "OK")]
    [SwaggerResponse(404, "Tarif ist in der Datenbank nicht vorhanden")]
    [SwaggerResponse(500, "Server-Fehler")]
    [HttpGet("{id:int}/features")]
    public async Task<IActionResult> GetFeatures(int id)
    {
        var result = await _tariffService.GetFeaturesFromTariff(id);
        if (result.Success) return Ok(result);
        return NotFound(result);
    }

    [SwaggerOperation("Fügt eine Tarif hinzu")]
    [SwaggerResponse(200, "OK")]
    [SwaggerResponse(500, "Server-Fehler")]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateOrUpdateTariffInput tariff)
    {
        var result = await _tariffService.CreateTariff(tariff);
        return result.Success ? Ok(result) : Problem(result.ErrorMessage);
    }

    [SwaggerOperation("Verändert einen Tarif")]
    [SwaggerResponse(200, "OK")]
    [SwaggerResponse(404, "Tarif ist in der Datenbank nicht vorhanden")]
    [SwaggerResponse(500, "Server-Fehler")]
    [HttpPut]
    public async Task<IActionResult> Put(int id, [FromBody] CreateOrUpdateTariffInput tariff)
    {
        var result = await _tariffService.UpdateSingleTariff(id, tariff);
        return result.Success ? Ok(result) : Problem(result.ErrorMessage);
    }

    [SwaggerOperation("Berechnet Tarife")]
    [SwaggerResponse(200, "OK")]
    [SwaggerResponse(500, "Server-Fehler")]
    [HttpPost("berechnen")]
    public async Task<IActionResult> Calculate([FromBody] CalculateTariffsInput filter)
    {
        var result = await _tariffService.CalculateAllTariffs(filter);
        return result.Success ? Ok(result) : Problem(result.ErrorMessage);
    }
}