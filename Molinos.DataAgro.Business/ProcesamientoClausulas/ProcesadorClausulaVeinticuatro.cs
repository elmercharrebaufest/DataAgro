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
    public class ProcesadorClausulaVeinticuatro : ProcesadorClausula<ClausulaVeinticuatro>
    {
        public ProcesadorClausulaVeinticuatro(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaVeinticuatro clausula)
        {
            var res = new ResultadoClausula();

            if (clausula.Basico.TipoNegocioId == 1 && clausula.Basico.Canje == true)
            {
                res.Texto += $"A los efectos fiscales, únicamente, las partes fijan el valor del presente boleto en la suma de " +
                    $"{clausula.Basico.Monto } {clausula.Basico.MonedaCanjeDescripcion } ({DevolverNumeroEnLetras(clausula.Basico.Monto.Value)}) que el " +
                    $"impuesto de sellos correspondiente será abonado por el comprador y el vendedor en partes iguales.";
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
