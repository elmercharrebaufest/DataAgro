using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;

namespace WebDataAgro.Models
{
    public class ResultActividadesModel : Resultado
    {
        public List<ActividadRecordatorio> Actividades { get; set; }
        
        public ResultActividadesModel()
        {
            this.Actividades = new List<ActividadRecordatorio>();
        }
    }
}