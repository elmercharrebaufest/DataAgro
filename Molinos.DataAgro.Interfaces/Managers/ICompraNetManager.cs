using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{

    public interface ICompraNetManager
    {
        DatosIniCompraNet TraerDatosIniciales(List<int> equipo);
        GrabarSuscripcionResult GrabarSuscripcion(string key, int comercialId);
        bool UsuarioSuscripto(int comercialId);
    }
}