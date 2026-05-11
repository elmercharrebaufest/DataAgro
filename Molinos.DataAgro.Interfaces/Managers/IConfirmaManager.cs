using Kendo.DynamicLinq;
using Molinos.DataAgro.Entities.Dto;
using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface IConfirmaManager
    {
        DatosIniContrato TraerDatosCombos();
        ConfirmaResult GrabarConfirmas(int ComercialId, List<string> codigosSap, bool usarWebServiceConfirma, List<string> clausulas, List<int> equipo);
        ConfirmaResult GrabarConfirmasAltaBorrador(int ComercialId, List<string> codigosSap, bool usarWebServiceConfirma, List<string> clausulas, List<int> equipo);

        string ObtenerNombreArchivoConfirma(string codigoSAP);
        byte[] ObtenerArchivoXML(string nombreArchivo);
        void EnviarMailConfirma(DateTime fecha);
        List<ConfirmaArchivoDto> ListarConfirmas(string filtroArchivo);
        List<ResultadoClausula> ObtenerClausulas(BasicoContrato basico);
        string CorregirFormatoFecha(string cadena);
        (List<BasicoConfirma> Data, int Total) TraerNegociosFiltrados(ConfirmaFiltroBusquedaDto filtro, List<int> equipo);
        List<string> ObtenerClausulasPorNegocio(string contratoSap, List<int> equipo);
        string ValidarNegocio(string negocioSAP, List<int> equipo);
        List<string> ListarComerciales();
        List<string> ListarCorredores();
        List<string> ListarVendedores();
        byte[] DescargarZipConfirmas(List<string> nombresArchivos);
        string ValidarContratoConfirma(string numeroSap, List<int> equipo);
    }
}
