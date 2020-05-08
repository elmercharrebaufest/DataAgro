using System;

namespace Molinos.DataAgro.Interfaces
{
    public interface ITipoDeCambioAgent
    {
        decimal TraerTipoDeCambio(DateTime? fecha);
    }
}