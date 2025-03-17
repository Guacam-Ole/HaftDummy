using HaftpflichtDummy.Models;
using HaftpflichtDummy.Models.InputModels;

namespace HaftpflichtDummy.BusinessLogic.Services.WebApiServices.Interfaces;

public interface ITariffService
{
    /// <summary>
    /// Tarif erzeugen
    /// </summary>
    /// <param name="tariff">Tarif</param>
    /// <returns>Angelegter Tarif</returns>
    Task<Payload<Tariff>> CreateTariff(CreateOrUpdateTariffInput tariff);

    /// <summary>
    /// Einzelnen Tarif nach Id ausgeben
    /// </summary>
    /// <param name="id">Id des Tarifs</param>
    /// <returns>Tarif</returns>
    Task<Payload<Tariff>> GetSingleTariffById(int id);

    /// <summary>
    /// Alle Tarife ausgeben
    /// </summary>
    /// <returns>Liste der Tarife</returns>
    Task<Payload<List<Tariff>>> GetAllTariffs();

    /// <summary>
    /// Einzelnen Tarif aktualisieren
    /// </summary>
    /// <param name="tariffId">Id des Tarifs</param>
    /// <param name="tariffInput">Tarifinformationen</param>
    /// <returns>Veränderter Tarif</returns>
    Task<Payload<Tariff>> UpdateSingleTariff(int tariffId, CreateOrUpdateTariffInput tariffInput);

    /// <summary>
    /// Tarife berechnen
    /// </summary>
    /// <param name="filter">Filtermerkmale</param>
    /// <returns>Berechnete Tarife</returns>
    Task<Payload<List<TariffCalculation>>> CalculateAllTariffs(CalculateTariffsInput filter);
}