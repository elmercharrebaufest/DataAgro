using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface IClienteStopAgent
    {
        //TokenStop ObtenerToken(string clave);
        void CrearCupo(List<string> listaCupos);
        void TransmitirJobCupos();
        List<RespuestaCupoStop> ConsultarCuposDiarios();
        Resultado EliminarCupo(Cupo cupo, TokenStop tokenNuevo = null, RepositorioEF repo = null);
        void ModificarCupo(Cupo cupo);
        List<RespuestaCupoNoPropioStop> ConsultarMisTurnosActivos();
        TokenStop ObtenerTokenStop(RepositorioEF repo = null);
    }
}