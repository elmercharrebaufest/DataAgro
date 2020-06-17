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
    public interface IHabilitacionCupoManager
    {
        Resultado GrabarHabilitacionCupo(HabilitacionCupo cupo);
        HabilitacionCupoDto TraerHabilitacionCupo(int id);
        KendoGrid<HabilitacionCupoDto> TraerTodaHabilitacionCupo(KendoGridMvcRequest request);

        List<HabilitacionCupoDto> TraerTodasHabilitacionesActivas(int zona);

        List<int> TraerTodoMaterialHabilitado(int zona);

        List<MaterialHabilitadoDto> TraerTodoMaterialRetirado(int zona);
    }
}


