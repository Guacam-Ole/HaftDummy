namespace HaftpflichtDummy.Models;

public class Tariff
{
    public int Id { get; set; }
    public required string Name { get; set; }

    public List<Feature> Features { get; } = [];
    public List<Feature> ActiveFeatures
    {
        get { return Features.Where(f => f.IsEnabled).ToList(); }
    } 
}