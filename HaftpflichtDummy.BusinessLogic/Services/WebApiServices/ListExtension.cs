using Newtonsoft.Json;

namespace HaftpflichtDummy.BusinessLogic.Services.WebApiServices;

public static class ListExtension
{
    public static List<T> Clone<T>(this List<T> listToClone)
    {
        // Quick & Dirty für dieses Beispiel (anstatt ICloneable)
        return JsonConvert.DeserializeObject<List<T>>(JsonConvert.SerializeObject(listToClone))!;
    }
}