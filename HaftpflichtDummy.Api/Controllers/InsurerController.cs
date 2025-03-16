using System.Reflection.Metadata.Ecma335;
using HaftpflichtDummy.BusinessLogic.Services.WebApiServices.Interfaces;
using HaftpflichtDummy.Models;
using Microsoft.AspNetCore.Mvc;

namespace HaftpflichtDummy.Api.Controllers;

[ApiController]
[Route("gesellschaften")]
public class InsurerController : Controller
{
    private readonly ILogger<Insurer> _logger;
    private readonly IInsurerService _insurerService;

    public InsurerController(ILogger<Insurer> logger, IInsurerService insurerService)
    {
        _logger = logger;
        _insurerService = insurerService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok(await _insurerService.GetAll());
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Insurer insurer)
    {
        return Ok(await _insurerService.CreateInsurer(insurer));
    }
}