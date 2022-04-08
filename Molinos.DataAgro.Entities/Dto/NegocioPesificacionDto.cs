
using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class NegocioPesificacionDto
    {
        public int Id { get; set; }
        public bool? Excepcion { get; set; }
        public DateTime? FechaExcepcion { get; set; }
        public DateTime? FechaIntruccion { get; set; }
        public DateTime? FechaEnvio { get; set; }
        public int? ComercialId { get; set; }
        public int NegocioId { get; set; }
    }  
}


