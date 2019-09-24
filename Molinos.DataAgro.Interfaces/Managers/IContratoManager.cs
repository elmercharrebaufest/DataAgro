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
        KendoGrid<BasicoContrato> TraerTodosContratos(KendoGridMvcRequest request, int perfil, List<int> listComercialesId, List<int> corredoresComercial);
        KendoGridContratoDto TraerContratosFiltrados(FiltroReporteNegocioDto filtro, int perfil, List<int> listComercialesId, List<int> corredoresComercial);

        DatosIniContrato TraerDatosCombo(int perfil);

        GrabarContratoResult GrabarContrato(Contrato oContrato);

        GrabarContratoResult ConfirmarContrato(int contratoId);

        GrabarContratoResult BorrarContrato(Contrato oContrato);

        GrabarContratoResult FinalizarContrato(int contratoId, string idActiveDirectory);

        GrabarContratoResult GrabarAmpliacionContrato(Contrato oContrato);

        List<DescuentoBonificacionDto> TraerDescuentosPorContrato(int contratoId);
        List<CalidadDto> TraerCalidadesPorContrato(int contratoId);
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
        TotalPesosDolares TraerTotalesPesosDolares(KendoGridMvcRequest request, int perfil, List<int> listComercialesId, List<int> corredoresComercial);
        List<EstadoContratoDto> TraerTodoLosEstados();
        List<BoletoCompraNetDto> TraerTodosLosBoletos();
        List<GrupoDeComprasDto> TraerTodoGrupoDeCompras();
            }
}
