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
    public interface IBoletoManager
    {
        DatosIniContrato TraerDatosCombo(int? tipoNegocioId = null);
        BoletoResult GrabarBoleto(List<int> tipoNegocios, int comercialId, List<string> contratos, bool enviarEmail, List<int> equipo);
        string ObtenerIdentDescarga();
        byte[] BoletoEnByte(string archivoUrl);
        List<string> FiltrarNegociosPorFecha(string desde, string hasta, int negocio);
    }
}
