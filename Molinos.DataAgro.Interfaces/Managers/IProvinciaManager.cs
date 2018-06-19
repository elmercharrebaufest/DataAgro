
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
    public interface IProvinciaManager
    {
        void Inicializar(MSContext oContexto);

        void Inicializar(MSContext oContexto, IUnitOfWorkAsync oUnitOfWork);

        Task<ResultIniProvincia> TraerTodoProvinciaAsync();

        Task<Provincia> TraerProvinciaAsync(int intProvinciaId);

        Task<EntityErrors> GrabarProvinciaAsync(Provincia oProvincia);

        Task<EntityErrors> EliminarProvinciaAsync(int intProvinciaId);
    }
}


