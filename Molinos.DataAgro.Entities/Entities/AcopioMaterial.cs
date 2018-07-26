
using Mastersoft.Framework.Interfaces;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
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
   


