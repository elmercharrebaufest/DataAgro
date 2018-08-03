using Molinos.DataAgro.Entities.Dto;

namespace WebDataAgro.Models
{
    public class ContratoModel_prueba : Resultado
    {
        public DatosIniContrato Datos { get; set; }

        public ContratoModel_prueba()
        {
            this.Datos = new DatosIniContrato();
        }
    }
}
