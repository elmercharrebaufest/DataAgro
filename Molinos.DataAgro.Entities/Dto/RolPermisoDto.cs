using Molinos.DataAgro.Entities.Seguridad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class RolPermisoDto
    {
        public int Id { get; set; }
        public int RolId{ get; set; }
        public string RolDescripcion { get; set; }
        public string PermisosDescripcion{ get; set; }
        public PermisosDataAgro Permisos{ get; set; }
    }
}
