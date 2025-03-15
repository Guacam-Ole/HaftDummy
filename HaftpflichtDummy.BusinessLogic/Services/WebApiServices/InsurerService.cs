using HaftpflichtDummy.BusinessLogic.Mapper;
using HaftpflichtDummy.BusinessLogic.Services.WebApiServices.Interfaces;
using HaftpflichtDummy.DataProviders.Repositories.Database.Interfaces;
using HaftpflichtDummy.Models;
using Microsoft.Extensions.Logging;

namespace HaftpflichtDummy.BusinessLogic.Services.WebApiServices;

public class InsurerService : IInsurerService
{
    private readonly ILogger<InsurerService> _logger;
    private readonly IInsurer _databaseInsurer;

    public InsurerService(ILogger<InsurerService> logger, IInsurer databaseInsurer)
    {
        _logger = logger;
        _databaseInsurer = databaseInsurer;
    }

    public async Task CreateInsurer(Insurer insurer)
    {
        try
        {
            await _databaseInsurer.AddInsurer(new DataProviders.Models.Database.Insurer
            {
                Name = insurer.Name
            });
            _logger.LogDebug("New Insurer '{InsurerName}' created in Database", insurer.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create insurer '{InsurerName}'", insurer.Name);
            throw;
        }
    }

    public async Task<IEnumerable<Insurer>> GetAll()
    {
        var insurers = await _databaseInsurer.GetAllInsurers();
        return insurers.Select(q => q.MapToInsurer());
    }
}