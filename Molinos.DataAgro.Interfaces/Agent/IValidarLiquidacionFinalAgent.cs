using Molinos.DataAgro.Entities.Entities;

namespace Molinos.DataAgro.Interfaces
{
    public interface IValidarLiquidacionFinalAgent
    {
        string ValidarLiquidacionFinal(FijacionDePrecioContrato fijacion);
    }
}