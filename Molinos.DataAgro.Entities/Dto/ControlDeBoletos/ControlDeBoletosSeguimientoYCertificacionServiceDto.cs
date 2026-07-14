using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto.ControlDeBoletos
{
    public class ControlDeBoletosSeguimientoYCertificacionServiceDto
    {
        public string ContratoSAP { get; set; }
        public string CodigoRegistracionAfip { get; set; }
        public string FechaRegistracionAfip { get; set; }
        public string FechaRecepcionAfip { get; set; }
    }
}
