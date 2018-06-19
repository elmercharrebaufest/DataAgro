
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
    public interface IMaterialManager
    {
        void Inicializar(MSContext oContexto);

        void Inicializar(MSContext oContexto, IUnitOfWorkAsync oUnitOfWork);

        Task<ResultIniMaterial> TraerFiltroMaterialAsync(ParamAbmMaterial oParam);

        Task<Material> TraerMaterialAsync(int intMaterialId);

        Task<EntityErrors> GrabarMaterialAsync(Material oMaterial);

        Task<EntityErrors> EliminarMaterialAsync(int intMaterialId);
    }
}


