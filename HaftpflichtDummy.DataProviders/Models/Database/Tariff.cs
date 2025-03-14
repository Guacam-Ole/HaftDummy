namespace HaftpflichtDummy.DataProviders.Models.Database;

public class Tariff:BaseTable
{
    // Todo: Annahme dass Tarife angelegt werden können und erst später einer Gesellschaft zugewiesen werden, daher nullable
    public int? Insurer { get; set; }
    
    // Todo: Null bei Grundtarif, bei Bausteintarif zugehöriger Grundtarif
    public int? ParentTariff { get; set; }

    public DateTime ValidFrom { get; set; }
    public decimal Premium { get; set; }
}