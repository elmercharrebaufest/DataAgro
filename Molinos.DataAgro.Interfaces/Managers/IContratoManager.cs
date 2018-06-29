using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Mastersoft.Framework.DataRepository;
using Mastersoft.Framework.Interfaces;
using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Interfaces
{
    public interface IContratoManager
    {
        void Inicializar(MSContext oContexto);

        void Inicializar(MSContext oContexto, IUnitOfWorkAsync oUnitOfWork);

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
