using Molinos.DataAgro.Entities.Entities;

namespace Molinos.DataAgro.Interfaces
{
    public interface IAnularFijacionVirtualAgent
    {
        string AnularFijacionVirtual(FijacionDePrecioContrato fijacion, string comercial);
    }
}