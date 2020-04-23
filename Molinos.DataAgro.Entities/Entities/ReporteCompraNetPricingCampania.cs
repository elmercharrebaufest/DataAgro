using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Entities
{
    public class ReporteCompraNetPricingCampania
    {
        public int Id { get; set; }
        public string Material { get; set; }
        public int MaterialId { get; set; }
        public string Campania { get; set; }
        public int CampaniaId { get; set; }
        public double Pricing { get; set; }
        public double SanLorenzo { get; set; }
        public double Acopio { get; set; }

    }
}
