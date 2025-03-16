using System.Diagnostics;
using HaftpflichtDummy.Models;

namespace HaftpflichtDummy.BusinessLogic.Services.WebApiServices.Interfaces;

public interface IInsurerService
{
    Task<Payload<Insurer>> CreateInsurer(Insurer insurer);
    Task<Payload<List<Insurer>>> GetAll();
}