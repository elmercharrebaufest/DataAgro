
using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class DatosContratoDto
    {
        public List<CalidadDto> Calidades { get; set; }
        public List<DescuentoBonificacionDto> DescuentosBonificaciones { get; set; }
        public List<PrecioPactadosDto> Precios { get; set; }
       
    }
}



