using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{

    public interface IHabilitacionBoletoManager
    {
        List<TipoNegocioDetalleDto> ListarTipoNegociosDetalle();
        List<BoletoCompraNetProvinciaDto> ListarBoletoCompraNetProvincia();
        List<BoletoCompraNetDto> ListarBoletoCompraNet();
        void ActualizarTipoNegocioDetalle(List<TipoNegocioDetalleDto> tipoDetalle);
        Resultado AgregarTipoNegocioDetalle(TipoNegocioDetalleDto tipoDetalle);
        Resultado AgregarBoletoCompraNetProvincia(BoletoCompraNetProvinciaDto boletoProvincia);
        Resultado EliminarBoletoCompraNetProvincia(int id);
    }
}