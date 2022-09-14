using Molinos.DataAgro.Entities.Entities;

namespace Molinos.DataAgro.Interfaces
{
    public interface IValidarLiquidacionComisionesAgent
    {
        string ValidarLiquidacionComisiones(FijacionDePrecioContrato fijacion);
    }
}