using Swashbuckle.AspNetCore.Annotations;

namespace HaftpflichtDummy.Models.InputModels;

public class CalculateTariffsInput
{
    [SwaggerSchema("Zu filternder Versicherer, NULL um alle auszuwählen")]
    public int? Insurer { get; set; }
    
    [SwaggerSchema("Notwendige Features. Leere Liste um alle auszuwählen")]
    public List<int> RequiredFeatures { get; set; }
}