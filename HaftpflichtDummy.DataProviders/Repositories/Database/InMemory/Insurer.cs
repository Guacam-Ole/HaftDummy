using HaftpflichtDummy.DataProviders.Repositories.Database.Interfaces;
using Microsoft.Extensions.Logging;

namespace HaftpflichtDummy.DataProviders.Repositories.Database;

public class Insurer : IInsurer
{
    private readonly ILogger<Insurer> _logger;
    private readonly FakeDb _fakeDb;

    public Insurer(ILogger<Insurer> logger, FakeDb fakeDb)
    {
        _logger = logger;
        _fakeDb = fakeDb;
    }

    public async Task<IEnumerable<Models.Database.Insurer>> SelectAllInsurers()
    {
        return await _fakeDb.ListItems<Models.Database.Insurer>();
    }

    public async Task<Models.Database.Insurer?> SelectInsurerByName(string insurer)
    {
        var allInsurers = await SelectAllInsurers();
        return allInsurers.SingleOrDefault(q => q.Name == insurer);
    }

    public async Task<int> InsertInsurer(Models.Database.Insurer insurer)
    {
        var existingInsurers = await SelectAllInsurers();
        if (existingInsurers.Any(q => q.Name == insurer.Name))
        {
            _logger.LogError("Tried to insert a duplicate insurer with Name '{Name}'", insurer.Name);
            throw new System.Data.DuplicateNameException("An insurer with this name already exists");
        }

        insurer.Id = FakeDb.GetNextId<Models.Database.Insurer>();
        await _fakeDb.InsertItem(insurer);
        return insurer.Id;
    }
}