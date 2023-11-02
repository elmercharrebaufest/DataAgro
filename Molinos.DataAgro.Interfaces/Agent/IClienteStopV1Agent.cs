using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Interfaces
{
    public interface IClienteStopV1Agent
    {
        void CrearCupo(List<string> listaCupos);
        void TransmitirJobCupos();
        List<RespuestaCupoStop> ConsultarCuposDiarios();
        Resultado EliminarCupo(Cupo cupo, TokenStop tokenNuevo = null, RepositorioEF repo = null);
        void ModificarCupo(Cupo cupo);
        List<RespuestaCupoNoPropioStop> ConsultarMisTurnosActivos();
    }
}