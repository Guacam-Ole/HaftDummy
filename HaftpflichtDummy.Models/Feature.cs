using Swashbuckle.AspNetCore.Annotations;

namespace HaftpflichtDummy.Models;

[SwaggerSchema("Leistungsmerkmal")]
public class Feature
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public bool IsActive { get; set; }
}