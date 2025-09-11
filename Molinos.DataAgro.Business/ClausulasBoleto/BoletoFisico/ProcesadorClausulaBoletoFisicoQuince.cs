using Molinos.DataAgro.Entities.ClausulasBoleto.BoletoFisico;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Autofac.Extras.NLog;

namespace Molinos.DataAgro.Business.ClausulasBoleto.BoletoFisico
{
    public class ProcesadorClausulaBoletoFisicoQuince : ProcesadorClausulaBoletoFisico<ClausulaBoletoFisicoQuince>
    {
        public ProcesadorClausulaBoletoFisicoQuince(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }

        public override ResultadoClausula DevolverClausulas(ClausulaBoletoFisicoQuince clausula)
        {
            var res = new ResultadoClausula();
            res.Texto += "Como proveedor de materias primas agrícolas me comprometo a cumplir con las recomendaciones de Buenas Prácticas Agrícolas leídas en la " +
"página web de Molinos Agro SA http://moaoperaciones.com.ar/Documentacion/GMP%20MOA.pdf";
            return res;
        }
    }
}
