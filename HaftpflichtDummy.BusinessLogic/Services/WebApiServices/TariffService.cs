using HaftpflichtDummy.BusinessLogic.Mapper;
using HaftpflichtDummy.BusinessLogic.Services.WebApiServices.Interfaces;
using DbModels = HaftpflichtDummy.DataProviders.Models.Database;
using HaftpflichtDummy.DataProviders.Repositories.Database.Interfaces;
using HaftpflichtDummy.Models;
using HaftpflichtDummy.Models.InputModels;
using Microsoft.Extensions.Logging;
using Tariff = HaftpflichtDummy.Models.Tariff;

namespace HaftpflichtDummy.BusinessLogic.Services.WebApiServices;

public class TariffService : ITariffService
{
    private readonly ILogger<TariffService> _logger;
    private readonly ITariff _databaseTariff;
    private readonly IInsurer _databaseInsurer;
    private readonly PayloadService _payloadService;

    public TariffService(ILogger<TariffService> logger, ITariff databaseTariff, IInsurer databaseInsurer,
        PayloadService payloadService)
    {
        _logger = logger;
        _databaseTariff = databaseTariff;
        _databaseInsurer = databaseInsurer;
        _payloadService = payloadService;
    }

    public async Task<Payload<Tariff>> CreateTariff(CreateOrUpdateTariffInput tariffInput)
    {
        try
        {
            // loading data from Database:
            var existingFeatures = await _databaseTariff.GetAllFeatures();
            var existingFeatureIds = existingFeatures.Select(feature => feature.Id);
            var missingFeatures = tariffInput.Features.Where(
                feature => !existingFeatureIds.Contains(feature.Id)
            ).ToList();

            // validate features from input:
            if (missingFeatures.Count != 0)
            {
                _logger.LogError(
                    "Cannot add Tariff '{TariffName}' because the following Feature(s) do not exist: '{FeatureIds}'",
                    tariffInput.Name, string.Join(',', missingFeatures.Select(feature => feature.Id)));
                return _payloadService.CreateError<Tariff>("Mindestens eines der Features fehlt");
            }

            // Add Tariff+Feaures
            var dbTariff = await _databaseTariff.InsertTariff(new DataProviders.Models.Database.Tariff
            {
                Name = tariffInput.Name,
                InsurerId = tariffInput.InsurerId,
                ParentTariff = tariffInput.ParentId,
                Provision = tariffInput.Provision,
                ValidFrom = tariffInput.ValidFrom
            });

            foreach (var feature in tariffInput.Features)
            {
                await _databaseTariff.AddTariffFeature(new DbModels.TariffFeature
                {
                    FeatureId = feature.Id,
                    IsActive = feature.IsActive,
                    TariffId = dbTariff.Id
                });
            }

            _logger.LogInformation("Created new tariff '{TariffName}' with '{FeatureCount}' features", dbTariff.Id,
                tariffInput.Features.Count);
            return _payloadService.CreateSuccess(dbTariff.MapToTariff());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed creating new Tariff with name '{TariffName}'", tariffInput.Name);
            return _payloadService.CreateError<Tariff>("Konnte Tarif nicht erstellen");
        }
    }

    public async Task<Payload<Tariff>> GetSingleTariffById(int id)
    {
        try
        {
            var dbTariff = await _databaseTariff.GetTariffById(id);
            return dbTariff == null
                ? _payloadService.CreateError<Tariff>("Tarif nicht vorhanden")
                : _payloadService.CreateSuccess(dbTariff.MapToTariff());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cannot get single Tariff with id '{Id}'", id);
            return _payloadService.CreateError<Tariff>("Tarif konnte nicht ausgelesen werden");
        }
    }

