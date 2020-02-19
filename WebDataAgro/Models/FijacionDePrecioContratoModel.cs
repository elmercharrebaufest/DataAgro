using Molinos.DataAgro.Entities.Dto;

namespace WebDataAgro.Models
{
    public class FijacionDePrecioContratoModel : Resultado
    {
        
        public DatosIniAbmFijacionDePrecioContrato Datos { get; set; }

        public FijacionDePrecioContratoModel()
        {
            
            this.Datos = new DatosIniAbmFijacionDePrecioContrato();
        }
    }
}
