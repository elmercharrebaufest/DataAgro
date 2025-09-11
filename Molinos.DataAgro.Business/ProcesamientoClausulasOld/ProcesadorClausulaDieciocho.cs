using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Entities.Common.Enums;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorClausulaDieciocho : ProcesadorClausula<ClausulaDieciocho>
    {
        public ProcesadorClausulaDieciocho(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaDieciocho clausula)
        {
            var res = new ResultadoClausula();
            if (clausula.Basico.BoletoId != (int)EnumBoletoCompraNet.CARTA_OFERTA)
            {
                if (clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && (clausula.Basico.CorredorId > 0 || clausula.Basico.ClasificacionId == (int)EnumClasificacionCompraNet.Acopiador))
                {
                    //res.Texto = "En caso de que el Acopiador/Corredor no proceda a liquidar la mercadería dentro de las 72 horas corridas desde que la misma fuera entregada, aplicada y fijada, las Partes acuerdan que quedará a opción del Comprador determinar el día que se tomará válido para establecer el tipo de cambio a utilizar en los términos dispuestos en el presente boleto.";
                }
            }
            return res;
        }
    }
}
