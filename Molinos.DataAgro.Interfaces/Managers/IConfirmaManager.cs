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
        List<string> ListarNegociosPorRangoCodigoSAP(int negocioDesde, int negocioHasta, int tipoNegocio, List<int> equipo);
        List<string> ValidarNegocios(List<string> codigosSAP, int claseNegocio, List<int> equipo);
        string ValidarNegocio(string codigoSAP, int tipoNegocio, List<int> equipo);
        List<string> FiltrarNegociosPorFecha(string desde, string hasta, int tipoNegocio, List<int> equipo);
        ConfirmaResult GrabarConfirmas(int claseNegocio, int ComercialId, List<string> codigosSap, bool usarWebServiceConfirma, List<int> equipo);
        string GenerarNombreArchivoConfirma(string codigoSAP);
        byte[] ConfirmaEnByte(string codigoSAP, List<int> equipo);
        string EnviarMailConfirmas();
        List<ConfirmaArchivoDto> ListarConfirmas();

    }
}
