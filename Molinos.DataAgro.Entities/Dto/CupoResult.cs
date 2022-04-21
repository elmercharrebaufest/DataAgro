using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class CupoResult : Resultado
    {
        public List<string> ListaCupos { get; set; } = new List<string>();

        public int CuposNormales { get; set; }
        public int CuposFlete { get; set; }

        public List<string> Estados { get; set; }
        public string Codigo { get; set; }

        public List<CupoNoPropioDto> CupoNoPropios { get; set; }
    }
}
