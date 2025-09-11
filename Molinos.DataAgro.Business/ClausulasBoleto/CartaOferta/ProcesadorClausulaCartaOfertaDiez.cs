using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.ClausulasBoleto.CartaOferta;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;

namespace Molinos.DataAgro.Business.ClausulasBoleto.CartaOferta
{
    public class ProcesadorClausulaCartaOfertaDiez : ProcesadorClausulaCartaOferta<ClausulaCartaOfertaDiez>
    {
        public ProcesadorClausulaCartaOfertaDiez(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }

        public override ResultadoClausula DevolverClausulas(ClausulaCartaOfertaDiez clausula)
        {
            var res = new ResultadoClausula();
            res.Texto += "El comprador otorgará el cupo con un código alfanumérico que obligatoriamente debe consignarse en el campo observaciones de cada carta de porte. " +
                         "En caso de que el vendedor remita camiones sin poseer cupo para la descarga, el comprador podrá, a su exclusiva opción, proceder a la descarga de los mismos, " +
                         "debiendo en tal caso el vendedor abonar al comprador U$S 10 (DIEZ dólares) por tonelada en concepto de gastos extras por descargas no otorgadas.";
            return res;
        }
    }

}
