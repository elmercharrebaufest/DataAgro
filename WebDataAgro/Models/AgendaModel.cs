using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;

namespace WebDataAgro.Models
{
    public class ResultAgendaModel : Resultado
    {
        public List<ActividadRecordatorio> Actividades { get; set; }
        
        public ResultAgendaModel()
        {
            this.Actividades = new List<ActividadRecordatorio>();
        }
    }

    
}