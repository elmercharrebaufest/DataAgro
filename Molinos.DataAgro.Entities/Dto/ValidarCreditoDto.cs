using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class ValidarCreditoDto
    {
        public string Cuit { get; set; }
        public decimal Monto { get; set; }
        public string Moneda { get; set; }
    }

}


