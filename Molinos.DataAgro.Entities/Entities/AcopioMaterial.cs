
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using Mastersoft.Framework.Interfaces;

namespace Molinos.DataAgro.Entities
{
    public partial class AcopioMaterial : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AcopioMaterialId { get; set; }
        public int AcopioId { get; set; }
        public int NroItem { get; set; }
        public Nullable<double> Toneladas { get; set; }
        public int CampañaId { get; set; }
        public int MaterialId { get; set; }

        public AcopioMaterial()
        {
            
        }
    }


}
   


