using System.Collections.Generic;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class RolDto
    {
        public int Id{ get; set; }
        public string Descripcion { get; set; }
        public string Permisos{ get; set; }
        public IEnumerable<PermisosDataAgro> PermisosEnum { get; set; }
    }
}
