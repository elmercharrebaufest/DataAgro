using System;

namespace Molinos.DataAgro.Entities.Dto
{
    public class TokenDto
    {
        public long Cuit { get; set; }
        public string NombreUsuario { get; set; }
        public string Url { get; set; }
        public DateTime Vencimiento { get; set; }
        public string Error { get; set; }
    }
}


