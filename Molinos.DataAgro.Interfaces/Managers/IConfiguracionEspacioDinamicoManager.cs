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
    public interface IConfiguracionEspacioDinamicoManager
    {
        Resultado GrabarConfiguracionEspacioDinamico(ConfiguracionEspacioDinamico cupo, List<DiaCupo> dias);
        KendoGrid<ConfiguracionEspacioDinamicoDto> TraerTodaConfiguracionEspacioDinamico(KendoGridMvcRequest request);
        ConfiguracionEspacioDinamicoDto TraerConfiguracionEspacioDinamico(int id);
        Resultado EliminarConfiguracionEspacioDinamico(int id);
    }
}


