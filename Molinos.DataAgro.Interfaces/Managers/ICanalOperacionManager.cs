
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
    public interface ICanalOperacionManager
    {
        void Inicializar(MSContext oContexto);

        void Inicializar(MSContext oContexto, IUnitOfWorkAsync oUnitOfWork);
        
        Task<ResultIniCanalOperacion> TraerTodoCanalOperacionAsync();

        Task<CanalOperacion> TraerCanalOperacionAsync(int intCanalOperacionId);

        Task<EntityErrors> GrabarCanalOperacionAsync(CanalOperacion oCanalOperacion);

        Task<EntityErrors> EliminarCanalOperacionAsync(int intCanalOperacionId);
    }
}


