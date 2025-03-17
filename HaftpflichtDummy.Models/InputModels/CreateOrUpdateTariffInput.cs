using Swashbuckle.AspNetCore.Annotations;

namespace HaftpflichtDummy.Models.InputModels;

[SwaggerSchema("Hinzuzfügender Tarif")]
public class CreateOrUpdateTariffInput
{
    [SwaggerSchema("Tarifbezeichnung")]
    public required string Name { get; set; }
    
    [SwaggerSchema("Id des Versicherers")] 
    public int Insurer { get; set; }

    [SwaggerSchema("Zugehöriger Basistarif (bei Modultarif)")]
    public int? Parent { get; set; }

    [SwaggerSchema("Prämie")] 
    
    public decimal Provision { get; set; }
    [SwaggerSchema("Gültigkeitsdatum")]
    public DateTime ValidFrom { get; set; }
    
    [SwaggerSchema("Merkmale des Tarifs")] 
    public List<TariffInputFeature> Features { get; set; } = [];
}

public class TariffInputFeature
{
    public int Id { get; set; }
    public bool IsEnabled { get; set; }
}