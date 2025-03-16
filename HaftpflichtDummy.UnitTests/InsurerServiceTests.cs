using System.Data;
using HaftpflichtDummy.BusinessLogic.Services.WebApiServices;
using HaftpflichtDummy.DataProviders.Repositories.Database.Interfaces;
using HaftpflichtDummy.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using DbModels = HaftpflichtDummy.DataProviders.Models.Database;
using Xunit;
using Assert = Xunit.Assert;

public class InsurerServiceTests
{
    private readonly ILogger<InsurerService> _insurerLogger;
    private readonly IInsurer _dbInsurer;
    private readonly InsurerService _insurerService;

    public InsurerServiceTests()
    {
        _insurerLogger = Substitute.For<ILogger<InsurerService>>();
        _dbInsurer = Substitute.For<IInsurer>();
        _insurerService = new InsurerService(_insurerLogger, _dbInsurer, Substitute.For<PayloadService>());
    }

    private void MockDataBase()
    {
        var insurers = JsonConvert.DeserializeObject<List<DbModels.Insurer>>(File.ReadAllText("json/MockInsurers.json"));
        _dbInsurer.GetAllInsurers().Returns(insurers!);
    }


    [Fact]
    public async Task CreateInsurerCallsDatabase()
    {
        var insurersToAdd = new List<Insurer>
        {
            new()
            {
                Name = "Stans Ship Insurances"
            },
            new()
            {
                Name = "Umbrella Corp Human Insurances"
            }
        };

        foreach (var insurer in insurersToAdd)
        {
            await _insurerService.CreateInsurer(insurer);
        }


        await _dbInsurer.Received()
            .AddInsurer(Arg.Is<DbModels.Insurer>(ins => ins.Name.Contains("Stan")));

        await _dbInsurer.Received()
            .AddInsurer(Arg.Is<DbModels.Insurer>(ins => ins.Name.Contains("Umbrella")));
    }

    [Fact]
    public async Task CatchAndLogCreationErrorFromDatabase()
    {
        var insurerToAdd = new Insurer
        {
            Name = "Stans Ship Insurances"
        };

        await _insurerService.CreateInsurer(insurerToAdd);
        await _dbInsurer.Received()
            .AddInsurer(Arg.Is<DbModels.Insurer>(ins => ins.Name.Contains("Stan")));

        _dbInsurer.AddInsurer(Arg.Any<DbModels.Insurer>()).Throws(new DuplicateNameException());

        // Make sure an error has been logged
        _insurerLogger.ReceivedWithAnyArgs(1).LogError("");

        // Check if the exception from datalayer is thrown from business logic
        await Assert.ThrowsAsync<DuplicateNameException>(() => _insurerService.CreateInsurer(insurerToAdd));
    }

    [Fact]
    public async Task InsurersGetCorrectlyMappedAndReturned()
    {
        MockDataBase();
        var allInsurers = (await _insurerService.GetAll()).ResponseObject!.ToList();

        var firstMatch = allInsurers.FirstOrDefault(q => q.Name.Contains("Stan") && q.Id == 1);
        var secondMatch = allInsurers.FirstOrDefault(q => q.Name.Contains("Umbrella") && q.Id == 2);

        Assert.Equal(2, allInsurers.Count);
        Assert.NotNull(firstMatch);
        Assert.NotNull(secondMatch);
    }
}