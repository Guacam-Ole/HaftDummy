using HaftpflichtDummy.DataProviders.Models.Database;

namespace HaftpflichtDummy.DataProviders.Repositories.Database;

public interface IInsuranceDatabase
{
}

internal interface IInsurer
{
    /// <summary>
    /// Liefert alle verfügbaren Gesellschaften
    /// </summary>
    /// <returns>Liste der Gesellschaften</returns>
    Task<IEnumerable<Insurer>> GetAllInsurers();

    /// <summary>
    /// Liefert eine einzelne Gesellschaft anhand des Namens
    /// </summary>
    /// <returns>Gesellschaft</returns>
    Task<Insurer> GetInsurerByName();

    /// <summary>
    /// Eine Gesellschaft zur Datenbank hinzufügen
    /// </summary>
    /// <param name="insurer">Gesellschaft</param>
    /// <exception cref="System.Data.DuplicateNameException">Wird geworfen wenn eine Gesellschaft mit dem Namen bereits existiert</exception>
    /// <returns>Id der erstellten Gesellschaft</returns>
    Task<int> AddInsurer(Insurer insurer);

}

internal interface ITariff
{
    /// <summary>
    /// Liefert alle verfügbaren Tarife
    /// </summary>
    /// <returns>Liste aller Tarife</returns>
    Task<IEnumerable<Models.Database.Tariff>> GetAllTariffs();

    // TODO: War nicht Teil der Aufgabe, erscheint mir aber ein erwartbarer UseCase
    /// <summary>
    /// Liefert alle verfügbaren Tarife eines Versicherers
    /// </summary>
    /// <returns>Liste der Tariffe</returns>
    Task<IEnumerable<Models.Database.Tariff>> GetAllTariffsByInsurerId();

    /// <summary>
    /// Liefert einen einzelnen Tarif anhand der id
    /// </summary>
    /// <returns>Tarif</returns>
    Task<Models.Database.Tariff> GetTariffById();

    /// <summary>
    /// Aktualisiert einen einzelnen Tarif
    /// </summary>
    /// <param name="tariff">Tarif</param>
    /// <exception cref="KeyNotFoundException">Wird geworfen wein kein Tarif mit dieser Id existiert</exception>
    /// <returns></returns>
    Task UpdateTariff(Models.Database.Tariff tariff);
}

  
