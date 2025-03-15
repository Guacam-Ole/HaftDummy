namespace HaftpflichtDummy.DataProviders.Models.Database;

public class Tariff:BaseTable
{
    public int Insurer { get; set; }
    
    // Todo: Null bei Grundtarif, bei Bausteintarif zugehöriger Grundtarif
    public int? ParentTariff { get; set; }

    public DateTime ValidFrom { get; set; }
    public decimal Provision { get; set; }
}