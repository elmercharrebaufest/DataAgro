using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Interfaces;

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

            return res;
        }
    }
}
