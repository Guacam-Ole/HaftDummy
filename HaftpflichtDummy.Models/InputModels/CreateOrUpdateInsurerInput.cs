using Swashbuckle.AspNetCore.Annotations;

namespace HaftpflichtDummy.Models.InputModels;

[SwaggerSchema("Ein hinzuzufügender Versicherer")]
public class CreateOrUpdateInsurerInput
{
    [SwaggerSchema("Bezeichnung des Versicherers")]
    public required string Name { get; set; }
}