using HaftpflichtDummy.DataProviders.Models.Database;
using HaftpflichtDummy.DataProviders.Repositories.Database.Interfaces;
using Microsoft.Extensions.Logging;

namespace HaftpflichtDummy.DataProviders.Repositories.Database;

public class Tariff : ITariff
{
    private readonly ILogger<Tariff> _logger;
    private readonly FakeDb _fakeDb;

    public Tariff(ILogger<Tariff> logger, FakeDb fakeDb)
    {
        _logger = logger;
        _fakeDb = fakeDb;
    }

    public async Task<IEnumerable<Models.Database.TariffFeature>> GetAllTariffFeatures()
    {
        return await _fakeDb.ListItems<TariffFeature>();
    }

    public async Task<IEnumerable<Models.Database.Feature>> GetAllFeatures()
    {
        return await _fakeDb.ListItems<Feature>();
    }

    public async Task<IEnumerable<Models.Database.Tariff>> GetAllTariffs()
    {
        return await _fakeDb.ListItems<Models.Database.Tariff>();
    }

    public async Task<IEnumerable<Models.Database.Tariff?>> GetAllTariffsByInsurerId(int insurerId)
    {
        var allTariffs = await GetAllTariffs();
        return allTariffs.Where(t => t.Insurer == insurerId);
    }

    public async Task<Models.Database.Tariff?> GetTariffById(int tariffId)
    {
        var allTariffs = await GetAllTariffs();
        return allTariffs.SingleOrDefault(t => t.Id == tariffId);
    }

    public async Task<Models.Database.Tariff> UpdateTariff(Models.Database.Tariff tariff)
    {
        return await _fakeDb.UpdateItem(tariff.Id, tariff);
    }

    public async Task<Models.Database.Tariff> InsertTariff(Models.Database.Tariff tariff)
    {
        var existingTariffs = await GetAllTariffs();
        if (existingTariffs.Any(q => q.Name == tariff.Name && q.Insurer == tariff.Insurer))
        {
            _logger.LogError("Tried to insert a duplicate tariff with Name '{TariffName}' to '{InsurerId}'",
                tariff.Name, tariff.Insurer);
            throw new System.Data.DuplicateNameException("A tariff with this name already exists for that insurer");
        }

        tariff.Id = FakeDb.GetNextId<Models.Database.Tariff>();
        await _fakeDb.InsertItem(tariff);
        return tariff;
    }

    public async Task<TariffFeature> AddTariffFeature(TariffFeature tariffFeature)
    {
        return await _fakeDb.InsertItem(tariffFeature);
    }

    public async Task RemoveTariffFeature(TariffFeature tariffFeature)
    {
        await _fakeDb.RemoveItem(tariffFeature);
    }

    public async Task<Feature> AddFeature(Feature feature)
    {
        feature.Id = FakeDb.GetNextId<Feature>();
        await _fakeDb.InsertItem(feature);
        return feature;
    }

    public async Task RemoveFeature(Feature feature)
    {
        await _fakeDb.RemoveItem(feature);
    }
}