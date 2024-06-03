using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Interfaces;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorClausulaCuarentaYUno : ProcesadorClausula<ClausulaCuarentaYUno>
    {
        public ProcesadorClausulaCuarentaYUno(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaCuarentaYUno clausula)
        {
            //MIENTRAS ESTÉN VIGENTES LOS DNU COVID

            var res = new ResultadoClausula();
            res.Texto += "Dada la circunstancia excepcional que se está viviendo en virtud del dictado del Decreto de Necesidad y Urgencia Nº 297/2020, que estableció " +
            "el aislamiento social preventivo y obligatorio, el contrato se envía en PDF por mail a la Bolsa de Cereales para su registro y consecuente cobro del Impuesto de Sellos.";
            return res;
        }
    }
}
