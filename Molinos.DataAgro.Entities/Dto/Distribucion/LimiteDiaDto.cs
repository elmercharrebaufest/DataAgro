using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto.Distribucion
{
    public class LimiteDiaDto
    {
        public string Fecha { get; set; }
        public Dictionary<string, int> Limites { get; set; }

        public LimiteDiaDto()
        {
            Limites = new Dictionary<string, int>();
        }
    }
}
