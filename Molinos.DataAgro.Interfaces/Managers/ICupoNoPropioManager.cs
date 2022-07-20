using Kendo.DynamicLinq;
using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Interfaces
{
    public interface ICupoNoPropioManager
    {
        CupoResult GrabarCupoNoPropio(CupoDto cupo);
        DataSourceResult TraerCuposNoPropioTabla(DataSourceRequest request, List<int> equipo);
        Resultado GrabarDisponibilidadCupoNoPropio(int id, bool disponibilidad);
         Resultado ModificacionMasivaDisponible(List<int> ids, bool disponible);
    }
}