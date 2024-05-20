using Kendo.DynamicLinq;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Interfaces
{
    public interface IConfirmaManager
    {
        DatosIniContrato TraerDatosCombos();
        List<string> ListarNegociosPorRangoCodigoSAP(int negocioDesde, int negocioHasta, int tipoNegocio);
        List<string> ValidarNegocios(List<string> codigosSAP, int claseNegocio);
        string ValidarNegocio(string codigoSAP, int tipoNegocio);
        List<string> FiltrarNegociosPorFecha(string desde, string hasta, int tipoNegocio);
        ConfirmaResult GrabarConfirmas(int tipoNegocio,int ComercialId,List<string> contratos, bool usarWebService, List<int> equipo);
        byte[] ConfirmaEnByte(string confirma);
        string GenerarNombreArchivoConfirma(string codigoSAP);
    }
}
