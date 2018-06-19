using Mastersoft.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities
{
    public class Interes : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int InteresId { get; set; }

        public string Descripcion { get; set; }


        public Interes()
        {
            this.InteresId = 0;
            this.Descripcion = "";

        }
    }
}
