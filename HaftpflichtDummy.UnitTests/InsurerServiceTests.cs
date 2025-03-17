namespace HaftpflichtDummy.UnitTests;

using System.Data;
using BusinessLogic.Services.WebApiServices;
using DataProviders.Repositories.Database.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using DbModels = HaftpflichtDummy.DataProviders.Models.Database;
using Xunit;
using Assert = Xunit.Assert;

public class InsurerServiceTests
{
    private readonly IInsurer _dbInsurer;
    private readonly InsurerService _insurerService;

    public InsurerServiceTests()
    {
        var insurerLogger = Substitute.For<ILogger<InsurerService>>();
        _dbInsurer = Substitute.For<IInsurer>();
        var payloadService = Substitute.For<PayloadService>(Substitute.For<ILogger<PayloadService>>());
        _insurerService = new InsurerService(insurerLogger, _dbInsurer, payloadService);
    }

    private void MockDataBase()
    {
        var insurers =
            JsonConvert.DeserializeObject<List<DbModels.Insurer>>(File.ReadAllText("json/MockInsurers.json"));
        _dbInsurer.SelectAllInsurers().Returns(insurers!);
    }


    [Fact]
    public async Task CreateInsurerCallsDatabase()
    {
        var insurersToAdd = new List<string>
        {
            "Stans Ship Insurances",
            "Umbrella Corp Human Insurances"
        };

        foreach (var insurer in insurersToAdd)
        {
            await _insurerService.CreateInsurer(insurer);
        }

        await _dbInsurer.Received()
            .InsertInsurer(Arg.Is<DbModels.Insurer>(ins => ins.Name.Contains("Stan")));

        await _dbInsurer.Received()
            .InsertInsurer(Arg.Is<DbModels.Insurer>(ins => ins.Name.Contains("Umbrella")));
    }

    [Fact]
    public async Task DuplicateInsertShouldReturnError()
    {
        var insurerToAdd = "Stans Ship Insurances";

        await _insurerService.CreateInsurer(insurerToAdd);
        await _dbInsurer.Received()
            .InsertInsurer(Arg.Is<DbModels.Insurer>(ins => ins.Name.Contains("Stan")));

        _dbInsurer.InsertInsurer(Arg.Any<DbModels.Insurer>()).Throws(new DuplicateNameException());

        var result = await _insurerService.CreateInsurer(insurerToAdd);

        // Make sure Errorfields are filled properly:
        Assert.False(result.Success);
        Assert.NotNull(result.ErrorMessage);
        Assert.Null(result.ResponseObject);
    }

    [Fact]
    public async Task InsurersGetCorrectlyMappedAndReturned()
    {
        MockDataBase();
        var allInsurers = (await _insurerService.GetAllInsurers()).ResponseObject!.ToList();

        var firstMatch = allInsurers.FirstOrDefault(q => q.Name.Contains("Stan") && q.Id == 1);
        var secondMatch = allInsurers.FirstOrDefault(q => q.Name.Contains("Umbrella") && q.Id == 2);

        Assert.Equal(2, allInsurers.Count);
        Assert.NotNull(firstMatch);
        Assert.NotNull(secondMatch);
    }
}