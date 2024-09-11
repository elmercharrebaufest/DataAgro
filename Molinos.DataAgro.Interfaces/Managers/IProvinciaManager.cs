using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface IProvinciaManager
    {
        ResultIniProvincia TraerTodoProvincia();

        ProvinciaDto ObtenerProvincia(int intProvinciaId);

        Resultado GrabarProvincia(Provincia oProvincia);

        Resultado EliminarProvincia(int intProvinciaId);

        List<ProvinciaDto> ListarProvincia(string provincia);
    }
}