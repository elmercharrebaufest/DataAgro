using Kendo.DynamicLinq;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface ILogDataAgroManager
    {
        int LogCambiosDataAgro(CupoDto cambios, TipoAccionLogDataAgro tipoDeAccion);
        DataSourceResult ListarDatosLogDataAgro(DataSourceRequest request, List<int> equipo);
        DatosModificadosLogDataAgroDto TraerDatosModificadosPorId(int idLogDataAgro);
        int LogCambiosDataAgro(BasicoContrato cambios, TipoAccionLogDataAgro tipoDeAccion, Type tipoDeContrato);
        int LogCambiosDataAgro(StoredPorProveedorResult cambios, TipoAccionLogDataAgro tipoDeAccion, int? idProveedor);
    }
}