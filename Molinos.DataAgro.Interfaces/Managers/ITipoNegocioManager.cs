
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
    public interface ITipoNegocioManager
    {
        void Inicializar(MSContext oContexto);

        void Inicializar(MSContext oContexto, IUnitOfWorkAsync oUnitOfWork);

        Task<ResultIniTipoNegocio> TraerTodoAsync();

        Task<TipoNegocio> TraerTipoNegociodAsync(int TipoNegocioId);

        Task<EntityErrors> GrabarTipoNegocioAsync(TipoNegocio oTipoNegocio);

        Task<EntityErrors> EliminarTipoNegocioAsync(int TipoNegocioId);
    }
}


