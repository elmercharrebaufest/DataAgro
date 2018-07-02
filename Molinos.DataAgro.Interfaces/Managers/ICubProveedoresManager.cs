using System.Threading.Tasks;

using Mastersoft.Framework.Standard;

using Molinos.DataAgro.Entities;

namespace Molinos.DataAgro.Interfaces
{
    public interface ICubProveedoresManager
    {
        Task<DatosIniCubProveedores> TraerDatosInicialesAsync(string ActiveDirectory);

        ParamCubProveedores TraerParam();

        EntityErrors Validar(ParamCubProveedores oParam);

        Task<ResultCubProveedores> TraerDatosAsync(ParamCubProveedores oParam);

        void ActualizarProveedoresCubo(int? ComercialId);

    }
}


