using Molinos.DataAgro.Entities.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebDataAgro.Models
{
    public class ResultIniCampanaModel : Resultado
    {
        public List<CampañaDto> Datos { get; set; }

        public ResultIniCampanaModel()
        {
            this.Datos = new List<CampañaDto>();
        }
    }
}