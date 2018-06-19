
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using Mastersoft.Framework.Interfaces;

namespace Molinos.DataAgro.Entities
{
    public partial class CampañaMaterial : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CampañaMaterialId { get; set; }
        public int CampañaId { get; set; }
        public int NroItem { get; set; }
        public int ProveedorId { get; set; }
        public int MaterialId { get; set; }
        public double ToneladasCompradas { get; set; }

        public CampañaMaterial()
        {
            
        }
    }


}
   


