
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
    public interface IMonedaManager
    {
        void Inicializar(MSContext oContexto);

        void Inicializar(MSContext oContexto, IUnitOfWorkAsync oUnitOfWork);

        Task<ResultIniMoneda> TraerTodoAsync();

        Task<Moneda> TraerMonedadAsync(string MonedaId);

        Task<EntityErrors> GrabarMonedaAsync(Moneda oMoneda);

        Task<EntityErrors> EliminarMonedaAsync(string MonedaId);
    }
}


