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
    public interface ICubProveedoresManager
    {
        void Inicializar(MSContext oContexto);

        void Inicializar(MSContext oContexto, IUnitOfWorkAsync oUnitOfWork);

        Task<DatosIniCubProveedores> TraerDatosInicialesAsync(string ActiveDirectory);

        ParamCubProveedores TraerParam();

        EntityErrors Validar(ParamCubProveedores oParam);

        Task<ResultCubProveedores> TraerDatosAsync(ParamCubProveedores oParam);

        void ActualizarProveedoresCubo(int? ComercialId);

    }
}


