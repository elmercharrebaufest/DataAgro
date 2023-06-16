using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

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
        Resultado GrabarLimitesMasivo(List<LimiteCupo> limite, List<int> configuracionesIds, int limiteAlgoritmo, int limiteDescarga);
        Resultado ModificarConfiguracion(int? id, int? limite, int? algoritmo, int? descarga, bool? bloquear, bool? liberar);
        int TraerLimiteMinimoCupoConDescarga(int id);
    }
}