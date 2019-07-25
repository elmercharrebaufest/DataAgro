using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebDataAgro.Models
{
    public class LogModel
    {
        public List<LogModel> Log;
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Xml { get; set; } 
    }
}