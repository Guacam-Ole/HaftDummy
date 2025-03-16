using HaftpflichtDummy.Models;

namespace HaftpflichtDummy.BusinessLogic.Services.WebApiServices.Interfaces;

public interface ITariffService
{
    Task<Payload<Tariff>> CreateTariff(Tariff tariff);
    Task<Payload<Tariff>> GetSingleTariffById(int id);
    Task<Payload<List<Tariff>>> GetAllTariffs();
    Task<Payload<Tariff>> UpdateSingleTariff(int tariffId, Tariff tariff);
    Task<Payload<List<TariffCalculation>>> CalculateAllTariffs(List<int> requiredFeatures);
}