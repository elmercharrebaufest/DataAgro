using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.ClausulasBoleto.BoletoFisico;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;

namespace Molinos.DataAgro.Business.ClausulasBoleto.BoletoFisico
{
    public class ProcesadorClausulaBoletoFisicoDos : ProcesadorClausulaBoletoFisico<ClausulaBoletoFisicoDos>
    {
        public ProcesadorClausulaBoletoFisicoDos(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }

        public override ResultadoClausula DevolverClausulas(ClausulaBoletoFisicoDos clausula)
        {
            var res = new ResultadoClausula();

            res.Texto += $"Las entregas y recibos se efectuarán desde el {clausula.Basico.FechaDesde.GetValueOrDefault():dd'.'MM'.'yyyy} hasta el {clausula.Basico.FechaHasta.GetValueOrDefault():dd'.'MM'.'yyyy},";

            if (clausula.Basico.MercsDeposito == true)
            {
                res.Texto += $" habiendo a su vez mercadería descargada,";
            }

            string destino = clausula.Basico.DestinoDescripcion;
            res.Texto += $" haciéndose el recibo por el recibidor del comprador en planta {(destino.Equals("San Lorenzo") ? "San Lorenzo o Ricardone" : destino)}" +
                $"{(clausula.Basico.DestinoCodigoSap.Equals("1068") ? "" : $", localidad {clausula.Basico.DestinoLocalidad}, provincia de {clausula.Basico.DestinoProvincia}")}" +
                $". Queda establecido que toda tasa contribución, impuesto provincial y/o municipal que grave la presente operación será a cargo de la parte vendedora.";
            return res;
        }
    }
}
