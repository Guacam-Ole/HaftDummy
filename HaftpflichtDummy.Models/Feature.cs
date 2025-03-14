namespace HaftpflichtDummy.Models;

public class Feature
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public bool IsEnabled { get; set; }
}