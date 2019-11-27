using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Rol
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual ICollection<RolPermiso> PermisosAsociados { get; set; }
        public virtual ICollection<Comercial> ComercialesAsociados { get; set; }
    }
}
