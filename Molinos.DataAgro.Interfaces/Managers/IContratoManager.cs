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
    public interface IContratoManager
    {
        DataSourceResult TraerTodosContratos(DataSourceRequest request, bool corredor, List<int> listComercialesId, List<int> corredoresComercial);
        DataSourceResult TraerContratosFiltrados(DataSourceRequest filtro, List<int> equipo);

        DatosIniContrato TraerDatosCombo(int? tipoNegocioId = null);

        GrabarContratoResult GrabarContrato(Contrato oContrato);

        GrabarContratoResult ConfirmarContrato(int contratoId, int comercialId);

        GrabarContratoResult BorrarContrato(Contrato oContrato);

        GrabarContratoResult FinalizarContrato(int contratoId, string idActiveDirectory);

        GrabarContratoResult GrabarAmpliacionContrato(Contrato oContrato);

        List<DescuentoBonificacionDto> TraerDescuentosPorContrato(int contratoId);
        List<CalidadDto> TraerCalidadesPorContrato(int contratoId, int acuerdoId);
        DatosContratoDto TraerDatosDeContrato(int contratoId);
        BasicoContrato TraerContrato(int contratoId);

        void EnviarMailPendiente();

        void FinalizacionAutomatica(string idActiveDirectory);
        void BorradoAutomatico();
        List<AvisoContratoDto> TraerContratosPendientes(List<int> equipo);
        DatosCompraNetDto TraerDatosCompraNet(int id);
        ContratoResult TraerContratoMadre(string sap);
        GrabarContratoResult AnularContrato(Contrato oContrato, string idActiveDirectory);
        List<ContratoCopiar> TraerContratosPorSap(string nrocontratoSap);
        List<ContratoCopiar> TraerContratosAcuerdo(string filtro);
        BasicoContrato TraerContratoAcuerdoACopiar(int contratoId);
        List<AperturaPrecioDto> TraerAperturaDePrecioPorContrato(int contratoId);
        TotalPesosDolares TraerTotalesPesosDolares(DataSourceRequest request, List<int> listComercialesId, List<int> corredoresComercial);
        List<EstadoContratoDto> TraerTodoLosEstados();
        List<BoletoCompraNetDto> TraerTodosLosBoletos();
        List<GrupoDeComprasDto> TraerTodoGrupoDeCompras();
        AltaTempranaNRCODto ValidarProveedor(int proveedorId);
        Resultado ActualizarContratoSAP(Contrato contratoSAP, bool validacionesMinimas);
        GrabarContratoResult ActualizarContratoFinalizado(Contrato contrato);
        List<ContratoIdDto> TraerContratosSAP(string desde, string hasta);
        RangoPrecioDto ObtenerRangoDePrecios(int materialId, string monedaId);
        EstadoSAPDto ValidarStatus(int contratoId);
        GrabarContratoResult PreAnularContrato(int contratoId, string motivo);
        GrabarContratoResult RechazarPreAnularContrato(int contratoId/*, string motivoRechazo*/);
        GrabarContratoResult ReconfirmarFinalizado(int contratoId, string mailComercial);
        GrabarContratoResult AnularContratoPreAnulado(int contratoId, string idActiveDirectory);
        List<BasicoContrato> CompararNegocioReconfirmado(int contratoId);
        bool DiferenciaEnCalidades(int contratoId);
        Resultado AnularContratoSAP(ContratoSAP contrato);
        DataSourceResult TraerContratosReporteAFijarPase(DataSourceRequest filtro, List<int> equipo);
        DatosContratoDto TraerDatosDeContratoAcuerdo(int contratoId);
        List<PagoCBUDto> ListarCBU(string cuitProveedor, string filtro);
        string ObtenerSapContrato(int contrato);
        string ObtenerSapFijacion(int contrato);
        Resultado AltaContratoSAP(Contrato contratoSap, bool validacionesMinimas);
        Resultado AprobarContrato(int id);
        GrabarContratoResult BorrarContratoPreAprobacion(int id, string motivo);

        BasicoContrato NegocioABasicoContrato(Negocio negocio);

        TipoNegocio DevolverNamespaceNegocio(int tipo);
        Resultado AnularContratoCarga(int contratoId, string motivoRechazo);
        List<CcPpPerndienteAplicarDto> ListarCartasDePortePendienteAplicar(CcPpPerndienteAplicarDto req);
        string ValidarCredito(string cuit, double cantidad, decimal precio, string moneda);
        List<ContratoCopiar> TraerContratosAcuerdoPorCorredor(int corredorId);
        List<GrabarContratoResult> GrabarContratoMasivo(List<BasicoContrato> contratos);
        List<CapacidadProductivaPendienteDto> ObtenerCapacidadProductivaPendiente(int proveedorId);
        Resultado ActualizarCesionContratoSAP(string contratoSAP, bool cesion);
        Resultado ValidacionesAnulaYReemplaza(string contratoSap);
        List<NegocioAsociadoDto> DevolverContratosParaAsociar(int contratoId, string numero);
        List<NegocioAsociadoDto> DevolverContratoAsociadosPase(int id);
        decimal CalcularImporteDeOperacion(decimal precio, double cantidad, int materialId, DateTime fechaoperacion, string moneda);
        Resultado GrabarNegociosAsociados(List<NegocioAsociadoDto> negocios, int contratoId, decimal precioPonderado);
        bool TieneAsociados(int contratoId);

        bool EsUnContratoAsociado(int negocioId);


        bool EstaConfirmadoEnSAP(string contratoSAP, int TipoNegocioId);
    }
}
