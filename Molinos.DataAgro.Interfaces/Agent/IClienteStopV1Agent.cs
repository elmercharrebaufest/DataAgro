using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
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
        Resultado EliminarCupo(Cupo cupo);
        void ModificarCupo(Cupo cupo);
        List<RespuestaCupoNoPropioStop> ConsultarMisTurnosActivos();
    }
}