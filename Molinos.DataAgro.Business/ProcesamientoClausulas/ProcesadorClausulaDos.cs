using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using System;
using Humanizer;
using System.Globalization;
using System.Linq;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Entities.Common.Enums;

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
            var res = new ResultadoClausula();  
            res.Texto += $"Las entregas y recibos se efectuarán desde el {clausula.Basico.FechaDesdeFormateado} hasta {clausula.Basico.FechaHastaFormateado} ";
            if (clausula.Basico.MercsDeposito == true)
            {
                res.Texto += $"habiendo a su vez mercadería descargada ";
            }
            res.Texto += $"haciéndose el recibo por el comprador en planta { clausula.Basico.DestinoDescripcion }, Localidad { clausula.Basico.DestinoLocalidad }, " +
                $"de Provincia de { clausula.Basico.DestinoProvincia }. Queda establecido que toda tasa contribución, impuesto provincial y/o municipal que grave " +
                $"la presente operación será a cargo de la parte vendedora. ";

            return res;
        }

        public string DevolverNumeroEnLetras(decimal numero)
        {
            var letras = ((int)Math.Abs(numero)).ToWords(CultureInfo.GetCultureInfo("es-AR")).ToUpper();             
            var fraccion = numero - Math.Floor(numero); 
            if (fraccion > 0)
            {
                letras += $" con {((int)(fraccion * 100)).ToWords(CultureInfo.GetCultureInfo("es-AR")).ToUpper() }";

            }
            return letras;
        }
    }
}
