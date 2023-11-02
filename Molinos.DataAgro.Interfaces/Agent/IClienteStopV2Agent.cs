using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Interfaces
{
    public interface IClienteStopV2Agent
    {
        //TokenStop ObtenerToken(string clave);
        void CrearCupo(List<string> listaCupos);
        void TransmitirJobCupos();
        //int ConsultarCupo(string cupo, int terminalId, string token);

        List<RespuestaCupoStop> ConsultarCuposDiarios();
        Resultado EliminarCupo(Cupo cupo, TokenStop tokenNuevo = null, RepositorioEF repo = null);
        //List<Cupo> ObtenerCuposPorFecha(DateTime fechaDelCupo);
        //ConsultaCuposStop ObtenerDatosDeStop(Configuracion datosConfiguracion, HttpClient client, List<DateTime> fechas);
        void ModificarCupo(Cupo cupo);
        List<RespuestaCupoNoPropioStop> ConsultarMisTurnosActivos();
        //ConsultaTurnosActivosStop ObtenerMisTurnosActivosDeStop(Configuracion datosConfiguracion, HttpClient client, DateTime fechaDesde, DateTime fechaHasta);
    }
}