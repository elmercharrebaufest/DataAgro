using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface IObjetivoManager
    {
        ObjetivoHome TraerObjetivoHome(int idComercial, List<int> equipo);
        Resultado GuardarObjetivo(ObjetivoComercial objetivo);
        Resultado EliminarObjetivo(int id);
    }
}
