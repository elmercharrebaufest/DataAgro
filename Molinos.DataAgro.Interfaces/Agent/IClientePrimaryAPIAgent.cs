using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface IClientePrimaryAPIAgent
    {
        TokenPrimary ReuseToken();
        List<AgenteCompra> ObtenerNegocios(DateTime fecha);
        MarketDataResult ObtenerCotizacion(DateTime fecha);
    }
}