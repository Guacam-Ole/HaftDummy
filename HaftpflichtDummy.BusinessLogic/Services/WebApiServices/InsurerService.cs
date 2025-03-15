using HaftpflichtDummy.BusinessLogic.Mapper;
using HaftpflichtDummy.BusinessLogic.Services.WebApiServices.Interfaces;
using HaftpflichtDummy.DataProviders.Repositories.Database.Interfaces;
using HaftpflichtDummy.Models;

namespace HaftpflichtDummy.BusinessLogic.Services.WebApiServices;

public class InsurerService:IInsurerService
{
    private readonly IInsurer _databaseInsurer;

    public InsurerService(IInsurer databaseInsurer)
    {
        _databaseInsurer = databaseInsurer;
    }
    
    public async Task CreateInsurer(Insurer insurer)
    {
        await _databaseInsurer.AddInsurer(new DataProviders.Models.Database.Insurer
        {
            Name = insurer.Name
        });
    }

    public async Task<IEnumerable<Insurer>> GetAll()
    {
        var insurers= await _databaseInsurer.GetAllInsurers();
        return insurers.Select(q => q.MapToInsurer());
    }
}