using Molinos.DataAgro.Entities.Dto;

namespace Molinos.DataAgro.Interfaces
{
    public interface IEnviarBoletoAgent
    {
        string EnviarBoleto(BoletoDto boleto);
    }
}