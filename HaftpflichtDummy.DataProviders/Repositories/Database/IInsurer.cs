
namespace HaftpflichtDummy.DataProviders.Repositories.Database;

public interface IInsurer
{
    // TODO: Aufrufe sind nicht wirklich async in der Fake-DB. Da aber der echte DB-Zugriff async sein wird ist das Interface entsprechend
    
    /// <summary>
    /// Liefert alle verfügbaren Gesellschaften
    /// </summary>
    /// <returns>Liste der Gesellschaften</returns>
    Task<IEnumerable<Models.Database.Insurer>> GetAllInsurers();

    /// <summary>
    /// Liefert eine einzelne Gesellschaft anhand des Namens
    /// </summary>
    /// <returns>Gesellschaft</returns>
    Task<Models.Database.Insurer?> GetInsurerByName(string insurer);

    /// <summary>
    /// Eine Gesellschaft zur Datenbank hinzufügen
    /// </summary>
    /// <param name="insurer">Gesellschaft</param>
    /// <exception cref="System.Data.DuplicateNameException">Wird geworfen wenn eine Gesellschaft mit dem Namen bereits existiert</exception>
    Task AddInsurer(Models.Database.Insurer insurer);

}

  
