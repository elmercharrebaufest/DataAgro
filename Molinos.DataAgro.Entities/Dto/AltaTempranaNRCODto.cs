using System;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class AltaTempranaNRCODto
    {
        public string AltaTemprana { get; set; }
        public string FechaActualizacion { get; set; }
        public string Nosis { get; set; }
        public string Bolsa { get; set; }
        public string PlanCanje { get; set; }
        public string Consignatario { get; set; }
        public Ruca Ruca { get; set; }
        public string Carta { get; set; }
        public string Mensaje { get; set; }
    }
    public partial class Ruca
    {
        public ValoresRuca Otros { get; set; }
        public ValoresRuca Acopiador { get; set; }
        public string Corredor { get; set; }
    }
    public partial class ValoresRuca
    {
        public string Consignatario { get; set; }
        public string PlanCanje { get; set; }
        public string Directo { get; set; }
    }
}



