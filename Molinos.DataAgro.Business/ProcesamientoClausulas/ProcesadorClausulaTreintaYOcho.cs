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
    public class ProcesadorClausulaTreintaYOcho : ProcesadorClausula<ClausulaTreintaYOcho>
    {
        public ProcesadorClausulaTreintaYOcho(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaTreintaYOcho clausula)
        {
            var res = new ResultadoClausula();
            if (clausula.Basico.TipoNegocioId == 2 && clausula.Basico.CondicionalContratoId > 0)
            {
                res.Texto += $"Las partes se comprometen a realizar un boleto de compraventa de {clausula.Basico.Material } por el mismo volumen de toneladas que " +
                    $"el presente boleto, bajo la condición a precio en {clausula.Basico.CondicionalPosicion } a {clausula.Basico.CondicionalPrecio } " +
                    $"{clausula.Basico.CondicionalMonedaDescripcion } si el ajuste de soja MATBA {clausula.Basico.CondicionalPosicion} supera dicho valor el " +
                    $"{clausula.Basico.CondicionalFechaFormateado } (en adelante la Condición Precedente).  En caso de que la Condición Precedente se cumpla y " +
                    $"el vendedor rechace la generación del nuevo boleto comprometido, las partes acuerdan que el vendedor deberá abonar en concepto de multa " +
                    $"el diferencial de precio resultante de la condición precedente es decir {clausula.Basico.CondicionalPrecio} {clausula.Basico.CondicionalMonedaDescripcion }" +
                    $"y el precio de ajuste MATBA de {clausula.Basico.Material } {clausula.Basico.CondicionalPosicion }";
            }
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
