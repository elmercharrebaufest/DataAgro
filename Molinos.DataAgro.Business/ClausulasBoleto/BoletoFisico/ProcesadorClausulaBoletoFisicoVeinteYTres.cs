using NLog;
using Molinos.DataAgro.Entities.ClausulasBoleto.BoletoFisico;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;

namespace Molinos.DataAgro.Business.ClausulasBoleto.BoletoFisico
{
    public class ProcesadorClausulaBoletoFisicoVeinteYTres : ProcesadorClausulaBoletoFisico<ClausulaBoletoFisicoVeinteYTres>
    {
        public ProcesadorClausulaBoletoFisicoVeinteYTres(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }

        public override ResultadoClausula DevolverClausulas(ClausulaBoletoFisicoVeinteYTres clausula)
        {
            var res = new ResultadoClausula();
            return res;
        }
    }
}
