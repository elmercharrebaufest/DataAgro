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
    public interface IConfiguracionCupoManager
    {
        Resultado GrabarConfiguracionCupo(ConfiguracionCupo cupo);
        KendoGrid<ConfiguracionCupoDto> TraerTodaConfiguracionCupo(KendoGridMvcRequest request);
        List<LimiteCupoDto> TraerLimites(int id);
        Resultado GrabarLimites(List<LimiteCupo> limite);
        ConfiguracionCupoDto TraerConfiguracionCupo(int id);
        List<ConfiguracionCupoDto> TraerTodaConfiguracionCupoPorDia(int zonaId, int materialId, int centroId);
    }
}


