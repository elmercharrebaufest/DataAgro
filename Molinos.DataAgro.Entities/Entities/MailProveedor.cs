using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class MailProveedor
    {
        [Key]
        public int Id { get; set; }
      
        public int? ProveedorId { get; set; }
        public string Pesificado { get; set; }

       

        [ForeignKey("ProveedorId")]
        public virtual Proveedor Proveedor { get; set; }
       
    }
}