    public async Task<Payload<List<Tariff>>> GetAllTariffs()
    {
        try
        {
            return _payloadService.CreateSuccess((await _databaseTariff.GetAllTariffs())
                .Select(tar => tar.MapToTariff())
                .ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to retrieve tariffs");
            return _payloadService.CreateError<List<Tariff>>("Konnte Tarife nicht ermitteln");
        }
    }

    public async Task<Payload<Tariff>> UpdateSingleTariff(int tariffId, CreateOrUpdateTariffInput tariff)
    {
        try
        {
            var dbTariff = await _databaseTariff.UpdateTariff(new DataProviders.Models.Database.Tariff
            {
                Id = tariffId,
                InsurerId = tariff.InsurerId,
                Name = tariff.Name,
                ParentTariff = tariff.ParentId,
                Provision = tariff.Provision,
                ValidFrom = tariff.ValidFrom
            });

            return _payloadService.CreateSuccess(dbTariff.MapToTariff());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to update tariff with id '{TariffId}'", tariffId);
            return _payloadService.CreateError<Tariff>("Konnte Tarif nicht speichern");
        }
    }

    public async Task<Payload<List<TariffCalculation>>> CalculateAllTariffs(CalculateTariffsInput filter)
    {
        // Todo: Annahme: Bausteintarife sind immer genau einem Tarif zugeordnet. Ein Bausteintarif kann nur 
        // Todo: an einem Grundtarif hängen, Bausteintarife können aber keine weiteren untergeordneten
        // Todo: Bausteintarife besitzen (keine Rekursion)

        IEnumerable<DbModels.Tariff> dbTariffs;
        IEnumerable<DbModels.Feature> dbFeatures;
        IEnumerable<DbModels.TariffFeature> dbTariffFeatures;

        IEnumerable<Insurer> insurers;

        var calculations = new List<TariffCalculation>();
        // Getting Data from Database:
        try
        {
            dbTariffs = await _databaseTariff.GetAllTariffs();
            dbFeatures = await _databaseTariff.GetAllFeatures();
            dbTariffFeatures = await _databaseTariff.GetAllTariffFeatures();
            var allDbInsurers = await _databaseInsurer.SelectAllInsurers();
            insurers = allDbInsurers.Select(dbi => dbi.MapToInsurer());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed retrieving data from database");
            return _payloadService.CreateError<List<TariffCalculation>>("Datenbankfehler");
        }

        try
        {
            // Retrieve only Tariffs that have a ValidFrom<Today:
            var activeTariffs = dbTariffs.Where(dbt => dbt.ValidFrom <= DateTime.Today)
                .Select(dbt => dbt.MapToTariff(dbTariffFeatures, dbFeatures.ToList()))
                .ToList();


            // Adding base Tariffs without Modules:
            calculations.AddRange(activeTariffs.Where(t => t.Parent == null).Select(q =>
                CalculateForTariff(q, insurers.Single(ins => ins.Id == q.Insurer).Name)));

            // Adding moduleTariffs:
            foreach (var moduleTariff in activeTariffs.Where(t => t.Parent != null))
            {
                var parent = calculations.Single(p => p.TariffId == moduleTariff.Parent);
                var moduleCalculation = CalculateForTariff(moduleTariff, parent.InsurerName, parent);
                calculations.Add(moduleCalculation);
            }

            // TODO: Erst alle Elemente abzuholen und dann zu filtern ist natürlich sehr inperformant. Bei einem
            // todo: echten Projekt mit echten DB-Kosten würde ich natürlich vorab filtern

            // filter by insurer if requested:
            if (filter.InsurerId != null)
            {
                calculations = calculations.Where(calc => calc.InsurerId == filter.InsurerId).ToList();
            }

            // filter by feature if requested:
            if (filter.RequiredFeatureIds.Count != 0)
            {
                calculations = calculations.Where(calculation =>
                        filter.RequiredFeatureIds.TrueForAll(
                            feat => calculation.Features.Exists(cf => cf.IsActive && cf.Id == feat)))
                    .ToList();
            }

            return _payloadService.CreateSuccess(calculations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in calculation");
            return _payloadService.CreateError<List<TariffCalculation>>("Fehler in der Kalkulation");
        }
    }

    private static TariffCalculation CalculateForTariff(Tariff tariff, string insurerName,
        TariffCalculation? parent = null)
    {
        var calculation = new TariffCalculation
        {
            InsurerId = tariff.Insurer,
            InsurerName = insurerName,
            TariffId = parent?.TariffId ?? tariff.Id,
            TariffName = parent?.TariffName ?? tariff.Name,
            BaseProvision = parent?.BaseProvision ?? tariff.Provision,
            ModuleProvision = tariff.Provision,
            TotalProvision = (parent?.TotalProvision ?? 0) + tariff.Provision,
            TariffModuleId = parent == null ? null : tariff.Id,
            TariffModuleName = parent == null ? null : tariff.Name
        };


        if (parent == null)
        {
            calculation.Features = tariff.Features.Clone();
        }
        else
        {
            calculation.Features = parent.Features.Clone();

            // Add additional features that are not already in parent:
            foreach (var moduleFeature in tariff.Features.Where(moduleFeature =>
                         calculation.Features.All(feature => feature.Id != moduleFeature.Id)))
            {
                calculation.Features.Add(moduleFeature);
            }

            // Enable features that have been disabled in parent but are enabled in module:
            foreach (var activeModuleFeature in tariff.ActiveFeatures)
            {
                calculation.Features.First(feature => feature.Id == activeModuleFeature.Id).IsActive = true;
            }
        }

        return calculation;
    }
}