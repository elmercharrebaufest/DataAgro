using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface IServicioRepositorioScatoAgent
    {
        List<EstablecimientoStockDto> ListarEstablecimientos(string cuitProveedor, string campania);
    }
}