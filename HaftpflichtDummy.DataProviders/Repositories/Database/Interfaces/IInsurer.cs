namespace HaftpflichtDummy.DataProviders.Repositories.Database.Interfaces;

public interface IInsurer
{
    // TODO: Aufrufe sind nicht wirklich async in der Fake-DB. Da aber der echte DB-Zugriff async sein wird ist das Interface entsprechend

    /// <summary>
    /// Liefert alle verfügbaren Gesellschaften
    /// </summary>
    /// <returns>Liste der Gesellschaften</returns>
    Task<IEnumerable<Models.Database.Insurer>> SelectAllInsurers();

    /// <summary>
    /// Liefert eine einzelne Gesellschaft anhand des Namens
    /// </summary>
    /// <returns>Gesellschaft</returns>
    Task<Models.Database.Insurer?> SelectInsurerByName(string insurer);

    /// <summary>
    /// Eine Gesellschaft zur Datenbank hinzufügen
    /// </summary>
    /// <param name="insurer">Gesellschaft</param>
    /// <exception cref="System.Data.DuplicateNameException">Wird geworfen wenn eine Gesellschaft mit dem Namen bereits existiert</exception>
    Task<int> InsertInsurer(Models.Database.Insurer insurer);
}