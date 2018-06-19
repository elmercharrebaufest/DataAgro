using Mastersoft.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities
{
    public class PostIt : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ComercialId { get; set; }
        public String Texto { get; set; }

        public PostIt()
        {
        }
    }
}
