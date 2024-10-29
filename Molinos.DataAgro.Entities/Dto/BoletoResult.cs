using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class BoletoResult : Resultado
    {
        public Boleto Boleto { get; set; }
        public List<Boleto> BoletosGenerados { get; set; }
        public List<BoletoDto> BoletosDto { get; set; }

        public BoletoResult()
        {
            BoletosGenerados = new List<Boleto>();
            BoletosDto = new List<BoletoDto>();
        }
    }
}
