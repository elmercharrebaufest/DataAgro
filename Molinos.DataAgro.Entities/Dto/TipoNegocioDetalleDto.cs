
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{

    public class TipoNegocioDetalleDto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public int TipoNegocioId { get; set; }
        public bool BoletoFisico { get; set; }
        public bool CartaOferta { get; set; }
        public bool Confirma { get; set; }
        public string TipoNegocioDescripcion { get; set; }
    }
}


