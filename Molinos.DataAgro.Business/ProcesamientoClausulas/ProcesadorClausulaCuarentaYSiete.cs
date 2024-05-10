using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Interfaces;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorClausulaCuarentaYSiete : ProcesadorClausula<ClausulaCuarentaYSiete>
    {
        public ProcesadorClausulaCuarentaYSiete(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaCuarentaYSiete clausula)
        {
            //CLAUSULA SIEMPRE PRESENTE

            var res = new ResultadoClausula();
            res.Texto += "Los firmantes acuerdan y aceptan que en lo sucesivo, todos los documentos que tengan relación con el presente contrato podrán suscribirse " +
                  "mediante firma digital y/o electrónica, la que tendrá plena validez para las Partes.";
            return res;
        }

    }
}
