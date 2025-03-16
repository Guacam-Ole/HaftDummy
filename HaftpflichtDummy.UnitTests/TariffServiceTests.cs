using HaftpflichtDummy.BusinessLogic.Services.WebApiServices;
using HaftpflichtDummy.DataProviders.Repositories.Database.Interfaces;
using HaftpflichtDummy.Models;
using HaftpflichtDummy.Models.InputModels;
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
    private readonly PayloadService _payloadService;

    public TariffServiceTests()
    {
        _logger = Substitute.For<ILogger<TariffService>>();
        _dbTariff = Substitute.For<ITariff>();
        _dbInsurer = Substitute.For<IInsurer>();

        _payloadService = Substitute.For<PayloadService>(Substitute.For<ILogger<PayloadService>>());
        _tariffService = new TariffService(_logger, _dbTariff, _dbInsurer, _payloadService);
    }

    private void MockDataBase()
    {
        var insurers =
            JsonConvert.DeserializeObject<List<DbModels.Insurer>>(File.ReadAllText("json/MockInsurers.json"));
        _dbInsurer.SelectAllInsurers().Returns(insurers!);

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
    private async Task CreateTariffShouldCallDatabase()
    {
        MockDataBase();

        var tariffToAdd = new CreateOrUpdateTariffInput
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

        _dbTariff.InsertTariff(Arg.Any<DbModels.Tariff>()).Returns(new DbModels.Tariff
        {
            Id = 5,
            Name = tariffToAdd.Name
        });

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
        Assert.Equal(5, newTariff.ResponseObject!.Id);
    }

    [Fact]
    private async Task CreatingTariffWithMissingFeatureShouldReturnError()
    {
        MockDataBase();
        var tariffToAdd = new CreateOrUpdateTariffInput
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

        await _tariffService.CreateTariff(tariffToAdd);

        // Check if an error is returned
        _payloadService.ReceivedWithAnyArgs().CreateError<Tariff>(default, default);
        // No Database-Insert should be happening
        await _dbTariff.DidNotReceive().InsertTariff(Arg.Any<DbModels.Tariff>());
        // Make sure an error has been logged
        _logger.ReceivedWithAnyArgs().LogError("");
    }

    [Theory]
    [InlineData(null, 3)]
    [InlineData(null, 1, 8)] // "Bürohaftpflicht"
    [InlineData(null, 0, 9)] // "Büro Plus"
    [InlineData(null, 2, 4, 5)] // "Sportboote+Segelboote
    [InlineData(null, 1, 4, 5, 2)] // "Sportboote+Segelboote+Panama
    private async Task CalculationsShouldFilterCorrectly(int? insurer, int expectedNumberOfResults, params int[] ids)
    {
        MockDataBase();

        var calculations = (await _tariffService.CalculateAllTariffs(
            new CalculateTariffsInput
            {
                Insurer = insurer, RequiredFeatures = ids.ToList()
            })).ResponseObject!;

        Assert.Equal(expectedNumberOfResults, calculations.Count);
    }

    [Fact]
    private async Task CalculationsShouldBeAddedCorrectly()
    {
        MockDataBase();

        var calculations = (await _tariffService.CalculateAllTariffs(new CalculateTariffsInput{ RequiredFeatures = [] })).ResponseObject!;

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