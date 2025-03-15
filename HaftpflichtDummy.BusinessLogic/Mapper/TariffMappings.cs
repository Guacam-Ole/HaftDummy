using HaftpflichtDummy.Models;

namespace HaftpflichtDummy.BusinessLogic.Mapper;

using DbModels = HaftpflichtDummy.DataProviders.Models.Database;

// TODO: Alternativ kann man natürlich auch AutoMapper verwenden, ich verwende aber tatsächlich lieber
// TODO: den manuellen Ansatz weil er besser lesbar und debugbar ist (solange es nicht einfach 20x die gleiche Property rechts/links ist)
public static class TariffMappings
{
    public static Tariff MapToTariff(this DbModels.Tariff tariff)
        => new Tariff
            {
                Name = tariff.Name,
                Id = tariff.Id
            };

    public static Tariff MapToTariff(this DbModels.Tariff tariff,
        IEnumerable<DbModels.TariffFeature> tariffFeatures, List<DbModels.Feature> features)
    {
        var retTariff = MapToTariff(tariff);
        retTariff.Features.AddRange(
            tariffFeatures.Select(tariffFeature =>
                features.First(f => f.Id == tariffFeature.FeatureId)
                    .MapToFeature(tariffFeature.IsActive))
        );


        return retTariff;
    }

    public static Feature MapToFeature(this DbModels.Feature feature, bool isEnabled)
        =>
            new()
            {
                Id = feature.Id,
                Name = feature.Name,
                IsEnabled = isEnabled
            };
}