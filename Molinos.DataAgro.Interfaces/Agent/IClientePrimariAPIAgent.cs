using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Interfaces
{
    public interface IClientePrimariAPIAgent
    {
        TokenPrimary ObtenerToken();
        List<AgenteCompra> ObtenerNegocios();
        MarketDataResult ObtenerCotizacion(DateTime fecha);


    }
}