namespace HaftpflichtDummy.DataProviders.Models.Database;
// Todo: Annahme dass in der DB die N:N verknüpfung durch eine Verknüpfungstabelle erfolgt
// Todo: Zusätzliche Annahme, dass auch inaktive Merkmale sichtbar sein müssen. Alternativ wäre
// Todo: "isActive" obsolete und nur Einträge mit aktiven merkmalen befinden sich in dieser Tabelle
public class TariffFeature
{
    public int TariffId { get; set; }
    public int FeatureId { get; set; }
    public bool IsActive { get; set; }
}