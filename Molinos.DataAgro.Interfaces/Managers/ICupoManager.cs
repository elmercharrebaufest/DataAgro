using Kendo.DynamicLinq;
using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Interfaces
{
    public interface ICupoManager
    {
        CupoResult GrabarCupo(Cupo cupo, List<DiaCupo> dias);
        DataSourceResult TraerCuposTabla(DataSourceRequest request, List<int> equipo);
        List<DateTime> FechasComprendidas();
        Resultado EliminarCupo(int id, string comercial);
        //Task ObtenerToken();
        Resultado Validar(Cupo cupo, int cantidadCupos, DateTime? fechaHasta);
        void TransmitirCupos();
        Resultado TransmitirCupos(List<string> cupos);
        List<RespuestaCupoStop> ConsultarCuposDiarios();
        CupoDto ObtenerCupo(int id);
        Resultado EliminarVarios(List<int> cupos, string comercial);
        string ObtenerCodigoSap(int id);
        void CrearSugerenciaCupo();
        List<SugerenciaCupoDto> ObtenerSugerenciaCupo(int ComercialId);
        IList<SugerenciaCupoDto> ObtenerSugerenciaCupoAgrupadasPorProveedor(int comercialId, int materialId, string centroId);
        SugerenciaCupoDto ObtenerSugerencia(int sugerenciaId);
        List<CupoResult> AceptarSugerenciaCupo(List<SugerenciaCupoDto> ids);
        CupoResult AceptarCupoExcedente(int administracionId);
        CupoResult RechazarSugerenciaCupo(List<int> ids,string motivo);
        List<EstadoCupoDto> TraerTodoLosEstados();
        CupoResult ConfirmarSugerencia(List<ConfirmacionSugerenciaCupoDto> datosTabla, int materialId, string centroId);
        List<DiaCupo> Panel();
        List<SugerenciaNoAceptada> SugerenciasNoAceptadas();
        void EnviarMailSinCtg();
        List<DisponibilidadCuposDto> TraerCupoDisponibilidad(DateTime? fechaDesde, DateTime? fechaHasta, string zonaId, List<string> centroId, string materialId);
        Resultado ActualizarCupoSAP(Cupo cupo);
        CupoResult RechazarCupo(Cupo cupo, string idActiveDirectory);
        CupoResult AceptarCupo(Cupo cupo);
        List<CupoDto> ListarCupo(string cupoSap);

        List<BasicoContrato> TraerNegocioConCupoDisponible(string proveedorCuit, int material, int centro, string filtro, DateTime desde, DateTime hasta);
        Resultado AltaCupoSAP(Cupo cupoSAP);
    }

}
