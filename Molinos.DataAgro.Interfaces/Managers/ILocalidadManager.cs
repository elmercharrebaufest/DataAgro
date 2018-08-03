using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface ILocalidadManager
    {
        DatosIniAbmLocalidad TraerDatosIniciales();

        ResultIniLocalidad TraerFiltroLocalidad(ParamAbmLocalidad oParam);

        ResultIniLocalidad TraerLocalidadPorProvincia(int ProvinciaId);

        Localidad TraerLocalidad(int intLocalidadId);

        Resultado GrabarLocalidad(Localidad oLocalidad);

        Resultado EliminarLocalidad(int intLocalidadId);

        List<Localidad> ListarLocalidad(string localidad);
    }
}
