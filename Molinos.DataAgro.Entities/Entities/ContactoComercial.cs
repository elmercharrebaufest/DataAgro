
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class ContactoComercial
    {
        [Key]
        public int ContactoComercialId { get; set; }
        public int ProveedorId { get; set; }
        public string Apellido { get; set; }
        public string Nombres { get; set; }
        public string Puesto { get; set; }
        public string Telefono1 { get; set; }
        public int? TipoTelefono1Id { get; set; }
        public string Telefono2 { get; set; }
        public int? TipoTelefono2Id { get; set; }
        public string Telefono3 { get; set; }
        public int? TipoTelefono3Id { get; set; }
        public string Email1 { get; set; }
        public string Email2 { get; set; }
        public string Email3 { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string OtrosIntereses { get; set; }
        public bool? EsPrincipal { get; set; }
        public string Cargo { get; set; }
        public bool? CompraNet { get; set; }
        public bool? Cupo { get; set; }
        public bool? Boleto { get; set; }

        [ForeignKey("ProveedorId")]
        public virtual Proveedor Proveedor { get; set; }

        public ContactoComercial()
        {
            Apellido = "";
            Nombres = "";
            Puesto = "";
            Telefono1 = "";
            Telefono2 = "";
            Telefono3 = "";
            TipoTelefono1Id = 0;
            TipoTelefono2Id = 0;
            TipoTelefono3Id = 0;
            Email1 = "";
            Email2 = "";
            Email3 = "";
            FechaNacimiento = null;
            OtrosIntereses = "";
            EsPrincipal = null;
            Cargo = "";
            CompraNet = null;
        }
    }


}
   


