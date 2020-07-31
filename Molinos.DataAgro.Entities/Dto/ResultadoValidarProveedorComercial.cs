using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class ResultadoValidarProveedorComercial
    {
        public ResultadoValidarProveedorComercial()
        {
            ListaErrores = new List<ErrorMessage>();
        }

        public List<ErrorMessage> ListaErrores { get; set; }
        
        public bool HayError { get; set; }
        public int ComercialId { get; set; }
        public string Apellido { get; set; }
        public string Nombres { get; set; }
        public string Mail { get; set; }
        public List<string> ProveedorMails { get; set; }
        public int? ProveedorId { get; set; }
        public string ProveedorRazonSocial { get; set; }
    }
}
