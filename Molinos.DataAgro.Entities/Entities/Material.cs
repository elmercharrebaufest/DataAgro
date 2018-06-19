
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using Mastersoft.Framework.Interfaces;

namespace Molinos.DataAgro.Entities
{
    public partial class Material : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MaterialId { get; set; }

        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public int? CampañaId { get; set; }

        public Material()
        {
            this.MaterialId = 0;
            this.Codigo = "";
            this.Descripcion = "";
        }
    }
}
   



