
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using Mastersoft.Framework.Interfaces;

namespace Molinos.DataAgro.Entities
{
    public partial class Destinatario : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int DestinatarioId { get; set; }

        public string Descripcion { get; set; }

        public bool? Inhabilitado { get; set; }

        public Destinatario()
        {
            this.DestinatarioId = 0;
            this.Descripcion = "";
            this.Inhabilitado = false;
        }
    }
}
   



