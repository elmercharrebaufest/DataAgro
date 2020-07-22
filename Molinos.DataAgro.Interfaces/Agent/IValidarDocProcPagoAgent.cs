using Molinos.DataAgro.Entities.Dto;

namespace Molinos.DataAgro.Interfaces
{
    public interface IValidarDocProcPagoAgent
    {
        string ValidarEstado(string contratoSap, string fijacion);
    }
}