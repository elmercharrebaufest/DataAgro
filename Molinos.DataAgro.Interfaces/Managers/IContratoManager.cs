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
        KendoGrid<BasicoContrato> TraerTodosContratos(KendoGridMvcRequest request, List<int> listComercialesId);

        DatosIniContrato TraerDatosCombo();
        
        GrabarContratoResult GrabarContrato(Contrato oContrato);

        GrabarContratoResult ConfirmarContrato(Contrato oContrato);

        GrabarContratoResult BorrarContrato(Contrato oContrato);

        GrabarContratoResult FinalizarContrato(Contrato oContrato, string idActiveDirectory);

        GrabarContratoResult GrabarAmpliacionContrato(Contrato oContrato);
        
        List<DescuentoBonificacionDto> TraerDescuentosPorContrato(int contratoId);
        List<CalidadDto> TraerCalidadesPorContrato(int contratoId);
        BasicoContrato TraerContrato(int contratoId);
    }
}
