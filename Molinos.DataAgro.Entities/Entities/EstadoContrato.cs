using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using Mastersoft.Framework.Interfaces;

namespace Molinos.DataAgro.Entities
{
    public partial class EstadoContrato : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int EstadoContratoId { get; set; }
        public string Descripcion { get; set; }
        public int Orden { get; set; }
    }
}
