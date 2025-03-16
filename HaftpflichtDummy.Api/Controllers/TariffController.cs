using HaftpflichtDummy.BusinessLogic.Services.WebApiServices.Interfaces;
using HaftpflichtDummy.Models;
using Microsoft.AspNetCore.Mvc;

namespace HaftpflichtDummy.Api.Controllers;

[ApiController]
[Route("tarife")]
public class TariffController : Controller
{
    private readonly ILogger<TariffController> _logger;
    private readonly ITariffService _tariffService;

    public TariffController(ILogger<TariffController> logger, ITariffService tariffService)
    {
        _logger = logger;
        _tariffService = tariffService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok(await _tariffService.GetAllTariffs());
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        return Ok(await _tariffService.GetSingleTariffById(id));
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Tariff tariff)
    {
        return Ok(await _tariffService.CreateTariff(tariff));
    }

    [HttpPut]
    public async Task<IActionResult> Put(int id, [FromBody] Tariff tariff)
    {
        return Ok(await _tariffService.UpdateSingleTariff(id, tariff));
    }

    [HttpPost("berechnen")]
    public async Task<IActionResult> Calculate([FromBody] List<int> requiredFeatures)
    {
        return Ok(await _tariffService.CalculateAllTariffs());
    }
}