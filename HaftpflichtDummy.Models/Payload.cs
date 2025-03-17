namespace HaftpflichtDummy.Models;

// TODO: Wrappen des Results in einen Payload damit das Frontend immer die gleiche Struktur erwarten kann
// todo: Und jederzeit schnell und einfach das Objekt und ggf. Fehler an einer zentralen Stelle abfragen kann
public class Payload<T>
{
    public bool Success { get; set; }
    public T? ResponseObject { get; set; }

    // todo: In einem realworld-Beispiel würde anstatt eines strings ein FehlerObjekt mit Details stehen
    // todo: also z.B. ErrorEnum+ErrorMessage+HumanReadableErrorMessage+AffectedField+Exception+HttpStatusCode etc.
    public string? ErrorMessage { get; set; }
}