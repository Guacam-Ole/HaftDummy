using HaftpflichtDummy.DataProviders.Models.Database;

namespace HaftpflichtDummy.DataProviders.Repositories.Database.Interfaces;

public interface ITariff
{
    /// <summary>
    /// Liefert alle verfügbaren Tarife
    /// </summary>
    /// <returns>Liste aller Tarife</returns>
    Task<IEnumerable<Models.Database.Tariff>> GetAllTariffs();

    /// <summary>
    /// Liefert alle vorhandenen Features 
    /// </summary>
    /// <returns>Liste aller Features</returns>
    Task<IEnumerable<Models.Database.Feature>> GetAllFeatures();

    /// <summary>
    /// Liefert alle Feature-Tarif-Verknüpfungen
    /// </summary>
    /// <returns>Liste aller Verknüpfungen</returns>
    Task<IEnumerable<Models.Database.TariffFeature>> GetAllTariffFeatures();

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
    /// <returns>Id from created Tariff</returns>
    Task<int> InsertTariff(Models.Database.Tariff tariff);

    /// <summary>
    /// TarifFeature-Verbindung hinzufügen
    /// </summary>
    /// <param name="tariffFeature">TarifFeature</param>
    /// <returns></returns>
    Task AddTariffFeature(TariffFeature tariffFeature);

    /// <summary>
    /// TarifFeature-Verbindung lösen
    /// </summary>
    /// <param name="tariffFeature">TarifFeature</param>
    /// <returns></returns>
    Task RemoveTariffFeature(TariffFeature tariffFeature);

    /// <summary>
    /// Feature erstellen
    /// </summary>
    /// <param name="feature">Feature</param>
    /// <returns>Id des Features</returns>
    Task<int> AddFeature(Feature feature);

    /// <summary>
    /// Feature entfernen
    /// </summary>
    /// <param name="feature">Feature</param>
    /// <returns></returns>
    Task RemoveFeature(Feature feature);
}