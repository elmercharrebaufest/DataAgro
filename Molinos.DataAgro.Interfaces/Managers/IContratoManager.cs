using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Interfaces
{
    public interface IContratoManager
    {
        KendoGrid<BasicoContrato> TraerTodosContratos(KendoGridMvcRequest request, List<int> listComercialesId);

        Task<DatosIniContrato> TraerDatosCombo();

        Task<Contrato> TraerContratoAsync(int ContratoId);

        Task<GrabarContratoResult> GrabarContrato(Contrato oContrato);

        Task<GrabarContratoResult> ConfirmarContrato(Contrato oContrato);

        Task<GrabarContratoResult> BorrarContrato(Contrato oContrato);

        Task<GrabarContratoResult> FinalizarContrato(Contrato oContrato, string idActiveDirectory);

        Task<GrabarContratoResult> GrabarAmpliacionContrato(Contrato oContrato);

        int ObtenerComercialId(string idActiveDirectory);
     }
}
