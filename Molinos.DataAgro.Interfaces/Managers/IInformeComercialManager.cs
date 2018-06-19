using Mastersoft.Framework.DataRepository;
using Mastersoft.Framework.Interfaces;
using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Entities.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Interfaces
{
    public interface IInformeComercialManager
    {
        void Inicializar(MSContext oContexto);

        void Inicializar(MSContext oContexto, IUnitOfWorkAsync oUnitOfWork);

        Task<List<InformeComercialMaterialDisponible>> TraerInformeComercialAsync(int ProveedorId);

        Task<InformeResult> GrabarInformeComercial(ParamInformeComercial informe, int IdActiveDirectory);

        Task<RptInformeComercialInfo> GenerarInformeComercial(ParamInformeComercial informe,int InformeId);

        Task<List<ReportesList>> ListarReportes(ParamReportesIC oParam);

        Task<List<InformeList>> TraerInformesGeneradosAsync() ;

        Task<List<ResultCapacidadProductiva>> TraerCapacidadProductivaAsync(string proveedores);

        Task<int> GrabarCapacidadProductivaAsync(string informes);

        Task<EntityErrors> RespuestaDeSapCapacidadProductivaAsync(string cuit, string Material, string Respuesta);

        Task<ParamInformeComercial> ReimprimirInformeComercial(int InformeId);

        Task<List<InformeGeneradoList>> TraerInformeComercialGeneradoAsync(int ProveedorId);

        Task<EntityErrors> EliminarInformes(int InformeId);

        Task<List<MaterialesModificacionInforme>> TraerInformeMaterialesAsync(int InformeId);
    }
}
