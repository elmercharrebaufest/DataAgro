using NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;


namespace Molinos.DataAgro.Business.ProcesamientoClausulas
{
    public class ProcesadorClausulaCincuentaYNueve : ProcesadorClausula<ClausulaCincuentaYNueve>
    {
        public ProcesadorClausulaCincuentaYNueve(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto) : base(repositorio, log, estadoBoleto)
        {
        }
        public override ResultadoClausula DevolverClausulas(ClausulaCincuentaYNueve clausula)
        {
            var res = new ResultadoClausula();
            if (clausula.Basico.MercsDeposito == true)
            {
                res.Texto += "El total o parte de la mercadería objeto del presente contrato ya se encuentra descargada.";
            }
            return res;
        }
    }
}
