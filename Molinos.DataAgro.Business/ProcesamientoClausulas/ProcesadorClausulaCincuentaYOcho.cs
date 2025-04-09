using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;

namespace Molinos.DataAgro.Business.ProcesamientoClausulas
{
    public class ProcesadorClausulaCincuentaYOcho : ProcesadorClausula<ClausulaCincuentaYOcho>
    {
        public ProcesadorClausulaCincuentaYOcho(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto) : base(repositorio, log, estadoBoleto)
        {
        }
        public override ResultadoClausula DevolverClausulas(ClausulaCincuentaYOcho clausula)
        {
            var res = new ResultadoClausula();
            if (clausula.Basico.CorredorId > 0 || clausula.Basico.CD == true)
            {
                res.Texto += "En caso de incumplimiento, podrá ejecutarse la obligación mediante la entrega de la mercadería objeto del CD o, en su defecto, el importe necesario para poder adquirir la misma cantidad de mercadería objeto del boleto a la fecha de vencimiento del plazo de entrega.";
            }
            return res;
        }
    }
}
