using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Interfaces;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorClausulaTrece : ProcesadorClausula<ClausulaTrece>
    {
        public ProcesadorClausulaTrece(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaTrece clausula)
        {
            //SI ES OPERACIÓN DIRECTA Y NO SE TRATA DE CANJE

            var res = new ResultadoClausula();

            if (clausula.Basico.CorredorId == 0 && clausula.Basico.Canje != true)
            {
                res.Texto += "Por medio de la presente la VENDEDORA autoriza en forma expresa e irrevocable a la COMPRADORA a compensar en los términos " +
                    "de los artículos 921, siguientes y concordantes del Código Civil y Comercial de la Nación, la totalidad de los CRÉDITOS que pudieran " +
                    "llegar a existir a su favor, en virtud de la venta de agroinsumos, subproductos, servicios y/o gastos derivados de la operación comercial " +
                    "con la VENDEDORA.";
            }

            return res;
        }
    }
}
