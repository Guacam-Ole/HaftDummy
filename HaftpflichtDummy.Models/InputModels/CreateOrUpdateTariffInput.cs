namespace HaftpflichtDummy.Models.InputModels;

public class CreateOrUpdateTariffInput
{
    public required string Name { get; set; }
    public int Insurer { get; set; }
    public int? Parent { get; set; }
    public decimal Provision { get; set; }
    public DateTime ValidFrom { get; set; }
    public List<Feature> Features { get; } = [];    
}