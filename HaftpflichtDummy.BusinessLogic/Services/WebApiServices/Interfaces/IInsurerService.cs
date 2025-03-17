using System.Diagnostics;
using HaftpflichtDummy.Models;

namespace HaftpflichtDummy.BusinessLogic.Services.WebApiServices.Interfaces;

public interface IInsurerService
{
    /// <summary>
    /// Eine neue Gesellschaft anlegen
    /// </summary>
    /// <param name="insurerName">Name der Gesellschaft</param>
    /// <returns>Neu erstellte Gesellschaft</returns>
    Task<Payload<Insurer>> CreateInsurer(string insurerName);

    /// <summary>
    /// Alle Gesellschaften ausgeben
    /// </summary>
    /// <returns>Liste der Gesellschaften</returns>
    Task<Payload<List<Insurer>>> GetAllInsurers();
}