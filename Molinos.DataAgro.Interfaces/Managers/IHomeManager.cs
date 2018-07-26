using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Interfaces
{
    public interface IHomeManager
    {
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
