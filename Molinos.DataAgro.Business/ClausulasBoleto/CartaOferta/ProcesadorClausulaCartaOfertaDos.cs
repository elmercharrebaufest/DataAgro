using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.ClausulasBoleto.CartaOferta;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;

namespace Molinos.DataAgro.Business.ClausulasBoleto.CartaOferta
{
    public class ProcesadorClausulaCartaOfertaDos : ProcesadorClausulaCartaOferta<ClausulaCartaOfertaDos>
    {
        public ProcesadorClausulaCartaOfertaDos(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }

        public override ResultadoClausula DevolverClausulas(ClausulaCartaOfertaDos clausula)
        {
            var res = new ResultadoClausula();

            res.Texto += $"La entrega y recibo de la mercadería se efectuará desde el {clausula.Basico.FechaDesde.GetValueOrDefault():dd'.'MM'.'yyyy} hasta el {clausula.Basico.FechaHasta.GetValueOrDefault():dd'.'MM'.'yyyy},";

            if (clausula.Basico.MercsDeposito == true)
            {
                res.Texto += $" habiendo a su vez mercadería descargada,";
            }

            string destino = clausula.Basico.DestinoDescripcion;
            res.Texto += $" haciéndose en: planta {(destino.Equals("San Lorenzo") ? "San Lorenzo o Ricardone" : destino)}" +
                $"{(clausula.Basico.DestinoCodigoSap.Equals("1068") ? "" : $", localidad {clausula.Basico.DestinoLocalidad}, provincia de {clausula.Basico.DestinoProvincia}")}" +
                $". Queda establecido que toda tasa contribución, impuesto provincial y/o municipal que grave la presente operación será a cargo de la parte vendedora.";

            return res;
        }
    }

}
