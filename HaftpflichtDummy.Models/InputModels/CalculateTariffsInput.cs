using Swashbuckle.AspNetCore.Annotations;

namespace HaftpflichtDummy.Models.InputModels;

[SwaggerSchema("Filter zur Tarifkalkulation")]
public class CalculateTariffsInput
{
    [SwaggerSchema("Zu filternder Versicherer, NULL um alle auszuwählen")]
    public int? Insurer { get; set; }
    
    [SwaggerSchema("Notwendige Features. Leere Liste um alle auszuwählen")]
    public required List<int> RequiredFeatures { get; set; }
}