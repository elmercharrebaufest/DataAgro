using NLog;
using Molinos.DataAgro.Entities.ClausulasBoleto.CartaOferta;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;

namespace Molinos.DataAgro.Business.ClausulasBoleto.CartaOferta
{
    public class ProcesadorClausulaCartaOfertaSeis : ProcesadorClausulaCartaOferta<ClausulaCartaOfertaSeis>
    {
        public ProcesadorClausulaCartaOfertaSeis(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }

        public override ResultadoClausula DevolverClausulas(ClausulaCartaOfertaSeis clausula)
        {
            var res = new ResultadoClausula();
            if (clausula.Basico.Moneda == "USD")
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
