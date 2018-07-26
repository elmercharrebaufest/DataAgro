
using Mastersoft.Framework.Interfaces;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class ContactoComercial : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ContactoComercialId { get; set; }
        public int ProveedorId { get; set; }
        public string Apellido { get; set; }
        public string Nombres { get; set; }
        public string Puesto { get; set; }
        public string Telefono1 { get; set; }
        public Nullable<int> TipoTelefono1Id { get; set; }
        public string Telefono2 { get; set; }
        public Nullable<int> TipoTelefono2Id { get; set; }
        public string Telefono3 { get; set; }
        public Nullable<int> TipoTelefono3Id { get; set; }
        public string Email1 { get; set; }
        public string Email2 { get; set; }
        public string Email3 { get; set; }
        public System.DateTime? FechaNacimiento { get; set; }
        public int? CategoriaId { get; set; }
        public string OtrosIntereses { get; set; }
        public bool? EsPrincipal { get; set; }
        public string Cargo { get; set; }

        public ContactoComercial()
        {
            this.Apellido = "";
            this.Nombres = "";
            this.Puesto = "";
            this.Telefono1 = "";
            this.Telefono2 = "";
            this.Telefono3 = "";
            this.TipoTelefono1Id = 0;
            this.TipoTelefono2Id = 0;
            this.TipoTelefono3Id = 0;
            this.Email1 = "";
            this.Email2 = "";
            this.Email3 = "";
            this.FechaNacimiento = null;
            this.CategoriaId = 0;
            this.OtrosIntereses = "";
            this.EsPrincipal = null;
            this.Cargo = "";                       
        }
    }


}
   


