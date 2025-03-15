using HaftpflichtDummy.BusinessLogic.Services.WebApiServices;
using HaftpflichtDummy.DataProviders.Repositories.Database.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using DbModels = HaftpflichtDummy.DataProviders.Models.Database;

namespace HaftpflichtDummy.UnitTests;
using Xunit;
using NSubstitute;

public class TariffServiceTests
{
    private readonly object _logger;
    private readonly ITariff _dbTariff;

    public TariffServiceTests()
    {
        _logger = Substitute.For<ILogger<TariffService>>();
        _dbTariff = Substitute.For<ITariff>();
        
    }
    
    private void MockDataBase()
    {
        var tariffs = JsonConvert.DeserializeObject<List<DbModels.Tariff>>(File.ReadAllText("json/MockTariffs.json"));
        _dbTariff.GetAllTariffs().Returns(tariffs!);
        
        var features = JsonConvert.DeserializeObject<List<DbModels.Feature>>(File.ReadAllText("json/MockFeatures.json"));
        _dbTariff.GetAllFeatures().Returns(features!);
            
        var tariffFeatures = JsonConvert.DeserializeObject<List<DbModels.TariffFeature>>(File.ReadAllText("json/MockTariffFeatures.json"));
        _dbTariff.GetAllTariffFeatures().Returns(tariffFeatures!);
    }
    
}