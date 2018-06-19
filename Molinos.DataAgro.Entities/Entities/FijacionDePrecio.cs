
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using Mastersoft.Framework.Interfaces;

namespace Molinos.DataAgro.Entities
{
    public partial class FijacionDePrecio : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int FijacionId { get; set; }

        public Nullable<int> MaterialId { get; set; }
        public Nullable<decimal> Precio { get; set; }
        public Nullable<System.DateTime> Fecha { get; set; }
        public Nullable<int> ProveedorId { get; set; }


        public FijacionDePrecio()
        {
            this.FijacionId = 0;
            this.MaterialId = null;
            this.Precio = null;
            this.Fecha = DateTime.Now;
            this.ProveedorId = null;
  
        }
    }
}
   
