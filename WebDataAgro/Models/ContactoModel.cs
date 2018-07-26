using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Entities.Dto;

namespace WebDataAgro.Models
{
 
    public class DatosIniAbmContactoModel
    {
        public List<MSErrorMessage> Errores { get; set; }
            

        public DatosIniAbmContactoModel()
        {
            this.Errores = new List<MSErrorMessage>();                
        }
            
    }


    public class ResultIniContactoModel
    {
        public List<MSErrorMessage> Errores { get; set; }
        public List<ContactoIni> Contactos { get; set; }
        public CampañaHome Campaña { get; set; }
        public DatosIniciales Datos { get; set; }

        public ResultIniContactoModel()
        {
            this.Errores = new List<MSErrorMessage>();
            this.Contactos = new List<ContactoIni>();
            this.Datos = new DatosIniciales();
        }

       
    }

}