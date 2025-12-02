using NLog;
using Molinos.DataAgro.Entities.ClausulasBoleto.CartaOferta;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;

namespace Molinos.DataAgro.Business.ClausulasBoleto.CartaOferta
{
    public class ProcesadorClausulaCartaOfertaDoce : ProcesadorClausulaCartaOferta<ClausulaCartaOfertaDoce>
    {
        public ProcesadorClausulaCartaOfertaDoce(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }

        public override ResultadoClausula DevolverClausulas(ClausulaCartaOfertaDoce clausula)
        {
            var res = new ResultadoClausula();
            res.Texto += "Como proveedor de materias primas agrícolas me comprometo a cumplir con las recomendaciones de Buenas Prácticas Agrícolas leídas en la " +
              "página web de Molinos Agro SA http://moaoperaciones.com.ar/Documentacion/GMP%20MOA.pdf";
            return res;
        }
    }

}
