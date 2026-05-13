using Kendo.DynamicLinq;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
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
        (List<BasicoBoleto> Data, int Total) TraerNegociosPendientesFiltrados(BoletoFiltroBusquedaDto filtro, List<int> equipo);
        (List<BasicoBoleto> Data, int Total) TraerContratosFiltrados(BoletoFiltroBusquedaDto filtro, List<int> equipo);
        string ValidarNegocio(string negocioSAP, List<int> equipo);
        string ValidarContratoTipoBoleto(string numeroSap, List<int> equipo);

        List<BolsaCompraNet> GetBolsaCompraNet();
        List<ComercialCombo> GetComercial();
        List<MaterialCombo> GetMaterial();
        List<ProveedorCombo> GetProveedorPorComercial(List<int> equipo);

    }
}
