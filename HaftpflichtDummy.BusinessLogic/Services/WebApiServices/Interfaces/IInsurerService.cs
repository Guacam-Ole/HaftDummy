using System.Diagnostics;
using HaftpflichtDummy.Models;

namespace HaftpflichtDummy.BusinessLogic.Services.WebApiServices.Interfaces;

public interface IInsurerService
{
    Task CreateInsurer(Insurer insurer);
    Task<IEnumerable<Insurer>> GetAll();
    

}