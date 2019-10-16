using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Interfaces
{
    public interface IConfiguracionManager
    {
        Configuracion TraerConfiguraciones();
        Resultado GrabarFechaPesificacionDolarizado(Configuracion oConfiguracion);


    }
}
