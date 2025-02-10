using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Entities.Common.Enums;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorClausulaCuarentaYSeis : ProcesadorClausula<ClausulaCuarentaYSeis>
    {
        public ProcesadorClausulaCuarentaYSeis(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaCuarentaYSeis clausula)
        {
            var res = new ResultadoClausula();
            if (clausula.Basico.TipoDeCambioId == (int)EnumTipoDeCambio.BNA && clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && clausula.Basico.MonedaId == "USDM ")
            {
                // NEGOCIOS FIJOS EN DOLARES
                res.Texto += "Las Partes acuerdan que la obligación será pagadera en pesos argentinos al tipo de cambio comprador publicado por el Banco de la Nación Argentina, salvo en el caso en que dicho tipo de cambio no sea el que resulte aplicable para la liquidación de divisas proveniente de la exportación de granos, en cuyo caso se tomará este último. En ese sentido, las Partes establecen de común acuerdo que la única forma de pago válida de las liquidaciones parciales y finales es la que se plasma en el presente instrumento, y la que rige según usos y costumbres del mercado de granos excluyendo cualquier otra que se pretenda o intente aplicar que atente contra el principio de autonomía de voluntad de las partes y de dichos usos y costumbres.";
            }
            else if ((clausula.Basico.TipoDeCambioId == (int)EnumTipoDeCambio.BNA && clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR) || clausula.Basico.MonedaId == "USDM " || (string.IsNullOrWhiteSpace(clausula.Basico.MonedaId) && clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR))
            {
                //SI EL CAMPO CODIGO_TC ES 02(BNA) mientras sea negocio de Tipo A Fijar  O  tambien se aplica a Negocios con Moneda Dolar
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
