using Molinos.DataAgro.Entities.Entities;

namespace Molinos.DataAgro.Interfaces
{
    public interface IValidarLiquidacionParcialAgent
    {
        string ValidarLiquidacionParcial(FijacionDePrecioContrato fijacion);
    }
}