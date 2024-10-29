using Kendo.DynamicLinq;
using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface IBoletoManager
    {
        DatosIniContrato TraerDatosCombo(int? tipoNegocioId = null);
        BoletoResult GrabarBoleto(List<string> contratos, List<int> tipoNegocios, BoletoDto boleto, List<int> equipo);
        string ObtenerIdentDescarga();
        byte[] BoletoEnByte(string archivoUrl);
        void ReenviarBoletos(List<string> listaContratos, List<string> archivos, string pathArchivos);
        BoletoDto ValidarNegocioParaGenerarBoleto(BasicoContrato negocio);
        List<string> ObtenerClausulasPorNegocio(string contratoSap, List<int> equipo);
        DataSourceResult TraerContratosFiltrados(DataSourceRequest filtro, List<int> equipo);
        string ValidarNegocio(string negocioSAP, List<int> equipo);
    }
}