using NLog;
using Molinos.DataAgro.Entities.ClausulasBoleto.CartaOferta;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;

namespace Molinos.DataAgro.Business.ClausulasBoleto.CartaOferta
{

    public class ProcesadorClausulaCartaOfertaSiete : ProcesadorClausulaCartaOferta<ClausulaCartaOfertaSiete>
    {
        public ProcesadorClausulaCartaOfertaSiete(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }

        public override ResultadoClausula DevolverClausulas(ClausulaCartaOfertaSiete clausula)
        {
            var res = new ResultadoClausula();
            res.Texto = "El presente contrato sólo es transferible con el expreso consentimiento de Molinos Agro SA.";
            return res;
        }
    }

}
