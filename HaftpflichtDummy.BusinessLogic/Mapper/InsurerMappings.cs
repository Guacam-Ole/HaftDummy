namespace HaftpflichtDummy.BusinessLogic.Mapper;

using DbModels = HaftpflichtDummy.DataProviders.Models.Database;

public static class InsurerMappings
{
    public static Models.Insurer MapToInsurer(this DbModels.Insurer insurer)
        =>  new()
        {
                Name = insurer.Name,
                Id = insurer.Id
            };
}