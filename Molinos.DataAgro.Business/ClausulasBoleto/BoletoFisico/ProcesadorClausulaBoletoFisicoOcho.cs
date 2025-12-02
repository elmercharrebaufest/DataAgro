using Molinos.DataAgro.Entities.ClausulasBoleto.BoletoFisico;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using NLog;

namespace Molinos.DataAgro.Business.ClausulasBoleto.BoletoFisico
{
    public class ProcesadorClausulaBoletoFisicoOcho : ProcesadorClausulaBoletoFisico<ClausulaBoletoFisicoOcho>
    {
        public ProcesadorClausulaBoletoFisicoOcho(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }

        public override ResultadoClausula DevolverClausulas(ClausulaBoletoFisicoOcho clausula)
        {
            var res = new ResultadoClausula();
            res.Texto = "El presente contrato sólo es transferible con el expreso consentimiento de Molinos Agro SA.";
            return res;
        }
    }
}
