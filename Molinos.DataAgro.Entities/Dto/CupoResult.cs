using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class CupoResult : Resultado
    {
        public List<string> ListaCupos { get; set; } = new List<string>();

        public int CuposNormales { get; set; }
        public int CuposFlete { get; set; }

    }
}
