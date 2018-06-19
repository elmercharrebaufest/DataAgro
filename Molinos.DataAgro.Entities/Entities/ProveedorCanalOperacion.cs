
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using Mastersoft.Framework.Interfaces;

namespace Molinos.DataAgro.Entities
{
    public partial class ProveedorCanalOperacion : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ContactoCanalOperacionId { get; set; }
        public int ProveedorId { get; set; }
        public string NroItem { get; set; }
        public int CanalOperacionId { get; set; }

        public ProveedorCanalOperacion()
        {
            
        }
    }


}
   


