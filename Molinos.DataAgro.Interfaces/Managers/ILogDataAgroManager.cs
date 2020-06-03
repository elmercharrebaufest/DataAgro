using Kendo.DynamicLinq;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface ILogDataAgroManager
    {
        int LogCambiosDataAgro(Cupo cambios, TipoAccionLogDataAgro tipoDeAccion);
        int LogCambiosDataAgro(Proveedor cambios, TipoAccionLogDataAgro tipoDeAccion);
        int LogCambiosDataAgro(Negocio cambios, TipoAccionLogDataAgro tipoDeAccion);
        int LogCambiosDataAgro(List<Cupo> cambios, TipoAccionLogDataAgro tipoDeAccion);
        DataSourceResult ListarDatosLogDataAgro(DataSourceRequest request, List<int> equipo);
    }
}