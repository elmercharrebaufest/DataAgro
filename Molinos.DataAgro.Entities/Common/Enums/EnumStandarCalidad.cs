using System.ComponentModel;

namespace Molinos.DataAgro.Entities.Common.Enums
{
    public enum EnumStandarCalidad
    {
        [Description("CodigoSap = 3")]
        CAMARA = 1,
        [Description("CodigoSap = 4")]
        ESPECIAL = 2,
        [Description("CodigoSap = 3")]
        FABRICA = 3,
        [Description("CodigoSap = 1")]
        CAMARA_1 = 4,
        [Description("CodigoSap = 2")]
        CAMARA_2 = 5,
        [Description("CodigoSap = 5")]
        MATERIA_EXTRANA = 6,
        [Description("CodigoSap = 6")]
        GRADO_2 = 7
    }
}