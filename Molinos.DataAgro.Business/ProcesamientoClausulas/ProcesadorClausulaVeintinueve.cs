using NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Entities.Common.Enums;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorClausulaVeintinueve : ProcesadorClausula<ClausulaVeintinueve>
    {
        public ProcesadorClausulaVeintinueve(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaVeintinueve clausula)
        {
            //SI EL CONTRATO CORRESPONDE A CANJE y es a fijar

            var res = new ResultadoClausula();
            if (clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && clausula.Basico.Canje == true)
            {
                res.Texto += $"La entrega de la Mercadería no es excusable bajo ningún supuesto, no pudiendo alegarse causales de caso fortuito o fuerza mayor. " +
                    $"La obligación de entrega de la Mercadería por parte del Vendedor se reputará cumplida -únicamente- con (i) la entrega total de la Mercadería, " +
                    $"necesaria para cancelar el Costo del Insumo y los Gastos Asociados, y (ii) la Mercadería entregada deberá reunir y cumplir las condiciones de " +
                    $"calidad solicitadas.";
            }

            return res;
        }
    }
}
