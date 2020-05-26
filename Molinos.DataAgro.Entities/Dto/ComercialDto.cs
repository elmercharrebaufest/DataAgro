using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class ComercialDto
    {
        public int ComercialId { get; set; }
        public string Apellido { get; set; }
        public string Nombres { get; set; }
        public int PerfilId { get; set; }
        public int? EmpleadorACargoId { get; set; }
        public string IdActiveDirectory { get; set; }
        public int? GrupoDeComprasId { get; set; }
        public string GrupoDeCompras { get; set; }
        public bool? Administrador { get; set; }
        public bool? Cupera { get; set; }
        public ICollection<RolBasicoDto> RolesAsociados { get; set; }

        public string NombreCompleto { get { return Apellido.ToUpper() + " " + Nombres.ToUpper(); }  }

        public bool Deshabilitado { get; set; }
    }
}


