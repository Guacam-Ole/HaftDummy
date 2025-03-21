namespace HaftpflichtDummy.Models;

public class Tariff
{
    public int Id { get; set; }
    public required string Name { get; set; }

    public int Insurer { get; set; }
    public int? Parent { get; set; }
    public decimal Provision { get; set; }
    public DateTime ValidFrom { get; set; }
}