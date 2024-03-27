using System;

namespace Molinos.DataAgro.Interfaces
{
    public interface ITipoDeCambioAgent
    {
        decimal TraerTipoDeCambio(DateTime? fecha, string typeOfRate = "M");
        decimal TraerTipoDeCambioMoneda(DateTime? fecha, string moneda, string typeOfRate = "M");
        decimal TraerTipoDeCambioUltimoDiaHabil(DateTime? fecha, string typeOfRate = "M");
    }
}