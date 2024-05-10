using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Interfaces;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorClausulaDos : ProcesadorClausula<ClausulaDos>
    {
        public ProcesadorClausulaDos(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaDos clausula)
        {
            // CLAUSULA SIEMPRE PRESENTE

            var res = new ResultadoClausula();
            res.Texto += $"Las entregas y recibos se efectuarán desde el {clausula.Basico.FechaDesde.GetValueOrDefault():dd'/'MM'/'yyyy} hasta el {clausula.Basico.FechaHasta.GetValueOrDefault():dd'/'MM'/'yyyy}";
            if (clausula.Basico.MercsDeposito == true)
            {
                res.Texto += $" habiendo a su vez mercadería descargada";
            }
            res.Texto += $", haciéndose el recibo por el comprador en planta {ReemplazarSanLorenzo(clausula.Basico.DestinoDescripcion)}, Localidad {clausula.Basico.DestinoLocalidad}, " +
                $"de Provincia de {clausula.Basico.DestinoProvincia}. Queda establecido que toda tasa contribución, impuesto provincial y/o municipal que grave " +
                $"la presente operación será a cargo de la parte vendedora.";

            return res;
        }

        private string ReemplazarSanLorenzo(string palabra)
        {
            return palabra.Equals("San Lorenzo") ? "San Lorenzo o Ricardone" : palabra;
        }
    }
}
