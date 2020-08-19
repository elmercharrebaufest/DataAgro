using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public class CampanaMaterialDetallePorMeseExcelDto : ICloneable
    {
        public string Situacion { get; set; }
        public string Clasificacion { get; set; }
        public string Cuit { get; set; }
        public string RazonSocial {get; set;}

        public string Contrato { get; set; }

        public double Tn { get; set; }
        public string Campaña { get; set; }

        public string Material { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
