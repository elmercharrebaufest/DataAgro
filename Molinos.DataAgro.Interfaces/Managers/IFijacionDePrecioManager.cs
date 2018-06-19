
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
    public interface IFijacionDePrecioManager
    {
        void Inicializar(MSContext oContexto);

        void Inicializar(MSContext oContexto, IUnitOfWorkAsync oUnitOfWork);

        Task<DatosIniAbmFijacionDePrecio> TraerDatosInicialesAsync();

        Task<ResultIniFijacionDePrecio> TraerTodoFijacionDePrecioAsync();

        Task<FijacionDePrecio> TraerFijacionDePrecioAsync(int intFijacionId);

        Task<EntityErrors> GrabarFijacionDePrecioAsync(FijacionDePrecio oFijacionDePrecio, string idActiveDirectory);

        Task<EntityErrors> EliminarFijacionDePrecioAsync(int intFijacionId);

        FijacionDePrecio NuevoFijacionDePrecio();
    }
}


