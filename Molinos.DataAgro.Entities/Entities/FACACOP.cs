
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using Mastersoft.Framework.Interfaces;

namespace Molinos.DataAgro.Entities
{
    public partial class FACACOP : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string CUIT { get; set; }
        public DateTime Fecha1 { get; set; }
        public DateTime Fecha2 { get; set; }
        public string ObservacionesEspeciales { get; set; }

        public FACACOP()
        {
            
        }
    }


}
   


