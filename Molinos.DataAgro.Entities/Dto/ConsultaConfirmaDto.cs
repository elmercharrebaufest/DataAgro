using Kendo.DynamicLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public class ConsultaConfirmaDto
    {
        public bool EsSoloPendientes { get; set; }
        public DataSourceRequest Filtros { get; set; }
    }
}
