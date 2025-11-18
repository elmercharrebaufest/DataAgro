using NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Entities.Common.Enums;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorClausulaTreintaYDos : ProcesadorClausula<ClausulaTreintaYDos>
    {
        public ProcesadorClausulaTreintaYDos(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaTreintaYDos clausula)
        {
            //SI EL CONTRATO CORRESPONDE A CANJE y es a fijar

            var res = new ResultadoClausula();
            if (clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && clausula.Basico.Canje == true)
            {
                res.Texto += $"Se considerarán Gastos Asociados a la operación todas aquellas sumas en las que hubiere incurrido el Comprador por los siguientes " +
                    $"conceptos, los que conforman una enumeración meramente enunciativa: Acondicionamiento, secada, impuesto de sellos, gastos de flete y traslado, " +
                    $"pago de obleas, intereses moratorios, intereses punitorios, multas, etc., y en general todos aquellos gastos en los que hubiere incurrido para " +
                    $"llevar a cabo la presente operación.";
            }
            return res;
        }
    }
}
