using Molinos.DataAgro.Entities.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Common.Enums
{
    public enum EnumEstadoAdministracionCupo
    {
        [Display(ResourceType = typeof(Text), Name = "EstadoAceptadoAdministracionCupo")]
        EstadoAceptadoAdministracionCupo = 1,
        [Display(ResourceType = typeof(Text), Name = "EstadoRechazadoAdministracionCupo")]
        EstadoRechazadoAdministracionCupo = 2,
        [Display(ResourceType = typeof(Text), Name = "EstadoPendienteAdministracionCupo")]
        EstadoPendienteAdministracionCupo = 3,
    }
}
