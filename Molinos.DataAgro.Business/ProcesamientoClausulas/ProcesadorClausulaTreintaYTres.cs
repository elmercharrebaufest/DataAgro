using NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Entities.Common.Enums;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorClausulaTreintaYTres : ProcesadorClausula<ClausulaTreintaYTres>
    {
        public ProcesadorClausulaTreintaYTres(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaTreintaYTres clausula)
        {
            //SI EL CONTRATO CORRESPONDE A CANJE y es a fijar

            var res = new ResultadoClausula();
            if (clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && clausula.Basico.Canje == true)
            {
                res.Texto += $"Los Gastos Asociados deberán ser cancelados por el Vendedor mediante la entrega de la cantidad de Mercadería necesaria para cancelar " +
                    $"el monto que dichos Gastos Asociados representen, teniendo en cuenta que la Mercadería será valuada en los términos establecidos anteriormente.";
            }

            return res;
        }
    }
}
