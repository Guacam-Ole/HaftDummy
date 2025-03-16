using HaftpflichtDummy.Models;
using HaftpflichtDummy.Models.InputModels;

namespace HaftpflichtDummy.BusinessLogic.Services.WebApiServices.Interfaces;

public interface ITariffService
{
    Task<Payload<Tariff>> CreateTariff(CreateOrUpdateTariffInput tariff);
    Task<Payload<Tariff>> GetSingleTariffById(int id);
    Task<Payload<List<Tariff>>> GetAllTariffs();
    Task<Payload<Tariff>> UpdateSingleTariff(int tariffId, CreateOrUpdateTariffInput tariffInput);
    Task<Payload<List<TariffCalculation>>> CalculateAllTariffs(CalculateTariffsInput filter);
}