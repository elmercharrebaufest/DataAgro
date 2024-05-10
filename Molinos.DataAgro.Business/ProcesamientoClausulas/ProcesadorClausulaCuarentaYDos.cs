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
            //res.Texto += "En atención a lo establecido precedentemente, una vez levantado el aislamiento social preventivo y obligatorio, el contrato/boleto será entregado en original.";
            return res;
        }
    }
}
