using Molinos.DataAgro.Entities.Seguridad;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class RolPermiso
    {
        [Key]
        public int Id { get; set; }
        public int RolId { get; set; }
        public PermisosDataAgro Permiso { get; set; }
        [ForeignKey("RolId")]
        public virtual Rol Rol { get; set; }
    }
}
