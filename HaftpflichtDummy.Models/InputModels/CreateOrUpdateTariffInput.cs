namespace HaftpflichtDummy.Models.InputModels;

public class CreateOrUpdateTariffInput
{
    public required string Name { get; set; }
    public int Insurer { get; set; }
    public int? Parent { get; set; }
    public decimal Provision { get; set; }
    public DateTime ValidFrom { get; set; }
    public List<TariffInputFeature> Features { get; set; } = [];    
}

public class TariffInputFeature
{
    public int Id { get; set; }
    public bool IsEnabled { get; set; }
}