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
        int LogCambiosDataAgro(StoredPorProveedorResult cambios, TipoAccionLogDataAgro tipoDeAccion, int idProveedor);
        LogDataAgroDto Obtener(int idLogDataAgro, bool anterior = false);
        int LogCambiosDataAgro(RangoConfirmacionAutomaticaDto cambios, TipoAccionLogDataAgro tipoDeAccion);
        int LogCambiosDataAgro(HabilitacionFijacionDto habilitacionFijacionDto, TipoAccionLogDataAgro tipoDeAccion);
        int LogCambiosDataAgro(HabilitacionPizarraDto habilitacionPizarraDto, TipoAccionLogDataAgro tipoDeAccion);
        int LogCambiosDataAgro(HabilitacionPagoDiferidoDto habilitacionPizarraDto, TipoAccionLogDataAgro tipoDeAccion);
        int LogCambiosDataAgro(PrecioMoaDto precioMoaDto, TipoAccionLogDataAgro tipoDeAccion);
        string AddSpacesToSentence(string text, char v);
        string BuscaFechaYFormatea(string text, string campo);
        List<int> ObtenerNegociosId(List<string> contratosSap);
        List<int> ObtenerCuposId(List<string> cupoSap);
        int LogCambiosDataAgro(HabilitacionCampañaDto habilitacionCampañaDto, TipoAccionLogDataAgro tipo);
    }
}