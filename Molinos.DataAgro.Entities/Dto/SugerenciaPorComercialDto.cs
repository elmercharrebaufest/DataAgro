using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public class SugerenciaPorComercialDto
    {
        public int Id { get; set; }

        public int ComercialId { get; set; }
        public DateTime Fecha { get; set; }
        public int CentroId { get; set; }
        public int MaterialId { get; set; }
        public int Total { get; set; }
    }
}
