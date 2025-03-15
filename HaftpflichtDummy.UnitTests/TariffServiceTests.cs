using HaftpflichtDummy.BusinessLogic.Services.WebApiServices;
using HaftpflichtDummy.DataProviders.Repositories.Database.Interfaces;
using HaftpflichtDummy.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using DbModels = HaftpflichtDummy.DataProviders.Models.Database;

namespace HaftpflichtDummy.UnitTests;

using Xunit;
using NSubstitute;

public class TariffServiceTests
{
    private readonly ITariff _dbTariff;
    private readonly IInsurer _dbInsurer;
    private readonly TariffService _tariffService;
    private readonly ILogger<TariffService> _logger;

    public TariffServiceTests()
    {
        _logger = Substitute.For<ILogger<TariffService>>();
        _dbTariff = Substitute.For<ITariff>();
        _dbInsurer = Substitute.For<IInsurer>();
        _tariffService = new TariffService(_logger, _dbTariff, _dbInsurer);
    }

    private void MockDataBase()
    {
        var insurers =
            JsonConvert.DeserializeObject<List<DbModels.Insurer>>(File.ReadAllText("json/MockInsurers.json"));
        _dbInsurer.GetAllInsurers().Returns(insurers!);

        var tariffs = JsonConvert.DeserializeObject<List<DbModels.Tariff>>(File.ReadAllText("json/MockTariffs.json"));
        _dbTariff.GetAllTariffs().Returns(tariffs!);

        var features =
            JsonConvert.DeserializeObject<List<DbModels.Feature>>(File.ReadAllText("json/MockFeatures.json"));
        _dbTariff.GetAllFeatures().Returns(features!);

        var tariffFeatures =
            JsonConvert.DeserializeObject<List<DbModels.TariffFeature>>(
                File.ReadAllText("json/MockTariffFeatures.json"));
        _dbTariff.GetAllTariffFeatures().Returns(tariffFeatures!);
    }

    [Fact]
    private async Task CreateTariffCallsDatabase()
    {
        MockDataBase();

        var tariffToAdd = new Tariff
        {
            Name = "Umbrella Deluxe",
            Insurer = 2,
            Provision = 1200,
            ValidFrom = new DateTime(1901, 01, 01)
        };
        tariffToAdd.Features.AddRange([
                new Feature
                {
                    Id = 9,
                    IsEnabled = true,
                    Name = "Büro Plus"
                },
                new Feature
                {
                    Id = 8,
                    IsEnabled = true,
                    Name = "BürohaftPflicht"
                }
            ]
        );

        _dbTariff.InsertTariff(Arg.Any<DbModels.Tariff>()).Returns(5);

        var newTariff = await _tariffService.CreateTariff(tariffToAdd);

        // Make sure the insert-Tariff DB-Call is run
        await _dbTariff.Received()
            .InsertTariff(Arg.Is<DbModels.Tariff>(tariff =>
                tariff.Name == tariffToAdd.Name && tariff.Insurer == tariffToAdd.Insurer &&
                tariff.Provision == tariffToAdd.Provision && tariff.ValidFrom == tariffToAdd.ValidFrom));

        // make sure two Tariff-Connections are created
        await _dbTariff.Received(2)
            .AddTariffFeature(Arg.Is<DbModels.TariffFeature>(feature =>
                feature.TariffId == 5));

        Assert.NotNull(newTariff);
        Assert.Equal(5, newTariff.Id);
    }

    [Fact]
    private async Task CreatingTariffWithMissingFeatureThrowsError()
    {
        MockDataBase();

        var tariffToAdd = new Tariff
        {
            Name = "Umbrella Deluxe",
            Insurer = 2,
            Provision = 1200,
            ValidFrom = new DateTime(1901, 01, 01)
        };
        tariffToAdd.Features.AddRange([
                new Feature
                {
                    Id = 99,
                    IsEnabled = true,
                    Name = "Büro Plus"
                }
            ]
        );

        // Check if an exception is thrown
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _tariffService.CreateTariff(tariffToAdd));
        // No Database-Insert should be happening
        await _dbTariff.DidNotReceive().InsertTariff(Arg.Any<DbModels.Tariff>());
        // Make sure an error has been logged
        _logger.ReceivedWithAnyArgs(1).LogError("");
    }

    [Fact]
    private async Task CalculationsShouldBeCorrect()
    {
        MockDataBase();
        var calculations = (await _tariffService.CalculateAllTariffs()).ToList();
        Assert.Equal(3, calculations.Count);

        // Tariff 1 (Main Tariff)
        Assert.NotNull(calculations.FirstOrDefault(c =>
            c is { TariffId: 1, TariffModuleId: null, TotalProvision: 100 } &&
            c.Features.Count(q => q.IsEnabled) == 2));

        // Tariff 2 (Module from Base Tariff 1).
        Assert.NotNull(calculations.FirstOrDefault(c =>
            c is { TariffId: 1, TariffModuleId: 2, BaseProvision: 100, ModuleProvision: 1000, TotalProvision: 1100 } &&
            c.Features.Count(q => q.IsEnabled) == 5));

        // Tariff 4 (Main Tariff):
        Assert.NotNull(calculations.FirstOrDefault(c =>
            c is { TariffId: 4, TariffModuleId: null, TotalProvision: 450 } &&
            c.Features.Count(q => q.IsEnabled) == 1));
    }
}