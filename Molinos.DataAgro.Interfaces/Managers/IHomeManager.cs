using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mastersoft.Framework.Standard;
using Mastersoft.Framework.DataRepository;
using Mastersoft.Framework.Interfaces;

using Molinos.DataAgro.Entities;

namespace Molinos.DataAgro.Interfaces
{
    public interface IHomeManager
    {

        void Inicializar(MSContext oContexto);

        void Inicializar(MSContext oContexto, IUnitOfWorkAsync oUnitOfWork);

        void Inicializar(MSContext oContexto, ICampañaManager oCampaña,IEstadoProveedorManager oEstado);

        Task<ResultIniContacto> TraerTodoContactoAsync(int idComercial);

        Task<ResultIniContacto> TraerBusquedaContactoAsync(oParamBusqueda oParam);

        Task<CampañaHome> TraerInfoCampañaAsync(int idComercial);

        Task<DatosIniciales> TraerInfoInicialesAsync(int comercialId);

        Task<int> TraerIdComercial(string idActiveDirectory);        

        Task<List<BusquedaHome>> BusquedaHome(string filtro, int ComercialId);

        //Task<List<BusquedaHome>> BusquedaHome(string filtro);

        Task<List<ActividadRecordatorio>> TraerActividadesPorComercialId(int ComercialId);

        Task<List<ContactoIni>> ExportarContactos(List<int> Ids, string idActiveDirectory);

        Task<ExportAll> ExportarAll(List<int> Ids, string idActiveDirectory);

        Task<PostIt> TraerTextoAsync(int idComercial);

        Task<GrabarPostItResult> GuardarPostItAsync(PostIt post);
    }
}
