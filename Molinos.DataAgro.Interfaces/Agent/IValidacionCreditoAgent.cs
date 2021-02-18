using Molinos.DataAgro.Entities.Dto;

namespace Molinos.DataAgro.Interfaces
{
    public interface IValidacionCreditoAgent
    {
        ValidarCreditoDto ValidarCredito(string cuitProveedor);
    }
}