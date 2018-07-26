using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;

namespace WebDataAgro.Models
{
    public class ResultActividadesModel
    {
        public List<MSErrorMessage> Errores { get; set; }
        public List<ActividadRecordatorio> Actividades { get; set; }
        
        public ResultActividadesModel()
        {
            this.Errores = new List<MSErrorMessage>();
            this.Actividades = new List<ActividadRecordatorio>();
        }
    }
}