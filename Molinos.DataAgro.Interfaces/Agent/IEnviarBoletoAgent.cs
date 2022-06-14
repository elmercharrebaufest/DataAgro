using System.Collections.Generic;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;

namespace Molinos.DataAgro.Interfaces
{
    public interface IEnviarBoletoAgent
    {
        string Enviar(BoletoGeneradoDto boleto);
    }
}