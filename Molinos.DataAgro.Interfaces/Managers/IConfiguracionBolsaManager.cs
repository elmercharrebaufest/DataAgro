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
    public interface IConfiguracionBolsaManager
    {

        Resultado GrabarConfiguracionBolsa(ConfiguracionBolsa bolsa);
        KendoGrid<ConfiguracionBolsaDto> TraerTodaConfiguracionBolsa(KendoGridMvcRequest request);

        ConfiguracionBolsaDto TraerConfiguracionBolsa(int id);

        Resultado EliminarConfiguracionBolsa(int id);

        ConfiguracionBolsa TraerConfiguracionBolsaConDestinoYProcedencia(int destinoId, int localidadId);
    }
}


