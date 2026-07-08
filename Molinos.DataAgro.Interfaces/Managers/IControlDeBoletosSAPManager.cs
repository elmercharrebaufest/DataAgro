using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Dto.ControlDeBoletos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Interfaces.Managers
{
    public interface IControlDeBoletosSAPManager
    {
        /// <summary>
        /// Registra datos de pre-certificación de un único contrato.
        /// Ejecuta RFC SAP y guarda cambios en BD.
        /// </summary>
        ControlDeBoletosOperacionSapResultadoDto RegistrarDatosPreCertificacion(ControlDeBoletosPreCertificacionServiceDto controlDeBoletosPreCertificacion);

        /// <summary>
        /// Registra datos de seguimiento de un único contrato.
        /// Ejecuta RFC SAP y guarda cambios en BD.
        /// </summary>
        ControlDeBoletosOperacionSapResultadoDto RegistrarDatosSeguimiento(ControlDeBoletosDatosSeguimientoServiceDto controlDeBoletosDatosSeguimiento);
    }
}
