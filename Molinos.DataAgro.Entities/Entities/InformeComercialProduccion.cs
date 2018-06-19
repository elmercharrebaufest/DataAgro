using Mastersoft.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities
{
    public partial class InformeComercialProduccion : Entity
    {

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int InformeComerciaProduccionId { get; set; }
        public int InformeComercialId { get; set; }
        public Nullable<int> MaterialId { get; set; }
        public Nullable<int> Hectareas { get; set; }
        public Nullable<decimal> Toneladas { get; set; }
        public Nullable<int> LocalidadId { get; set; }
        public Nullable<bool> Propio { get; set; }
        public Nullable<bool> Alquilado { get; set; }
        public Nullable<bool> RtaOkSap { get; set; }
        public string MensajeSap { get; set; }

    }
}
