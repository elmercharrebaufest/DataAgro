
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
    public interface IDestinatarioManager
    {
        void Inicializar(MSContext oContexto);

        void Inicializar(MSContext oContexto, IUnitOfWorkAsync oUnitOfWork);
        
        Task<ResultIniDestinatario> TraerTodoDestinatarioAsync();

        Task<Destinatario> TraerDestinatarioAsync(int intDestinatarioId);

        Task<EntityErrors> GrabarDestinatarioAsync(Destinatario oDestinatario);

        Task<EntityErrors> EliminarDestinatarioAsync(int intDestinatarioId);

    }
}


