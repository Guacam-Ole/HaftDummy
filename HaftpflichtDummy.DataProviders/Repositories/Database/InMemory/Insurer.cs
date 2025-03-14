namespace HaftpflichtDummy.DataProviders.Repositories.Database;

public class Insurer:IInsuranceDatabase, IInsurer
{
    public Task<IEnumerable<Insurer>> GetAllInsurers()
    {
        throw new NotImplementedException();
    }

    public Task<Insurer> GetInsurerByName()
    {
        throw new NotImplementedException();
    }

    public Task<int> AddInsurer(Insurer insurer)
    {
        throw new NotImplementedException();
    }
}