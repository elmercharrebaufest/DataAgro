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

            //SI EL CAMPO CODIGO_TC ES 02(BNA) mientras sea negocio de Tipo A Fijar  O  tambien se aplica a Negocios con Moneda Dolar
            if ((clausula.Basico.TipoDeCambioId == (int)EnumTipoDeCambio.BNA && clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR) || clausula.Basico.MonedaId?.Trim() == "USDM")
            {
                res.Texto += "Las Partes acuerdan que la obligación será pagadera en pesos argentinos al tipo de cambio comprador publicado por el Banco de la Nación Argentina.";
            }
            //SI EL CAMPO CODIGO_TC ES 04
            else if (clausula.Basico.TipoDeCambioId == (int)EnumTipoDeCambio.BLEND)
            {
                res.Texto += "Las Partes acuerdan que la obligación será pagadera en pesos argentinos al tipo de cambio publicado por el MATBA ROFEX SA denominado “índice Dólar Exportación Matba Rofex”, " +
                    "del día de la pesificación, siempre que la comunicación de pesificación sea informada antes de las 13.00 horas. De comunicarse fuera del horario mencionado, se tomará el tipo de cambio del día siguiente al de la comunicación. " +
                    "En caso de no existir el tipo de cambio denominado “índice Dólar Exportación Matba Rofex”, la obligación será pagadera en pesos argentinos al tipo de cambio aplicable para la liquidación de divisas provenientes de la exportación del producto objeto de la presente oferta/boleto.";
            }

            return res;
        }
    }
}
