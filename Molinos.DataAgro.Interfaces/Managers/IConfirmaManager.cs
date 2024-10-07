using Kendo.DynamicLinq;
using Molinos.DataAgro.Entities.Dto;
using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface IConfirmaManager
    {
        DatosIniContrato TraerDatosCombos();
        ConfirmaResult GrabarConfirmas(int claseNegocio, int ComercialId, List<string> codigosSap, bool usarWebServiceConfirma, List<string> clausulas, List<int> equipo);
        string ObtenerNombreArchivoConfirma(string codigoSAP);
        byte[] ObtenerArchivoXML(string nombreArchivo);
        void EnviarMailConfirma(DateTime fecha);
        List<ConfirmaArchivoDto> ListarConfirmas();
        List<ResultadoClausula> ObtenerClausulas(BasicoContrato basico);
        string CorregirFormatoFecha(string cadena);
        DataSourceResult TraerNegociosFiltrados(DataSourceRequest filtro,List<int> equipo);
        List<string> ObtenerClausulasPorNegocio(string contratoSap, List<int> equipo);
        string ValidarNegocio(string negocioSAP, List<int> equipo);
    }
}
