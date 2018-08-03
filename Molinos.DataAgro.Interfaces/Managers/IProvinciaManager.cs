using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface IProvinciaManager
    {
        ResultIniProvincia TraerTodoProvincia();

        Provincia TraerProvincia(int intProvinciaId);

        Resultado GrabarProvincia(Provincia oProvincia);

        Resultado EliminarProvincia(int intProvinciaId);

        List<Provincia> ListarProvincia(string provincia);
    }
}


