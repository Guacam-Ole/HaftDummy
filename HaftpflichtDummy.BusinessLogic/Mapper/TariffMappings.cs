using HaftpflichtDummy.Models;

namespace HaftpflichtDummy.BusinessLogic.Mapper;

using DbModels = HaftpflichtDummy.DataProviders.Models.Database;

// TODO: Alternativ kann man natürlich auch AutoMapper verwenden, ich verwende aber tatsächlich lieber
// TODO: den manuellen Ansatz weil er besser lesbar und debugbar ist (solange es nicht einfach 20x die gleiche Property rechts/links ist)
public static class TariffMappings
{
    public static Tariff MapToTariff(this DbModels.Tariff tariff)
        => new()
        {
            Name = tariff.Name,
            Id = tariff.Id,
            Insurer = tariff.InsurerId,
            Parent = tariff.ParentTariff,
            Provision = tariff.Provision,
            ValidFrom = tariff.ValidFrom
        };

    public static Feature MapToFeature(this DbModels.Feature feature, bool isEnabled)
        =>
            new()
            {
                Id = feature.Id,
                Name = feature.Name,
                IsActive = isEnabled
            };
}