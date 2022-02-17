using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public class BoletoResult : Resultado
    {
        public Boleto boleto { get; set; }
        public List<Boleto> boletos { get; set; }
        public List<BoletoGeneradoDto> boletosGenerados { get; set; }

        public BoletoResult()
        {
            boletos = new List<Boleto>();
            boletosGenerados = new List<BoletoGeneradoDto>();
        }
    }
}
