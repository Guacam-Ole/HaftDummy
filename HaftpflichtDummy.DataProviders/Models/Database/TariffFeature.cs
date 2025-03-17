namespace HaftpflichtDummy.DataProviders.Models.Database;
// Todo: Annahme dass in der DB die N:N verknüpfung durch eine Verknüpfungstabelle erfolgt
public class TariffFeature
{
    public int TariffId { get; set; }
    public int FeatureId { get; set; }
    public bool IsActive { get; set; }
}