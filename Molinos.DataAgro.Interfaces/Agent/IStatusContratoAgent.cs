using Molinos.DataAgro.Entities.Dto;

namespace Molinos.DataAgro.Interfaces
{
    public interface IStatusContratoAgent
    {
        string ValidarEstado(string contratoSap);
    }
}