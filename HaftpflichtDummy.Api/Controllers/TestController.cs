using HaftpflichtDummy.DataProviders.Repositories.Database;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HaftpflichtDummy.Api.Controllers;

// Quick & Dirty DB-Reset mit Dummydaten
[ApiController]
[Route("api/test")]
public class TestController : Controller
{
    private readonly FakeDb _fakeDb;

    public TestController(FakeDb fakeDb)
    {
        _fakeDb = fakeDb;
    }
    
    [HttpGet("fill")]
    public IActionResult FillDb()
    {
        DummyData.LoadDummyData(_fakeDb);
        return Ok(true);
    }

    [HttpGet("empty")]
    public IActionResult EmptyDb()
    {
        _fakeDb.Empty();
        _fakeDb.BulkReplace("Feature",
            JsonConvert.DeserializeObject<List<HaftpflichtDummy.DataProviders.Models.Database.Feature>>(
                System.IO.File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
                "json/MockFeatures.json")))!);
        return Ok(true);
    }
}