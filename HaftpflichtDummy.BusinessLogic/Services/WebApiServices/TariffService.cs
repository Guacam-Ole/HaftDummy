using System.Data;
using HaftpflichtDummy.BusinessLogic.Mapper;
using HaftpflichtDummy.BusinessLogic.Services.WebApiServices.Interfaces;
using HaftpflichtDummy.DataProviders.Models.Database;
using HaftpflichtDummy.DataProviders.Repositories.Database.Interfaces;
using HaftpflichtDummy.Models;
using Tariff = HaftpflichtDummy.Models.Tariff;

namespace HaftpflichtDummy.BusinessLogic.Services.WebApiServices;

public class TariffService : ITariffService
{
    private readonly ITariff _databaseTariff;
    private readonly IInsurer _databaseInsurer;

    public TariffService(ITariff databaseTariff, IInsurer databaseInsurer)
    {
        _databaseTariff = databaseTariff;
        _databaseInsurer = databaseInsurer;
    }

    public async Task<Tariff> CreateTariff(Tariff tariff)
    {
        var tariffId = await _databaseTariff.InsertTariff(new DataProviders.Models.Database.Tariff
        {
            Name = tariff.Name,
            Insurer = tariff.Insurer,
            ParentTariff = tariff.Parent,
            Premium = tariff.Premium,
            ValidFrom = tariff.ValidFrom
        });

        tariff.Id = tariffId;
        foreach (var feature in tariff.Features)
        {
            await _databaseTariff.AddTariffFeature(new TariffFeature
            {
                FeatureId = feature.Id,
                IsActive = feature.IsEnabled,
                TariffId = tariffId
            });
        }

        return tariff;
    }

    public async Task<Tariff?> GetSingleTariffById(int id)
    {
        var dbTariff = await _databaseTariff.GetTariffById(id);
        return dbTariff?.MapToTariff();
    }

    public async Task<Tariff> UpdateSingleTariff(int tariffId, Tariff tariff)
    {
        await _databaseTariff.UpdateTariff(new DataProviders.Models.Database.Tariff
        {
            Id = tariffId,
            Insurer = tariff.Insurer,
            Name = tariff.Name,
            ParentTariff = tariff.Parent,
            Premium = tariff.Premium,
            ValidFrom = tariff.ValidFrom
        });

        return tariff;
    }

    public async Task<IEnumerable<TariffCalculation>> CalculateAllTariffs()
    {
        // Todo: Annahme: Bausteintarife sind immer genau einem Tarif zugeordnet. Ein Bausteintarif kann nur 
        // Todo: an einem Grundtarif hängen, Bausteintarife können aber keine weiteren untergeordneten
        // Todo: Bausteintarife besitzen (keine Rekursion)

        var calculations = new List<TariffCalculation>();
        var allDbTariffs = await _databaseTariff.GetAllTariffs();
        var allDbFeatures = await _databaseTariff.GetAllFeatures();
        var allDbTariffFeatures = await _databaseTariff.GetAllTariffFeatures();
        var allDbInsureres = await _databaseInsurer.GetAllInsurers();

        var allTariffs = allDbTariffs.Select(dbt => dbt.MapToTariff(allDbTariffFeatures, allDbFeatures.ToList()))
            .ToList();
        var allInsurers = allDbInsureres.Select(dbi => dbi.MapToInsurer());

        // Adding base Tariffs without Modules:
        calculations.AddRange(allTariffs.Where(t => t.Parent == null).Select(q =>
            new TariffCalculation
            {
                InsurerId = q.Insurer,
                InsurerName = allInsurers.Single(ins => ins.Id == q.Insurer).Name,

                TariffId = q.Id,
                TariffName = q.Name,
                BasePremium = q.Premium,
                TotalPremium = q.Premium,
                Features = q.ActiveFeatures.Select(f => f.Name).ToList()
            }
        ));

        // Adding moduleTariffs:
        foreach (var moduleTariff in allTariffs.Where(t => t.Parent != null))
        {
            var parent = calculations.Single(p => p.TariffId == moduleTariff.Parent);
            var moduleCalculation =
                new TariffCalculation
                {
                    InsurerId = parent.InsurerId,
                    InsurerName = parent.InsurerName,
                    TariffId = parent.TariffId,
                    TariffName = parent.TariffName,
                    BasePremium = parent.BasePremium,
                    ModulePremium = moduleTariff.Premium,
                    TotalPremium = parent.BasePremium + moduleTariff.Premium,
                    Features = parent.Features,
                    TariffModuleId = moduleTariff.Id,
                    TariffModuleName = moduleTariff.Name
                };
            moduleCalculation.Features.AddRange(moduleTariff.Features.Select(f => f.Name));
            calculations.Add(moduleCalculation);
        }

        return calculations;
    }
}