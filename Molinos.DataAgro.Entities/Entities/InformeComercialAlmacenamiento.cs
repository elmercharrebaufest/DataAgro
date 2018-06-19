using Mastersoft.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities
{
    public partial class InformeComercialAlmacenamiento : Entity
    {

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int InformeComercialAlmacenamientoId { get; set; }
        public Nullable<int> InformeComercialId { get; set; }
        public Nullable<Decimal> Toneladas { get; set; }
        public Nullable<int> LocalidadId { get; set; }
        public Nullable<bool> Propia { get; set; }
        public Nullable<bool> Alquilada { get; set; }
        
    }
}
