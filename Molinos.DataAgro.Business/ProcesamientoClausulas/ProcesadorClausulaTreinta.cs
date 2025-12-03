using NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Entities.Common.Enums;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorClausulaTreinta : ProcesadorClausula<ClausulaTreinta>
    {
        public ProcesadorClausulaTreinta(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaTreinta clausula)
        {
            //SI EL CONTRATO CORRESPONDE A CANJE y es a fijar

            var res = new ResultadoClausula();
            if (clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && clausula.Basico.Canje == true)
            {
                res.Texto += $"En caso de incumplimiento de la obligación de entrega de la Mercadería por parte del Vendedor, en los términos y condiciones " +
                    $"acordados en este Boleto, el Vendedor quedará constituido automáticamente en mora, sin necesidad de interpelación previa alguna de ningún " +
                    $"tipo, debiendo inmediatamente reintegrar al Comprador la totalidad de los importes correspondientes al Costo del Insumo y Gastos Asociados, " +
                    $"dentro de los 15 (quince) días posteriores al vencimiento del Plazo de Entrega. El Vendedor deberá abonar al Comprador las sumas " +
                    $"correspondientes al Costo del Insumo y los Gastos Asociados mediante transferencia o depósito bancario en la cuenta bancaria que el Comprador " +
                    $"indique al Vendedor, a tal efecto";
            }

            return res;
        }
    }
}
