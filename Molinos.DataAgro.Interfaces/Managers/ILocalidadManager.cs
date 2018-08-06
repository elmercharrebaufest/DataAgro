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

        LocalidadDto TraerLocalidad(int intLocalidadId);

        Resultado GrabarLocalidad(Localidad oLocalidad);

        Resultado EliminarLocalidad(int intLocalidadId);

        List<LocalidadDto> ListarLocalidad(string localidad);
    }
}
