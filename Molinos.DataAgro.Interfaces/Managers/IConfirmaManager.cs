using Molinos.DataAgro.Entities.Dto;
using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface IConfirmaManager
    {
        DatosIniContrato TraerDatosCombos();
        List<string> ListarNegociosPorRangoCodigoSAP(int negocioDesde, int negocioHasta, int tipoNegocio, List<int> equipo);
        List<string> ValidarNegocios(List<string> codigosSAP, int claseNegocio, List<int> equipo);
        string ValidarNegocio(string codigoSAP, int tipoNegocio, List<int> equipo);
        List<string> FiltrarNegociosPorFecha(string desde, string hasta, int tipoNegocio, List<int> equipo);
        ConfirmaResult GrabarConfirmas(int claseNegocio, int ComercialId, List<string> codigosSap, bool usarWebServiceConfirma, List<int> equipo);
        string GenerarNombreArchivoConfirma(string codigoSAP);
        byte[] ObtenerArchivoXML(string nombreArchivo);
        void EnviarMailConfirma(DateTime fecha);
        List<ConfirmaArchivoDto> ListarConfirmas();
        List<ResultadoClausula> ObtenerClausulas(BasicoContrato basico);
        string CorregirFormatoFecha(string cadena);
    }
}
