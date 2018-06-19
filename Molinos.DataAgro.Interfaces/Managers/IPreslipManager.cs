
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
    public interface IPreslipManager
    {
        void Inicializar(MSContext oContexto);

        void Inicializar(MSContext oContexto, IUnitOfWorkAsync oUnitOfWork);

        Task<DatosIniAbmPreslip> TraerDatosInicialesAsync();

        Task<ResultIniPreslip> TraerTodoPreslipAsync();

        Task<Preslip> TraerPreslipAsync(int intPreslipId);

        Task<EntityErrors> GrabarPreslipAsync(Preslip oPreslip);

        Task<EntityErrors> EliminarPreslipAsync(int intPreslipId);

        Preslip NuevoPreslip();
    }
}


