namespace HaftpflichtDummy.DataProviders.Repositories.Database;

public class Tariff : ITariff
{
    private readonly FakeDb _fakeDb;

    public Tariff(FakeDb fakeDb)
    {
        _fakeDb = fakeDb;
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

    public async Task UpdateTariff(Models.Database.Tariff tariff)
    {
        await _fakeDb.UpdateItem(tariff.Id, tariff);
    }

    public async Task InsertTariff(Models.Database.Tariff tariff)
    {
        await _fakeDb.InsertItem(tariff);
    }
}