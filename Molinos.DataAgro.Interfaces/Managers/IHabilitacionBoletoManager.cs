using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{

    public interface IHabilitacionBoletoManager
    {
        List<TipoNegocioDetalleDto> ListarTipoNegociosDetalle();
        void ActualizarTipoNegocioDetalle(List<TipoNegocioDetalleDto> tipoDetalle);
        Resultado AgregarHabilitacionBoleto(TipoNegocioDetalleDto tipoDetalle);
    }
}