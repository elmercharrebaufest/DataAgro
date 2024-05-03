using System.Collections.Generic;
using Molinos.DataAgro.Entities.Entities;
using System;

namespace Molinos.DataAgro.Interfaces
{
    public interface IFinalizarContratoAgent
    {
        string Finalizar(Contrato contrato, List<DescuentoBonificacion> descuentoBonificacion, List<Calidad> calidad);
        string DevolverTipoCambioSAP(int tipoNegocioId, string monedaId, int? tipoAgenteCompraId, DateTime fecha, bool? modifica = false);
    }
}