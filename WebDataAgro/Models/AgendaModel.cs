using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;

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