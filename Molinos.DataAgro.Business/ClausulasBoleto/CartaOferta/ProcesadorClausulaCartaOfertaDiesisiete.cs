using NLog;
using Molinos.DataAgro.Entities.ClausulasBoleto.CartaOferta;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;

namespace Molinos.DataAgro.Business.ClausulasBoleto.CartaOferta
{
    public class ProcesadorClausulaCartaOfertaDiesisiete : ProcesadorClausulaCartaOferta<ClausulaCartaOfertaDiesisiete>
    {
        public ProcesadorClausulaCartaOfertaDiesisiete(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }

        public override ResultadoClausula DevolverClausulas(ClausulaCartaOfertaDiesisiete clausula)
        {
            var res = new ResultadoClausula();
            return res;
        }
    }

}
