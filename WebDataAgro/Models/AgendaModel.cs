using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Entities;

namespace WebDataAgro.Models
{
    public class ResultAgendaModel
    {
        public List<MSErrorMessage> Errores { get; set; }
        public List<ActividadRecordatorio> Actividades { get; set; }
        
        public ResultAgendaModel()
        {
            this.Errores = new List<MSErrorMessage>();
            this.Actividades = new List<ActividadRecordatorio>();
        }
    }

    
}