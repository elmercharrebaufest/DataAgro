using Mastersoft.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mastersoft.Framework.Standard;

namespace Molinos.DataAgro.Entities
{
    public partial class NuevoProveedorDatos : Entity
    {
        [NotMapped]
        public virtual NuevoProveedor Proveedor { get; set; }

        public NuevoProveedorDatos()
        {

            this.Proveedor = null;
        }
    }
}
