using System.Data;
using HaftpflichtDummy.BusinessLogic.Mapper;
using HaftpflichtDummy.BusinessLogic.Services.WebApiServices.Interfaces;
using HaftpflichtDummy.DataProviders.Models.Database;
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
        var existingFeatures = await _databaseTariff.GetAllFeatures();
        var existingFeatureIds = existingFeatures.Select(feature => feature.Id);
        var missingFeatures = tariffInput.Features.Where(
            feature => !existingFeatureIds.Contains(feature.Id)
        ).ToList();
        if (missingFeatures.Count != 0)
        {
            _logger.LogError(
                "Cannot add Tariff '{TariffName}' because the following Feature(s) do not exist: '{FeatureNames}'",
                tariffInput.Name, string.Join(',', missingFeatures.Select(feature => feature.Name)));
            return _payloadService.CreateError<Tariff>("Mindestens eines der Features fehlt");
        }

        var dbTariff = await _databaseTariff.InsertTariff(new DataProviders.Models.Database.Tariff
        {
            Name = tariffInput.Name,
            Insurer = tariffInput.Insurer,
            ParentTariff = tariffInput.Parent,
            Provision = tariffInput.Provision,
            ValidFrom = tariffInput.ValidFrom
        });

        foreach (var feature in tariffInput.Features)
        {
            await _databaseTariff.AddTariffFeature(new TariffFeature
            {
                FeatureId = feature.Id,
                IsActive = feature.IsEnabled,
                TariffId = dbTariff.Id
            });
        }

        return _payloadService.CreateSuccess(dbTariff.MapToTariff());
    }

    public async Task<Payload<Tariff>> GetSingleTariffById(int id)
    {
        var dbTariff = await _databaseTariff.GetTariffById(id);
        return dbTariff == null
            ? _payloadService.CreateError<Tariff>("Tarif nicht vorhanden")
            : _payloadService.CreateSuccess(dbTariff.MapToTariff());
    }

    public async Task<Payload<List<Tariff>>> GetAllTariffs()
    {
        return _payloadService.CreateSuccess((await _databaseTariff.GetAllTariffs()).Select(tar => tar.MapToTariff())
            .ToList());
    }

    public async Task<Payload<Tariff>> UpdateSingleTariff(int tariffId, CreateOrUpdateTariffInput tariff)
    {
        var dbTariff = await _databaseTariff.UpdateTariff(new DataProviders.Models.Database.Tariff
        {
            Id = tariffId,
            Insurer = tariff.Insurer,
            Name = tariff.Name,
            ParentTariff = tariff.Parent,
            Provision = tariff.Provision,
            ValidFrom = tariff.ValidFrom
        });

        return _payloadService.CreateSuccess(dbTariff.MapToTariff());
    }

    public async Task<Payload<List<TariffCalculation>>> CalculateAllTariffs(CalculateTariffsInput filter)
    {
        // Todo: Annahme: Bausteintarife sind immer genau einem Tarif zugeordnet. Ein Bausteintarif kann nur 
        // Todo: an einem Grundtarif hängen, Bausteintarife können aber keine weiteren untergeordneten
        // Todo: Bausteintarife besitzen (keine Rekursion)

        var calculations = new List<TariffCalculation>();
        var allDbTariffs = await _databaseTariff.GetAllTariffs();
        var allDbFeatures = await _databaseTariff.GetAllFeatures();
        var allDbTariffFeatures = await _databaseTariff.GetAllTariffFeatures();
        var allDbInsurers = await _databaseInsurer.SelectAllInsurers();

        var activeTariffs = allDbTariffs.Where(dbt => dbt.ValidFrom <= DateTime.Today)
            .Select(dbt => dbt.MapToTariff(allDbTariffFeatures, allDbFeatures.ToList()))
            .ToList();
        var allInsurers = allDbInsurers.Select(dbi => dbi.MapToInsurer());

        // Adding base Tariffs without Modules:
        calculations.AddRange(activeTariffs.Where(t => t.Parent == null).Select(q =>
            new TariffCalculation
            {
                InsurerId = q.Insurer,
                InsurerName = allInsurers.Single(ins => ins.Id == q.Insurer).Name,
                TariffId = q.Id,
                TariffName = q.Name,
                BaseProvision = q.Provision,
                TotalProvision = q.Provision,
                Features = q.Features.Clone()
            }
        ));

        // Adding moduleTariffs:
        foreach (var moduleTariff in activeTariffs.Where(t => t.Parent != null))
        {
            var parent = calculations.Single(p => p.TariffId == moduleTariff.Parent);
            var moduleCalculation =
                new TariffCalculation
                {
                    InsurerId = parent.InsurerId,
                    InsurerName = parent.InsurerName,
                    TariffId = parent.TariffId,
                    TariffName = parent.TariffName,
                    BaseProvision = parent.BaseProvision,
                    ModuleProvision = moduleTariff.Provision,
                    TotalProvision = parent.TotalProvision + moduleTariff.Provision,
                    TariffModuleId = moduleTariff.Id,
                    TariffModuleName = moduleTariff.Name,
                    Features = parent.Features.Clone()
                };

            // Add additional Features:
            foreach (var moduleFeature in moduleTariff.Features.Where(moduleFeature =>
                         moduleCalculation.Features.All(feature => feature.Id != moduleFeature.Id)))
            {
                moduleCalculation.Features.Add(moduleFeature);
            }

            // Enable added Features
            foreach (var activeModuleFeature in moduleTariff.ActiveFeatures)
            {
                moduleCalculation.Features.First(feature => feature.Id == activeModuleFeature.Id).IsEnabled = true;
            }

            calculations.Add(moduleCalculation);
        }
        
        // TODO: Erst alle Elemente abzuholen und dann zu filtern ist natürlich sehr inperformant. Bei einem
        // todo: echten Projekt mit echten DB-Kosten würde ich natürlich vorab filtern

        if (filter.Insurer != null)
        {
            calculations = calculations.Where(calc => calc.InsurerId == filter.Insurer).ToList();
        }
        
        if (filter.RequiredFeatures.Count != 0)
        {
            calculations = calculations.Where(calculation =>
                    filter.RequiredFeatures.TrueForAll(
                        feat => calculation.Features.Exists(cf => cf.IsEnabled && cf.Id == feat)))
                .ToList();
        }

        return _payloadService.CreateSuccess(calculations);
    }
}