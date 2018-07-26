
using Mastersoft.Framework.Interfaces;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Comercial : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ComercialId { get; set; }

        public string Apellido { get; set; }
        public string Nombres { get; set; }
        public int PerfilId { get; set; }
        public Nullable<int> EmpleadorACargo { get; set; }
        public string IdActiveDirectory { get; set; }
        public Nullable<int> GrupoDeCompras { get; set; }
        public Nullable<bool> Administrador { get; set; }


        public Comercial()
        {
            this.ComercialId = 0;
            this.Apellido = "";
            this.Nombres = "";
            this.PerfilId = 0;
            this.EmpleadorACargo = null;
            this.IdActiveDirectory = "";
            this.GrupoDeCompras = null;
            this.Administrador = false;

        }
    }
}


