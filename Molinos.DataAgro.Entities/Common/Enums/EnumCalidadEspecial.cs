using System.ComponentModel;

namespace Molinos.DataAgro.Entities.Common.Enums
{
    public enum EnumCalidadEspecial
    {
        [Description("Soja")]
        DAÑADOS = 1,
        [Description("Soja")]
        GRANOS_VERDES = 2,
        [Description("Maiz")]
        GRADO = 4,
        [Description("Trigo")]
        GRADO_2 = 5,
        [Description("Girasol")]
        MATERIA_EXTRAÑA = 8,
        [Description("Girasol AO")]
        MATERIA_EXTRAÑA_AO = 9,
        [Description("Trigo")]
        ESPECIAL = 10,
        [Description("Trigo")]
        HUMEDO = 11
    }
}
