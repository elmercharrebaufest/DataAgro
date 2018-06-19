
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using Mastersoft.Framework.Interfaces;

namespace Molinos.DataAgro.Entities
{
    public partial class ProveedorEstado : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProveedorEstadoId { get; set; }
        public int ProveedorId { get; set; }
        public int EstadoId { get; set; }
        public int ComercialId { get; set; }

        public ProveedorEstado()
        {

        }
    }


}



