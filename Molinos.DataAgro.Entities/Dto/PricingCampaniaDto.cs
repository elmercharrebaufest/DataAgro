using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public class PricingCampaniaDto
    {
        public int Id { get; set; }
        public string Material { get; set; }
        public int MaterialId { get; set; }
        public string Campania { get; set; }
        public int CampaniaId { get; set; }
        public double Pricing { get; set; }
        public IEnumerable<KeyValuePair<int, int>> PricingIds { get; set; } = new List<KeyValuePair<int, int>>();
        public double SanLorenzo { get; set; }
        public IEnumerable<KeyValuePair<int, int>> SanLorenzoIds { get; set; } = new List<KeyValuePair<int, int>>();
        public double Acopio { get; set; }
        public IEnumerable<KeyValuePair<int, int>> AcopioIds { get; set; } = new List<KeyValuePair<int, int>>();
        public int TipoNegocioId { get; set; }
    }
}
