using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Interfaces;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorClausulaCatorce : ProcesadorClausula<ClausulaCatorce>
    {
        public ProcesadorClausulaCatorce(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaCatorce clausula)
        {
            //SI EL NEGOCIO TIENE INDICADO QUE CORRESPONDE A UN PLAN CANJE 

            var res = new ResultadoClausula();
			/*
            if (clausula.Basico.PlanCanje == true)
            {
                res.Texto += "El vendedor declara que la mercadería proviene de un canje o de un pago en especie por lo que de acuerdo a lo estipulado por el Art. 45º de la RG 4310 el presente contrato no se encuentra sujeto a retención de IVA, siempre y cuando se presente la oblea que acredite esta condición.";
            }
			*/
            return res;
        }
    }
}
