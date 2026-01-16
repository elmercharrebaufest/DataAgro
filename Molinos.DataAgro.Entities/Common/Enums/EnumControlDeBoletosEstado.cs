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
        CONTROLADO = 2,
        EN_FIRMA = 3,
        PENDIENTE_REGISTRACIÓN = 4,
        REGISTRADO = 5,
        ANULADO = 6,
        ANULADO_POST_REGISTRACIÓN = 7,
        EXCLUÍDO = 8
    }
}
