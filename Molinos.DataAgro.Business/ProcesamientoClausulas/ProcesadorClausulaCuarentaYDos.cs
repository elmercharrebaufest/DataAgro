using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Interfaces;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorClausulaCuarentaYDos : ProcesadorClausula<ClausulaCuarentaYDos>
    {
        public ProcesadorClausulaCuarentaYDos(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaCuarentaYDos clausula)
        {
            // MIENTRAS ESTÉN VIGENTES LOS DNU COVID

            var res = new ResultadoClausula();
            res.Texto += "";
            return res;
        }
    }
}
