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
using System.Diagnostics.Contracts;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorClausulaCuarentaYOcho : ProcesadorClausula<ClausulaCuarentaYOcho>
    {
        public ProcesadorClausulaCuarentaYOcho(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaCuarentaYOcho clausula)
        {
            // CONDICIÓN DE LA SIGUIENTE CLÁUSULA: SI EL CAMPO FECHA_CIERTA ESTÁ COMPLETO

            var res = new ResultadoClausula();
            if (clausula.Basico.FechaCierta != null)
            {
                res.Texto += $" El pago del "+$"{clausula.Basico.PorcentajeDePago.Value}"+"  % se efectuará en la fecha indicada en ‘Información" + " sobre pagos’ con la condición que con 72 hs de anticipación se hayan cumplido los requisitos exigibles para el pago. En caso " + "contrario, el pago se realizará a las 72 hs de cumplidos los requisitos previamente mencionados. El "+ $"{(100 - clausula.Basico.PorcentajeDePago.Value)} " +" % restante se pagará durante los 30 días posteriores";
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
