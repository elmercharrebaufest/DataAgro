using NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Interfaces;
using System.Globalization;
using System;
using Humanizer;
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
            //EL CONTRATO CORRESPONDE A NEGOCIOS CONDICIONALES Y EL CONTRATO PRECESOR ES UN CONTRATO A PRECIO, LA CLÁUSULA SE AGREGA EN EL PRIMER CONTRATO

            var res = new ResultadoClausula();
            if (clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO && clausula.Basico.CondicionalContratoId > 0 && clausula.Basico.CondicionalPrecio.HasValue)
            {
                res.Texto += $"Las partes se comprometen a realizar un boleto de compraventa de {clausula.Basico.Material} por el mismo volumen de toneladas que " +
                    $"el presente boleto, bajo la condición a precio en {clausula.Basico.CondicionalPosicion} a {clausula.Basico.CondicionalMonedaDescripcion} {clausula.Basico.CondicionalPrecio?.ToString("N", new CultureInfo("es-AR"))} " +
                    $"({DevolverNumeroEnLetras(clausula.Basico.CondicionalPrecio.Value)}) si el ajuste de soja MATBA {clausula.Basico.CondicionalPosicion} supera dicho valor el {clausula.Basico.CondicionalFechaFormateado} (en adelante la Condición Precedente). " +
                    $"En caso de que la Condición Precedente se cumpla y el vendedor rechace la generación del nuevo boleto comprometido, las partes acuerdan que el vendedor deberá abonar en concepto de multa " +
                    $"el diferencial de precio resultante de la condición precedente es decir {clausula.Basico.CondicionalMonedaDescripcion} {clausula.Basico.CondicionalPrecio?.ToString("N", new CultureInfo("es-AR"))} " +
                    $"y el precio de ajuste MATBA de {clausula.Basico.Material} {clausula.Basico.CondicionalPosicion}.";
            }

            return res;
        }

        private string DevolverNumeroEnLetras(decimal numero)
        {
            numero = Math.Round(numero, 2);
            var letras = ((int)Math.Abs(numero)).ToWords(CultureInfo.GetCultureInfo("es-AR")).ToUpper();
            var fraccion = numero - Math.Floor(numero);
            if (fraccion > 0)
            {
                letras += $" con {((int)(fraccion * 100)).ToWords(CultureInfo.GetCultureInfo("es-AR")).ToUpper()}";
            }
            return letras;
        }
    }
}
