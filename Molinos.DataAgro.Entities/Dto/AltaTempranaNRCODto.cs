using System;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class AltaTempranaNRCODto
    {
        public string AltaTemprana { get; set; }
        public string FechaActualizacion { get; set; }
        public string Nosis { get; set; }
        public string Bolsa { get; set; }
        public Ruca Ruca { get; set; }
        public string Carta { get; set; }
        public string Mensaje { get; set; }
    }
    public partial class Ruca
    {
        public string Otros { get; set; }
        public string Acopiador { get; set; }
    }
}



