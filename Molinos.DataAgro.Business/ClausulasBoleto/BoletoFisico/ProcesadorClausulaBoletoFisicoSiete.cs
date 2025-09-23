using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.ClausulasBoleto.BoletoFisico;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;

namespace Molinos.DataAgro.Business.ClausulasBoleto.BoletoFisico
{
    public class ProcesadorClausulaBoletoFisicoSiete : ProcesadorClausulaBoletoFisico<ClausulaBoletoFisicoSiete>
    {
        public ProcesadorClausulaBoletoFisicoSiete(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }

        public override ResultadoClausula DevolverClausulas(ClausulaBoletoFisicoSiete clausula)
        {
            var res = new ResultadoClausula();
            if (clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && 
                clausula.Basico.Moneda == "USD" &&
                clausula.Basico.ClasificacionDescripcion?.ToString().ToUpper() == "PRODUCTOR"
                )
            {
                // NEGOCIOS FIJOS EN DOLARES
                res.Texto += "Las Partes acuerdan que la obligación será pagadera en pesos argentinos al tipo de cambio comprador publicado por el Banco de la Nación Argentina.";
            }
            else if (clausula.Basico.TipoDeCambioId == (int)EnumTipoDeCambio.BLEND)
            {
                //SI EL CAMPO CODIGO_TC ES 04
                res.Texto += "Las Partes acuerdan que la obligación será pagadera en pesos argentinos al tipo de cambio publicado por el MATBA ROFEX SA denominado \"índice Dólar Exportación Matba Rofex\", " +
                    "del día de la pesificación, siempre que la comunicación de pesificación sea informada antes de las 13.00 horas. De comunicarse fuera del horario mencionado, se tomará el tipo de cambio del día siguiente al de la comunicación. " +
                    "En caso de no existir el tipo de cambio denominado \"índice Dólar Exportación Matba Rofex\", la obligación será pagadera en pesos argentinos al tipo de cambio aplicable para la liquidación de divisas provenientes de la exportación del producto objeto de la presente oferta/boleto.";
            }
            return res;
        }
    }
}
