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
        Resultado GrabarConfiguracionCupo(ConfiguracionCupo cupo, List<DiaCupo> dias);
        KendoGrid<ConfiguracionCupoDto> TraerTodaConfiguracionCupo(KendoGridMvcRequest request);
        List<LimiteCupoDto> TraerLimites(int id);
        Resultado GrabarLimites(List<LimiteCupo> limite);
        ConfiguracionCupoDto TraerConfiguracionCupo(int id);
        Resultado CambioMasivo(bool aceptar);
        Resultado GrabarLimitesMasivo(List<LimiteCupo> limite, List<int> configuracionesIds);
        Resultado ModificarConfiguracion(int? id, int? limite, int? algoritmo, bool? bloquear, bool? liberar);

    }
}


