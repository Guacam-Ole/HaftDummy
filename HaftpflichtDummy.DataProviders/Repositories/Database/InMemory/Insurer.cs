namespace HaftpflichtDummy.DataProviders.Repositories.Database;

public class Insurer: IInsurer
{
    private readonly FakeDb _fakeDb;

    public Insurer(FakeDb fakeDb)
    {
        _fakeDb = fakeDb;
    }
    
    public async Task<IEnumerable<Models.Database.Insurer>> GetAllInsurers()
    {
        return await _fakeDb.ListItems<Models.Database.Insurer>();
    }

    public async Task<Models.Database.Insurer?> GetInsurerByName(string insurer)
    {
        var allInsurers = await GetAllInsurers();
        return allInsurers.SingleOrDefault(q => q.Name == insurer);
    }

    public async Task AddInsurer(Models.Database.Insurer insurer)
    {
        var existingInsurers = await GetAllInsurers();
        if (existingInsurers.Any(q => q.Name == insurer.Name))
            throw new System.Data.DuplicateNameException("An insurer with this name already exists");
        await _fakeDb.InsertItem(insurer);
    }
}