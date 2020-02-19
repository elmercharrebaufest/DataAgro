using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface ICubProveedoresManager
    {
        DatosIniCubProveedores TraerDatosIniciales(List<int> equipo);

        ParamCubProveedores TraerParam();

        ResultCubProveedores TraerDatos(ParamCubProveedores oParam);
    }
}


