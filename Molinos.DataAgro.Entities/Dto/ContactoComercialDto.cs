
using System;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class ContactoComercialDto
    {
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
    }
}
   


