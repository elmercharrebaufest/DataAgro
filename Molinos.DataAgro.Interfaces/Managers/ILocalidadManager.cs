
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
    public interface ILocalidadManager
    {
        void Inicializar(MSContext oContexto);

        void Inicializar(MSContext oContexto, IUnitOfWorkAsync oUnitOfWork);

        Task<DatosIniAbmLocalidad> TraerDatosInicialesAsync();

        Task<ResultIniLocalidad> TraerFiltroLocalidadAsync(ParamAbmLocalidad oParam);

        Task<ResultIniLocalidad> TraerLocalidadPorProvincia(int ProvinciaId);

        Task<Localidad> TraerLocalidadAsync(int intLocalidadId);

        Task<EntityErrors> GrabarLocalidadAsync(Localidad oLocalidad);

        Task<EntityErrors> EliminarLocalidadAsync(int intLocalidadId);
    }
}


