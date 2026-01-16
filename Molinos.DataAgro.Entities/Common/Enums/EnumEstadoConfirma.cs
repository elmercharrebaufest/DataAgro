using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Common.Enums
{
    public enum EnumEstadoConfirma
    {
        PENDIENTE = 1,
        EN_PROCESO = 2,
        EN_OBLEA = 3,
        EN_CERTIFICACION = 4,
        ENVIADO_AFIP_ARCA = 5,
        FINALIZADO = 6,
        ANULADO = 7
    }
}
