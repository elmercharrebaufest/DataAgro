using Molinos.DataAgro.Entities.Dto;

namespace Molinos.DataAgro.Interfaces
{
    public interface IStatusContratoAgent
    {
        EstadoSAPDto ValidarEstado(string contratoSap);
    }
}