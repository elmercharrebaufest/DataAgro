
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
    public interface ICondicionManager
    {
        void Inicializar(MSContext oContexto);

        void Inicializar(MSContext oContexto, IUnitOfWorkAsync oUnitOfWork);

        Task<ResultIniCondicion> TraerTodoCondicionAsync();

        Task<Condicion> TraerCondicionAsync(int intCondicionId);

        Task<EntityErrors> GrabarCondicionAsync(Condicion oCondicion);

        Task<EntityErrors> EliminarCondicionAsync(int intCondicionId);
    }
}


