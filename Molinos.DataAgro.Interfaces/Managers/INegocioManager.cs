using Kendo.DynamicLinq;
using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Interfaces
{
    public interface INegocioManager
    {
        Resultado OcultarEnTablero(Negocio negocio);
        void EnvioMailNegociosConDiaAnterior();
        void ConsultarContratosPrimary();
    }
}
