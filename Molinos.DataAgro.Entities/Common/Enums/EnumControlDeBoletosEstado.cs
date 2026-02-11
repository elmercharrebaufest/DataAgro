using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Common.Enums
{
    public enum EnumControlDeBoletosEstado
    {
        PENDIENTE_CONTROL = 1,
        EN_PROCESO = 2,
        EN_OBLEA = 3,
        EN_CERTIFICACION = 4,
        ENVIADO_AFIP = 5,
        FINALIZADO = 6,
        ANULADO = 7
    }
}
