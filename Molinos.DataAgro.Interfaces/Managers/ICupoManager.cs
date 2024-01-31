using Kendo.DynamicLinq;
using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Interfaces
{
    public interface ICupoManager
    {
        CupoResult GrabarCupo(Cupo cupo, List<DiaCupo> dias, bool validarDisponibilidad = true);
        DataSourceResult TraerCuposTabla(DataSourceRequest request, List<int> equipo);
        List<DateTime> FechasComprendidas(int? materialId);
        Resultado EliminarCupo(int id, string comerciall, bool enviarMail, TokenStop token = null);
        //Task ObtenerToken();
        Resultado Validar(Cupo cupo, int cantidadCupos, DateTime? fechaHasta);
        void TransmitirCupos();
        Resultado TransmitirCupos(List<string> cupos);
        List<RespuestaCupoStop> ConsultarCuposDiarios();
        CupoDto ObtenerCupo(int id, RepositorioEF repo);
        Resultado EliminarVarios(List<int> cupos, string comercial);
        string ObtenerCodigoSap(int id);
        List<SugerenciaCupoDto> CrearSugerenciaCupo(int MaterialId, FormulaDto formula, ConfiguracionCupo configuracion);
        FormulaDto ObtenerFormulaDto(int material);
        List<SugerenciaCupoDto> ObtenerSugerenciaCupo(int ComercialId, int? materialId);
        IList<SugerenciaCupoDto> ObtenerSugerenciaCupoAgrupadasPorProveedor(int comercialId, int materialId, string centroId);
        List<SugerenciaPorComercialDto> ObtenerSugerenciaPorComercialFecha(int comercialId, int materialId, string centroId);
        SugerenciaCupoDto ObtenerSugerencia(int sugerenciaId);
        List<CupoResult> AceptarSugerenciaCupo(List<SugerenciaCupoDto> ids);
        CupoResult RechazarSugerenciaCupo(List<int> ids, string motivo);
        List<EstadoCupoDto> TraerTodoLosEstados();
        List<DiaCupo> Panel();
        List<SugerenciaNoAceptada> SugerenciasNoAceptadas();
        void EnviarMailSinCtg();
        List<DisponibilidadCuposDto> TraerDisponibilidadCupo(DateTime? fechaDesde, DateTime? fechaHasta, List<string> centroId, string materialId);
        Resultado ActualizarCupoSAP(Cupo cupo);
        CupoResult RechazarCupo(Cupo cupo, string idActiveDirectory);
        CupoResult AceptarCupo(Cupo cupo);
        List<CupoDto> ListarCupo(string cupoSap);
        List<BasicoContrato> TraerNegocioConCupoDisponible(string proveedorCuit, int material, int centro, string filtro, DateTime desde, DateTime hasta);
        Resultado AltaCupoSAP(Cupo cupoSAP);
        string Td(ref int linea, int largo = 1);
        string Split(string str);
        SugerenciaPorComercial ObtenerSugerenciaPorComercial(DateTime fecha, int comercialId, int materialId, string centro);
        List<SugerenciaCupo> SugerenciasParaAceptar(int proveedorId, int comercialId, string centro, int materialId, DateTime? fecha);
        CupoResult ConfirmarSugerencia(List<ConfirmacionSugerenciaCupoDto> datosTablaPorProveedor, List<DiaCupo> devoluciones, int materialId, string centroId, int comercialId);

        List<ConfiguracionCupoDto> TraerTodaConfiguracionCupoPorDia(int zonaId, int materialId, int centroId, DateTime fecha);
        List<EstablecimientoStockDto> TraerEstablecimientos(string proveedor, bool esEPA);
        void AnulacionMasiva(List<int> equipo, string comercialId, List<int> ids, string path);
        void AnulacionMasiva2(List<int> equipo, string comercialId, List<int> ids, string path);
        List<CierreCupera> DevolverTodoCierreCupera();
        void CrearSugerenciaCupo();
        void EnviarMailSugerenciasPendientesPorComercial();
        CupoResult GenerarSolicitudExtraordinaria(AdministracionCupoDto solicitud);
        void EliminarSugerenciaDeCupos(ConfiguracionCupo configuracion);
        List<MensajeCupoDto> MostrarDetalle(int comercialSeleccionado, string centroId, int materialId);
        List<CupoResult> AceptarSugerenciaCupo(int idSugerencia, int cantidad, int cantidadFlete, bool poProveedor = false);
        List<CupoResult> AceptarSugerenciaCupoPorProveedor(int proveedorId, int materialId, DateTime fecha, int cupoNormalSolicitud, int fleteSolicitud, int comercialId, string centroCodigo);
        List<CupoResult> ModificarSugerenciaCupo(List<AceptarSugerenciaCupoDto> items);
        List<CupoResult> ModificarSugerenciaCupoPorProveedor(List<AceptarSugerenciaCupoDto> items);
        List<CupoDto> ObtenerCupos(List<int> list, RepositorioEF repositorio);
        void ActualizarCumplimientoCupos(DateTime date);
        SugerenciaCupo ClonarSugerencia(SugerenciaCupo s);
        //void EjecutarAlgoritmoManual(int materialId, FormulaDto formula, ConfiguracionCupo configuracion = null, string path = "");
        List<RespuestaCupoNoPropioStop> ConsultarMisTurnosActivos();
        CupoResult ValidarDisponibilidadCupera(int materialId, int centroId, DateTime fechaSugerida, int cantidad);
        List<CupoResult> DevolverSugerenciasMasivo(List<DevolucionSugerenciaCupoDto> sugerenciasADevolver);
        List<DisponibilidadCuposDto> TraerCupoDisponibilidadDescarga(DateTime? fechaDesde, DateTime? fechaHasta, string zonaId, List<string> centroId, string materialId);
        CupoResult ValidarDisponibilidadCuperaConDescarga(int materialId, int centroId, DateTime fechaSugerida, int cantidad);
        void VerificarSolicitudesExtraordinariasPendientes(DateTime fecha);
    }
}