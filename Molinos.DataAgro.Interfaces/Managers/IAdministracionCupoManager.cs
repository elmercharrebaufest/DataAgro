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
        Resultado CambiarEstadoRechazado(int idAdministracion, string motivo, string idActiveDirectory);
        CupoResult AceptarCupoExcedente(int administracionId, int cantidad, int cantidadFp, int cantidadOriginal, int cantidadFleteOriginal, string idActiveDirectory, string motivo);
        void RechazarSolicitudesVencidas();
        string ActualizarSolicitud(int id, int cantidadCupo, int cantidadFlete, bool estado);
    }
}


