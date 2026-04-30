using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Common.Enums
{
    public enum EnumConfirmaEstadoDocumento
    {
        CONTRATO_PENDIENTE_DE_CONTROL = 1,
        CONTROLADO = 2,
        EN_FIRMA = 3,
        PENDIENTE_DE_REGISTRACION = 4,
        REGISTRADO = 5,
        ANULADO = 7,
        EXCLUIDO_POR_TIEMPO_EXCEDIDO = 9,
        ANULADO_POST_REGISTRACION = 99
    }
}
