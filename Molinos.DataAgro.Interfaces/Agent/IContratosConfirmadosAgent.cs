using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Molinos.DataAgro.Entities.Entities;

namespace Molinos.DataAgro.Interfaces
{
    public interface IContratosConfirmadosAgent
    {
        List<Contrato> ConfirmarContrato(Contrato contrato);
    }
}
