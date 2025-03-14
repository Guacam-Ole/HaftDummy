using HaftpflichtDummy.DataProviders.Models.Database;

namespace HaftpflichtDummy.DataProviders.Repositories.Database;

public class Tariff:IInsuranceDatabase, ITariff
{
    public Task<IEnumerable<Models.Database.Tariff>> GetAllTariffs()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Models.Database.Tariff>> GetAllTariffsByInsurerId()
    {
        throw new NotImplementedException();
    }

    public Task<Models.Database.Tariff> GetTariffById()
    {
        throw new NotImplementedException();
    }

    public Task UpdateTariff(Models.Database.Tariff tariff)
    {
        throw new NotImplementedException();
    }
}