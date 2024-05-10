using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Interfaces;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorClausulaCuarentaYTres : ProcesadorClausula<ClausulaCuarentaYTres>
    {
        public ProcesadorClausulaCuarentaYTres(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaCuarentaYTres clausula)
        {
            //CLAUSULA SIEMPRE PRESENTE

            var res = new ResultadoClausula();
            res.Texto += "Como proveedor de materias primas agrícolas me comprometo a cumplir con las recomendaciones de Buenas Prácticas Agrícolas leídas en la " +
                    "página web de Molinos Agro SA http://moaoperaciones.com.ar/Documentacion/GMP%20MOA.pdf";
            return res;
        }
    }
}
