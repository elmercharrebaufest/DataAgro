using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Log
    {
        [Key]
        public int Id { get; set; }
        public DateTime Fecha { get; set; } 
        public string Xml { get; set; }

    }
}
