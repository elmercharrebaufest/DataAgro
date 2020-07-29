using System.Collections.Generic;
using Molinos.DataAgro.Entities.Dto;

namespace WebDataAgro.Models
{

    public class ResultIniContactoModel : Resultado
    {
        public ResultIniContacto Contactos { get; set; }
        public CampañaHome Campaña { get; set; }
        public ObjetivoHome Objetivo { get; set; }
        public DatosIniciales Datos { get; set; }
        public List<CompraDto> Detalle { get; internal set; }

        public ResultIniContactoModel()
        {
            this.Contactos = new ResultIniContacto();
            this.Datos = new DatosIniciales();
        }
    }

}