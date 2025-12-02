using NLog;
using Molinos.DataAgro.Entities.ClausulasBoleto.BoletoFisico;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System.Globalization;
using System.Linq;

namespace Molinos.DataAgro.Business.ClausulasBoleto.BoletoFisico
{
    public class ProcesadorClausulaBoletoFisicoDoce : ProcesadorClausulaBoletoFisico<ClausulaBoletoFisicoDoce>
    {
        public ProcesadorClausulaBoletoFisicoDoce(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }

        public override ResultadoClausula DevolverClausulas(ClausulaBoletoFisicoDoce clausula)
        {
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
