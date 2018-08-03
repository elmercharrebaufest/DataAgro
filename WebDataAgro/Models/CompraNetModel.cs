using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;

namespace WebDataAgro.Models
{

    public class DatosIniCompraNetModel : Resultado
    {
        
        public DatosIniCompraNet Datos { get; set; }

        public DatosIniCompraNetModel()
        {
            this.Datos = new DatosIniCompraNet(); 
        }
    }
}