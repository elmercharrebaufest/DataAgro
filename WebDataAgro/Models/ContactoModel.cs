using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;

namespace WebDataAgro.Models
{

    public class ResultIniContactoModel : Resultado
    {
        public List<ContactoIni> Contactos { get; set; }
        public CampañaHome Campaña { get; set; }
        public DatosIniciales Datos { get; set; }

        public ResultIniContactoModel()
        {
            this.Contactos = new List<ContactoIni>();
            this.Datos = new DatosIniciales();
        }
    }

}