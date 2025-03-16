using System.Net.Http.Headers;
using HaftpflichtDummy.Models;
using HaftpflichtDummy.Models.InputModels;
using Newtonsoft.Json;
using NuGet.Frameworks;
using Xunit;
using Assert = Xunit.Assert;

namespace HaftpflichtDummy.IntegrationTests;

public class TestApi
{
    private readonly HttpClient _client;
    private const string baseUrl = "http://localhost:5274";

    public TestApi()
    {
        _client = new HttpClient { BaseAddress = new Uri(baseUrl) };
    }

    private async Task FillDb()
    {
        await _client.GetAsync("/api/test/fill");
    }

    private async Task EmptyDb()
    {
        await _client.GetAsync("/api/test/empty");
    }

    [Fact]
    private async Task AllTariffsShouldBeReturned()
    {
        await FillDb();
        var getTariffsResponse = await _client.GetAsync("/api/tarife");

        var content = await ConvertResponse<List<Tariff>>(getTariffsResponse);

        Assert.Equal(4, content.ResponseObject.Count);
        Assert.Equal(2, content.ResponseObject.Count(q => q.Name.Contains("Schiffsversicherung")));
    }

    [Fact]
    private async Task AddingTariffShouldGetReturnedOnNextSelectCall()
    {
        await FillDb();
        var request = new HttpRequestMessage(new HttpMethod("POST"), "/api/tarife");
        request.Content = new StringContent(JsonConvert.SerializeObject(new CreateOrUpdateTariffInput
        {
            Insurer = 1,
            Name = "Neuer Tarif",
            Provision = (decimal)47.11,
            ValidFrom = DateTime.Now.AddDays(-1)
        }));
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        var insertTariffResponse = await _client.SendAsync(request);

        var getTariffsResponse = await _client.GetAsync("/api/tarife");
        var getContent = await ConvertResponse<List<Tariff>>(getTariffsResponse);

        Assert.Equal(5, getContent.ResponseObject.Count);
        Assert.NotNull(getContent.ResponseObject.FirstOrDefault(q => q.Name == "Neuer Tarif"));
    }

    [Fact]
    private async Task CompleteRun()
    {
        await EmptyDb();
        await AddInsurer("Gesellschaft 1");

        await AddTariff(new CreateOrUpdateTariffInput
        {
            Features = { new TariffInputFeature { Id = 1, IsEnabled = false } },
            Insurer = 1,
            Name = "BasisTariff erste Gesellschaft",
            Provision = 22.4m,
            ValidFrom = DateTime.Today.AddDays(-1)
        }); // 1

        await AddTariff(new CreateOrUpdateTariffInput
        {
            Features = { new TariffInputFeature { Id = 1, IsEnabled = true } },
            Insurer = 1,
            Name = "Modultarif vom BasisTarif der ersten Gesellschaft",
            Provision = 12.0m,
            ValidFrom = DateTime.Today.AddDays(-1),
            Parent = 1
        }); // 2
        await AddInsurer("Gesellschaft 2");
        await AddTariff(new CreateOrUpdateTariffInput
        {
            Features = { new TariffInputFeature { Id = 1, IsEnabled = true } },
            Insurer = 2,
            Name = "BasisTariff zweite Gesellschaft",
            Provision = 120.4m,
            ValidFrom = DateTime.Today.AddDays(-1)
        }); // 3

        await AddTariff(new CreateOrUpdateTariffInput
        {
            Features = { new TariffInputFeature { Id = 2, IsEnabled = true } },
            Insurer = 2,
            Name = "Modultarif vom BasisTarif der zweiten Gesellschaft",
            Provision = 0.10m,
            ValidFrom = DateTime.Today.AddDays(-1),
            Parent = 3
        });//4

        var firstCalculation = await GetCalculations(1); // Sollte den ersten Bausteintarif und beide Tarife des zweiten Anbieters zurückliefern
        var secondCalculation = await GetCalculations(2); // Sollte nur den zweiten Bausteintarif zurückliefern

        Assert.Equal(3, firstCalculation.Count);
        Assert.NotNull(firstCalculation.FirstOrDefault(q => q.TariffId == 1 && q.TariffModuleId==2 && q.TotalProvision == 34.4m));
        Assert.NotNull(firstCalculation.FirstOrDefault(q => q.TariffId == 3 && q.TariffModuleId==null && q.TotalProvision == 120.4m));
        Assert.NotNull(firstCalculation.FirstOrDefault(q => q.TariffId == 3 && q.TariffModuleId==4 && q.TotalProvision == 120.5m));
        
        Assert.Equal(1, secondCalculation.Count);
        Assert.NotNull(secondCalculation.FirstOrDefault(q => q.TariffId == 3 && q.TariffModuleId==4 && q.TotalProvision == 120.5m));
    }


    private async Task AddTariff(CreateOrUpdateTariffInput tariff)
    {
        var request = new HttpRequestMessage(new HttpMethod("POST"), "/api/tarife");
        request.Content = new StringContent(JsonConvert.SerializeObject(tariff));
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        await _client.SendAsync(request);
    }


    private async Task AddInsurer(string name)
    {
        var request = new HttpRequestMessage(new HttpMethod("POST"), "/api/gesellschaften");
        request.Content = new StringContent(JsonConvert.SerializeObject(new CreateOrUpdateInsurerInput
        {
            Name = name
        }));
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        await _client.SendAsync(request);
    }

    private async Task<List<TariffCalculation>> GetCalculations(int requiredFeature)
    {
        var request = new HttpRequestMessage(new HttpMethod("POST"), "/api/tarife/berechnen");
        request.Content = new StringContent(JsonConvert.SerializeObject(new CalculateTariffsInput
        {
            RequiredFeatures = [requiredFeature]
        }));
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        var response=await _client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        var payload = JsonConvert.DeserializeObject<Payload<List<TariffCalculation>>>(content);
        return payload.ResponseObject;
    }

    private async Task<Payload<T>> ConvertResponse<T>(HttpResponseMessage message)
    {
        return JsonConvert.DeserializeObject<Payload<T>>(await message.Content.ReadAsStringAsync())!;
    }
}