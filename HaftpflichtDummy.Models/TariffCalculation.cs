namespace HaftpflichtDummy.Models;

public class TariffCalculation
{
    public int InsurerId { get; set; }
    public required string InsurerName { get; set; }
    public int TariffId { get; set; }
    public required string TariffName { get; set; }
    public int? TariffModuleId { get; set; }
    public string? TariffModuleName { get; set; }
    public List<Feature> Features { get; set; } = [];
    public decimal BaseProvision { get; set; }
    public decimal ModuleProvision { get; set; }
    public decimal TotalProvision { get; set; }
}