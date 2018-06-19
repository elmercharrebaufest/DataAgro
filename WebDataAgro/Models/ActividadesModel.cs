using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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