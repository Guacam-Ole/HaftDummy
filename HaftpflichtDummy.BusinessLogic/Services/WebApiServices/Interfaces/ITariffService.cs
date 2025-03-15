using HaftpflichtDummy.Models;

namespace HaftpflichtDummy.BusinessLogic.Services.WebApiServices.Interfaces;

public interface ITariffService
{
    Task<Tariff> CreateTariff(Tariff tariff);
    Task<Tariff?> GetSingleTariffById(int id);
    Task<Tariff> UpdateSingleTariff(int tariffId, Tariff tariff);
    Task<IEnumerable<TariffCalculation>>  CalculateAllTariffs();

}