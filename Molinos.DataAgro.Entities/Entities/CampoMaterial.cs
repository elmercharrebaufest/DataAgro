
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using Mastersoft.Framework.Interfaces;

namespace Molinos.DataAgro.Entities
{
    public partial class CampoMaterial : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CampoMaterialid { get; set; }
        public int CampoId { get; set; }
        public int NroItem { get; set; }
        public int MaterialId { get; set; }
        public int? Hectareas { get; set; }
        public int? Toneladas { get; set; }
        public int CampañaId { get; set; }

        public CampoMaterial()
        {
            
        }
    }


}
   


