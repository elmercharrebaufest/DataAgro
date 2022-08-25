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
        public IEnumerable<int> PricingIds { get; set; } = new List<int>();
        public double SanLorenzo { get; set; }
        public IEnumerable<int> SanLorenzoIds { get; set; } = new List<int>();
        public double Acopio { get; set; }
        public IEnumerable<int> AcopioIds { get; set; } = new List<int>();
        public int TipoNegocioId { get; set; }
        public int Orden { get; set; }
        public double BahiaBlanca { get; set; }
        public IEnumerable<int> BahiaBlancaIds { get; set; } = new List<int>();
    }
}
