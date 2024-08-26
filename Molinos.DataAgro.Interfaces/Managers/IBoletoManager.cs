using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface IBoletoManager
    {
        DatosIniContrato TraerDatosCombo(int? tipoNegocioId = null);
        BoletoResult GrabarBoleto(List<string> contratos, List<int> tipoNegocios, int comercialId, bool enviarEmail, List<int> equipo);
        string ObtenerIdentDescarga();
        byte[] BoletoEnByte(string archivoUrl);
        List<string> FiltrarNegociosPorFecha(string desde, string hasta, int negocio);
        List<string> FiltrarNegociosNumeroSAP(int negocioDesde, int negocioHasta, int tipoNegocio);
        void ReenviarBoletos(List<string> listaContratos, List<string> archivos, string pathArchivos);
        BoletoDto ValidarNegocioParaGenerarBoleto(BasicoContrato negocio);
    }
}