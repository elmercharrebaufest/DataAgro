using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Interfaces
{
    public interface IAdministracionCupoManager
    {
        KendoGrid<AdministracionCupoDto> TraerTodaAdministracionCupo(KendoGridMvcRequest request, int? comercialId);
        AdministracionCupoDto TraerAdministracionCupo(int id);      
        Resultado CambiarEstadoRechazado(int idAdministracion, string idActiveDirectory);
        CupoResult AceptarCupoExcedente(int administracionId, int cantidad, int cantidadFp, string idActiveDirectory);
        void RechazarSolicitudesVencidas();
    }
}


