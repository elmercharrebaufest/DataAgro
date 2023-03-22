using System;

namespace Molinos.DataAgro.Interfaces
{
    public interface ITipoDeCambioAgent
    {
        decimal TraerTipoDeCambio(DateTime? fecha);
        decimal TraerTipoDeCambioMoneda(DateTime? fecha, string moneda);
        decimal TraerTipoDeCambioUltimoDiaHabil(DateTime? fecha);
    }
}