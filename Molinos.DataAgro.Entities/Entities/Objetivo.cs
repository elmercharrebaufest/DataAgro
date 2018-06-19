
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using Mastersoft.Framework.Interfaces;

namespace Molinos.DataAgro.Entities
{
    public partial class Objetivo : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ObjetivoId { get; set; }
        public int CampañaId { get; set; }
        public int NroItem { get; set; }
        public int ProveedorId { get; set; }
        public int MaterialId { get; set; }
        public double ToneladasObjetivos { get; set; }
        

        public Objetivo()
        {

        }
    }


}



