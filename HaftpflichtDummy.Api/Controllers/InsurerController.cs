using HaftpflichtDummy.BusinessLogic.Services.WebApiServices.Interfaces;
using HaftpflichtDummy.Models;
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

    [SwaggerOperation("Liefert alle verfügbaren Geselleschaften")]
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok(await _insurerService.GetAll());
    }

    [SwaggerOperation("Fügt eine neue Gesellschaft hinzu")]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Insurer insurer)
    {
        return Ok(await _insurerService.CreateInsurer(insurer));
    }
}