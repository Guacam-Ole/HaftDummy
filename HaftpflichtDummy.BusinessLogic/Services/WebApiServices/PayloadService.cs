using System.Runtime.CompilerServices;
using HaftpflichtDummy.Models;
using Microsoft.Extensions.Logging;

namespace HaftpflichtDummy.BusinessLogic.Services.WebApiServices;

public class PayloadService
{
    private readonly ILogger<PayloadService> _logger;

    public PayloadService(ILogger<PayloadService> logger)
    {
        _logger = logger;
    }

    public Payload<T> CreateSuccess<T>(T responseObject, [CallerMemberName] string callerMemberName = "")
    {
        _logger.LogDebug("'{method}' returned success", callerMemberName);
        return new Payload<T>
        {
            Success = true,
            ErrorMessage = null,
            ResponseObject = responseObject
        };
    }

    public Payload<T> CreateError<T>(string errorMessage, [CallerMemberName] string callerMemberName = "")
    {
        _logger.LogError("'{method}' failed with error:'{errorMessage}'", callerMemberName, errorMessage);
        return new Payload<T>
        {
            Success = false,
            ResponseObject = default,
            ErrorMessage = errorMessage
        };
    }
}