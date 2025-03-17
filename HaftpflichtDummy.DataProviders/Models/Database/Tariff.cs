namespace HaftpflichtDummy.DataProviders.Models.Database;

public class Tariff:BaseTable
{
    public int Insurer { get; set; }
    
    /// <summary>
    /// Basistarif des Bausteintarifs, Null wenn Basistarif
    /// </summary>
    public int? ParentTariff { get; set; }

    public DateTime ValidFrom { get; set; }
    public decimal Provision { get; set; }
}