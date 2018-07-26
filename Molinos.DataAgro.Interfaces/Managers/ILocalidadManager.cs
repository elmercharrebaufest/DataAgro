
using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Interfaces
{
    public interface ILocalidadManager
    {
        Task<DatosIniAbmLocalidad> TraerDatosInicialesAsync();

        Task<ResultIniLocalidad> TraerFiltroLocalidadAsync(ParamAbmLocalidad oParam);

        Task<ResultIniLocalidad> TraerLocalidadPorProvincia(int ProvinciaId);

        Task<Localidad> TraerLocalidadAsync(int intLocalidadId);

        Task<EntityErrors> GrabarLocalidadAsync(Localidad oLocalidad);

        Task<EntityErrors> EliminarLocalidadAsync(int intLocalidadId);

        List<Localidad> ListarLocalidad(string localidad);
    }
}


