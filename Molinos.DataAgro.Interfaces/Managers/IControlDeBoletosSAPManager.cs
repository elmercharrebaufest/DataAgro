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
        Resultado RegistrarDatosPreCertificacion(ControlDeBoletosPreCertificacionServiceDto controlDeBoletosPreCertificacion);
        Resultado RegistrarDatosSeguimiento(ControlDeBoletosDatosSeguimientoServiceDto controlDeBoletosDatosSeguimiento);
    }
}
