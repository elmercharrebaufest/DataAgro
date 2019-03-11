using Molinos.DataAgro.Entities.Common.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class FasonDto
    {
        [Key]
        public int FasonId { get; set; }
        public int TipoFasonId { get; set; }
        public int FasoneroId { get; set; }
        public int MaterialId { get; set; }
        public int CampanaId { get; set; }
        public decimal Precio { get; set; }
        public string MonedaId { get; set; }
        public double Cantidad { get; set; }
        public string Posicion { get; set; }        
        public DateTime Fecha { get; set; }
        public int ComercialId { get; set; }
        public int EstadoId { get; set; }           
        public string Fasonero { get; set; }
        public int? ComercialCreadorId { get; set; }
    }
}

