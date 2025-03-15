namespace HaftpflichtDummy.Models;

public class TariffCalculation
{
    public int InsurerId { get; set; }
    public required string InsurerName { get; set; }
    public int TariffId { get; set; }
    public required string TariffName { get; set; }
    public int? TariffModuleId { get; set; }
    public string? TariffModuleName { get; set; }
    public List<string> Features { get; set; } = [];
    public decimal BasePremium { get; set; }
    public decimal ModulePremium { get; set; }
    public decimal TotalPremium { get; set; }
}