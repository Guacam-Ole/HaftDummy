using HaftpflichtDummy.DataProviders.Repositories.Database;
using Newtonsoft.Json;

namespace HaftpflichtDummy.Api;

public static class DummyData
{
    public static void LoadDummyData(FakeDb fakeDb)
    {
        // Quick&Dirty dummydaten einfügen

        fakeDb.BulkReplace("Feature",
            JsonConvert.DeserializeObject<List<HaftpflichtDummy.DataProviders.Models.Database.Feature>>(File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
                "json/MockFeatures.json"))));
        fakeDb.BulkReplace("Insurer",
            JsonConvert.DeserializeObject<List<HaftpflichtDummy.DataProviders.Models.Database.Insurer>>(File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
                "json/MockInsurers.json"))));
        fakeDb.BulkReplace("Tariff",
            JsonConvert.DeserializeObject<List<HaftpflichtDummy.DataProviders.Models.Database.Tariff>>(File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
                "json/MockTariffs.json"))));
        fakeDb.BulkReplace("TariffFeature",
            JsonConvert.DeserializeObject<List<HaftpflichtDummy.DataProviders.Models.Database.TariffFeature>>(File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
                "json/MockTariffFeatures.json"))));
    }
}