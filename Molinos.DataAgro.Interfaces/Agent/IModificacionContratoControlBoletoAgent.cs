using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Interfaces.Agent
{
    public interface IModificacionContratoControlBoletoAgent
    {
        string ModificarContrato(string Clasificacion, string Contrato, string Cosecha, string Fecha, string Hora, string Procedencia, string Provincia, string Usuario);
    }
}
