using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class ResearchCondicionCultivoDto
    {
        public int CondicionCultivoId { get; set; }
        public int MaterialId { get; set; }
        public int CondicionId { get; set; }
        public int Valor { get; set; }
    }
}
