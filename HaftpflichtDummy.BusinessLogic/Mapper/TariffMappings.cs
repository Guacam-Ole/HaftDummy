using HaftpflichtDummy.Models;

namespace HaftpflichtDummy.BusinessLogic.Mapper;

using DbModels = HaftpflichtDummy.DataProviders.Models.Database;

public static class TariffMappings
{
    public static Tariff MapToTariff(this DbModels.Tariff tariff)
        => new()
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

    public static Models.Feature MapToFeature(this DbModels.Feature feature, bool isEnabled)
        =>
            new Feature
            {
                Id = feature.Id,
                Name = feature.Name,
                IsEnabled = isEnabled
            };
}