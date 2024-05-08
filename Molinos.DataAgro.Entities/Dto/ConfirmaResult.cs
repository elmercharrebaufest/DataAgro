using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public class ConfirmaResult : Resultado
    {
        public Confirma confirma { get; set; }
        public List<Confirma> confirmas { get; set; }
        public List<ConfirmaGeneradoDto> confirmasGenerados { get; set; }

        public ConfirmaResult()
        {
            confirmas = new List<Confirma>();
            confirmasGenerados = new List<ConfirmaGeneradoDto>();
        }
    }
}
