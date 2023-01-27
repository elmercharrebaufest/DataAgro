using System;

namespace Molinos.DataAgro.Entities.Dto
{
    public class EnviarCapacidadProductivaSAPDto
    {
        public int Id { get; set; }
        public string Cuit { get; set; }
        public string Material { get; set; }
        public string Campania { get; set; }
        public decimal Cantidad { get; set; }
        public string UnidadMedida { get; set; }
        public decimal Porcentaje { get; set; }
        public string MaterialDescripcion { get; set; }
    }
}
