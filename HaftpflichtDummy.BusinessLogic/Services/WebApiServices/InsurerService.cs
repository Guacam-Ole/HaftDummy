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

    public async Task<Payload<Insurer>> CreateInsurer(string insurerName)
    {
        try
        {
            var insurerId = await _databaseInsurer.InsertInsurer(new DataProviders.Models.Database.Insurer
            {
                Name = insurerName
            });
            _logger.LogDebug("New Insurer '{InsurerName}' created in Database", insurerName);
            return _payloadService.CreateSuccess(new Insurer { Id = insurerId, Name = insurerName });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create insurer '{InsurerName}'", insurerName);
            return _payloadService.CreateError<Insurer>("Database Error");
        }
    }

    public async Task<Payload<List<Insurer>>> GetAllInsurers()
    {
        try
        {
            var insurers = (await _databaseInsurer.SelectAllInsurers()).Select(q => q.MapToInsurer());
            return _payloadService.CreateSuccess(insurers.ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get insurers from Database");
            return _payloadService.CreateError<List<Insurer>>("Database Error");
        }
    }
}