using Kendo.DynamicLinq;
using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Interfaces
{
    public interface IContratoManager
    {
        DataSourceResult TraerTodosContratos(DataSourceRequest request, bool corredor, List<int> listComercialesId, List<int> corredoresComercial);
        DataSourceResult TraerContratosFiltrados(DataSourceRequest filtro, List<int> equipo);

        DatosIniContrato TraerDatosCombo();

        GrabarContratoResult GrabarContrato(Contrato oContrato);

        GrabarContratoResult ConfirmarContrato(int contratoId);

        GrabarContratoResult BorrarContrato(Contrato oContrato);

        GrabarContratoResult FinalizarContrato(int contratoId, string idActiveDirectory);

        GrabarContratoResult GrabarAmpliacionContrato(Contrato oContrato);

        List<DescuentoBonificacionDto> TraerDescuentosPorContrato(int contratoId);
        List<CalidadDto> TraerCalidadesPorContrato(int contratoId,int acuerdoId); 
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
        Resultado ActualizarContratoSAP(Contrato contratoSAP);
        GrabarContratoResult ActualizarContratoFinalizado(Contrato contrato);
        List<ContratoIdDto> TraerContratosSAP(string desde, string hasta);
        RangoPrecioDto ObtenerRangoDePrecios(int materialId, string monedaId);
    }
}
