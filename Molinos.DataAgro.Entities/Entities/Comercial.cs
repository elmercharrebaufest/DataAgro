using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Comercial
    {
        [Key]
        public int ComercialId { get; set; }
        public string Apellido { get; set; }
        public string Nombres { get; set; }
        public int? PerfilId { get; set; }
        public int? EmpleadorACargoId { get; set; }        
        public string IdActiveDirectory { get; set; }
        public int? GrupoDeComprasId { get; set; }        
        public bool? Administrador { get; set; }
        public bool? Cupera { get; set; }
        public bool? Deshabilitado { get; set; }
        public DateTime? FechaDeshabilitado { get; set; }
        public bool AsignarNegocios { get; set; }
        public string IdUsuarioSAP { get; set; }


        [ForeignKey("PerfilId")]
        public virtual Perfil Perfil { get; set; }
        [ForeignKey("EmpleadorACargoId")]
        public virtual Comercial EmpleadorACargo { get; set; }
        [ForeignKey("GrupoDeComprasId")]
        public virtual GrupoDeCompras GrupoDeCompras { get; set; }

        [InverseProperty("ComercialesAsociados")]
        public virtual ICollection<Rol> RolesAsociados { get; set; }

        public Comercial()
        {
            Apellido = "";
            Nombres = "";
            IdActiveDirectory = "";
        }
    }
}


