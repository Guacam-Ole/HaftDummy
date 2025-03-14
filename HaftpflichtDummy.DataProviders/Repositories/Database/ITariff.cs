namespace HaftpflichtDummy.DataProviders.Repositories.Database;

public interface ITariff
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
    /// <param name="insurerId"></param> 
    /// <returns>Liste der Tarife</returns>
    Task<IEnumerable<Models.Database.Tariff?>> GetAllTariffsByInsurerId(int insurerId);

    /// <summary>
    /// Liefert einen einzelnen Tarif anhand der id
    /// </summary>
    /// <param name="id">Tarif-Id</param>
    /// <returns>Tarif</returns>
    Task<Models.Database.Tariff?> GetTariffById(int id);

    /// <summary>
    /// Aktualisiert einen einzelnen Tarif
    /// </summary>
    /// <param name="tariff">Tarif</param>
    /// <exception cref="KeyNotFoundException">Wird geworfen wein kein Tarif mit dieser Id existiert</exception>
    /// <returns></returns>
    Task UpdateTariff(Models.Database.Tariff tariff);

    /// <summary>
    /// Einen neuen Tarif hinzufügen
    /// </summary>
    /// <param name="tariff">Tarif</param>
    /// <returns></returns>
    Task InsertTariff(Models.Database.Tariff tariff);
}