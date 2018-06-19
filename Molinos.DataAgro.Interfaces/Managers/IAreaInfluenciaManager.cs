
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
    public interface IAreaInfluenciaManager
    {
        void Inicializar(MSContext oContexto);

        void Inicializar(MSContext oContexto, IUnitOfWorkAsync oUnitOfWork);
        
        Task<ResultIniAreaInfluencia> TraerTodoAreaInfluenciaAsync();

        Task<AreaInfluencia> TraerAreaInfluenciaAsync(int intAreaInfluenciaId);

        Task<EntityErrors> GrabarAreaInfluenciaAsync(AreaInfluencia oAreaInfluencia);

        Task<EntityErrors> EliminarAreaInfluenciaAsync(int intAreaInfluenciaId);
    }
}


