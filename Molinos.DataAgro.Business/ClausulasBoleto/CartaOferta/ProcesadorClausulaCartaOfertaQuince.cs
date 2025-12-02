using NLog;
using Molinos.DataAgro.Entities.ClausulasBoleto.CartaOferta;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;

namespace Molinos.DataAgro.Business.ClausulasBoleto.CartaOferta
{
    public class ProcesadorClausulaCartaOfertaQuince : ProcesadorClausulaCartaOferta<ClausulaCartaOfertaQuince>
    {
        public ProcesadorClausulaCartaOfertaQuince(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }

        public override ResultadoClausula DevolverClausulas(ClausulaCartaOfertaQuince clausula)
        {
            var res = new ResultadoClausula();
            res.Texto += "En caso de aceptación, las partes acuerdan y aceptan que en lo sucesivo, todos los documentos que tengan relación con la presente Oferta podrán suscribirse " +
                         "mediante firma digital y/o electrónica, la que tendrá plena validez para las Partes.";
            return res;
        }
    }

}
