using Molinos.DataAgro.Entities.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class LogProveedor
    {
        [Key]
        public int LogProveedorId { get; set; } // LogProveedorId (Primary key)
       
        public int ProveedorId { get; set; } // ProveedorId
       
        public DateTime Fecha { get; set; } // Fecha
       
        public int? ComercialId { get; set; } // ComercialId
        
        [ForeignKey("ProveedorId")]
        public virtual Proveedor Proveedor { get; set; } // ProveedorId
      
        [ForeignKey("ComercialId")]
        public virtual Comercial Comercial { get; set; } // ComercialId
       
        public LogProveedor()
        {
            Fecha = DateTime.Now;
        }
    }
}



