using Xunit;

namespace HaftpflichtDummy.UnitTests;

// Todo: Der einfachheit halber im Unittest-Projekt. Üblicherweise getrennt, (auch) weil Daten verändert werden
public class IntegrationTests
{
    private readonly HttpClient _client;
    private const string baseUrl="http://localhost:5274";

    public IntegrationTests()
    {
        _client = new HttpClient();
    }

    [Fact]
    public async Task GetInsurersShouldReturnAListOfInsurers()
    {
        _client.GetAsync("/api/gesellschaften");
    }
}