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
    private readonly PayloadService _payloadService;

    public InsurerService(ILogger<InsurerService> logger, IInsurer databaseInsurer, PayloadService payloadService)
    {
        _logger = logger;
        _databaseInsurer = databaseInsurer;
        _payloadService = payloadService;
    }

    public async Task<Payload<Insurer>> CreateInsurer(Insurer insurer)
    {
        try
        {
            await _databaseInsurer.AddInsurer(new DataProviders.Models.Database.Insurer
            {
                Name = insurer.Name
            });
            _logger.LogDebug("New Insurer '{InsurerName}' created in Database", insurer.Name);
            return _payloadService.CreateSuccess(insurer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create insurer '{InsurerName}'", insurer.Name);
            return  _payloadService.CreateError<Insurer>("Database Error");
        }
    }

    public async Task<Payload<List<Insurer>>> GetAll()
    {
        var insurers = (await _databaseInsurer.GetAllInsurers()).Select(q => q.MapToInsurer());
        return _payloadService.CreateSuccess(insurers.ToList());
    }
}