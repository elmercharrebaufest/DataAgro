using Molinos.DataAgro.Entities.Entities;
using System;

namespace Molinos.DataAgro.Entities.Dto
{
    public class HabilitacionPagoDiferidoDto
    {
        public int Id { get; set; }
        public int MaterialId { get; set; }
        public string Material { get; set; }
        public int TipoNegocioId { get; set; }
        public string TipoNegocio { get; set; }
        public int CantidadDia { get; set; }
        public decimal Tasa { get; set; }
        public DateTime DesdeVigencia { get; set; }
        public DateTime HastaVigencia { get; set; }
        public bool Habilitado { get; set; }
    }
}
