using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface ICupoManager
    {
        Resultado GrabarCupo(Cupo cupo,int cantidadCupos);
        KendoGrid<CupoDto> TraerCuposTabla(KendoGridMvcRequest request, List<int> equipo);
        Resultado EliminarCupo(int id);
    }
}
