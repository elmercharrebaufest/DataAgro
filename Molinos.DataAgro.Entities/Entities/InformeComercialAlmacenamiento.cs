using Mastersoft.Framework.Interfaces;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
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
