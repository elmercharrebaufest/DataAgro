using Molinos.DataAgro.Entities.Common.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class UsuarioExterno
    {
        [Key]
        public int Id { get; set; }
        public int ProveedorId { get; set; }
        public string Nombre { get; set; }
        public bool AceptaTyC { get; set; }
        
        [ForeignKey("ProveedorId")]
        public virtual Proveedor Proveedor { get; set; }
    }
}

