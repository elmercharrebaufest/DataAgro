using Kendo.DynamicLinq;
using Molinos.DataAgro.Entities.Dto;
using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface IConfirmaManager
    {
        DatosIniContrato TraerDatosCombos();
        ConfirmaResult GrabarConfirmas(int claseNegocio, int ComercialId, List<string> codigosSap, bool usarWebServiceConfirma, List<int> equipo);
        string GenerarNombreArchivoConfirma(string codigoSAP);
        byte[] ObtenerArchivoXML(string nombreArchivo);
        void EnviarMailConfirma(DateTime fecha);
        List<ConfirmaArchivoDto> ListarConfirmas();
        List<ResultadoClausula> ObtenerClausulas(BasicoContrato basico);
        string CorregirFormatoFecha(string cadena);
        DataSourceResult TraerNegociosFiltrados(DataSourceRequest filtro);
    }
}
