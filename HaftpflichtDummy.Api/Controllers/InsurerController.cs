using HaftpflichtDummy.BusinessLogic.Services.WebApiServices.Interfaces;
using HaftpflichtDummy.Models;
using HaftpflichtDummy.Models.InputModels;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace HaftpflichtDummy.Api.Controllers;

[ApiController]
[Route("api/gesellschaften")]
public class InsurerController : Controller
{
    private readonly ILogger<Insurer> _logger;
    private readonly IInsurerService _insurerService;

    public InsurerController(ILogger<Insurer> logger, IInsurerService insurerService)
    {
        _logger = logger;
        _insurerService = insurerService;
    }

    [SwaggerOperation("Liefert alle verfügbaren Gesellschaften")]
    [SwaggerResponse(200, "OK")]
    [SwaggerResponse(500, "Server-Fehler")]
    [HttpGet]
    public async Task<IActionResult> GetInsurers()
    {
        var result = await _insurerService.GetAllInsurers();
        return result.Success ? Ok(result) : Problem(result.ErrorMessage);
    }

    [SwaggerOperation("Fügt eine neue Gesellschaft hinzu")]
    [SwaggerResponse(200, "OK")]
    [SwaggerResponse(500, "Server-Fehler")]
    [HttpPost]
    public async Task<IActionResult> PostInsurer([FromBody] CreateOrUpdateInsurerInput orUpdateInsurer)
    {
        var result = await _insurerService.CreateInsurer(orUpdateInsurer.Name);
        return result.Success ? Ok(result) : Problem(result.ErrorMessage);
    }
}