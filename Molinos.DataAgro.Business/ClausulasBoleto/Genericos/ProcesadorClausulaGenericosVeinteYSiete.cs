using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.ClausulasBoleto.Genericos;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Business.ClausulasBoleto.Genericos
{
    public class ProcesadorClausulaGenericosVeinteYSiete : ProcesadorClausulaGenericos<ClausulaGenericosVeinteYSiete>
    {
        public ProcesadorClausulaGenericosVeinteYSiete(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }

        public override ResultadoClausula DevolverClausulas(ClausulaGenericosVeinteYSiete clausula)
        {
            var res = new ResultadoClausula();
            if (clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO && clausula.Basico.CondicionalContratoId > 0 && clausula.Basico.CondicionalPrecio.HasValue)
            {
                res.Texto += $"Las partes se comprometen a realizar un boleto de compraventa de {clausula.Basico.Material} por el mismo volumen de toneladas que " +
                    $"el presente boleto, bajo la condición a precio en {clausula.Basico.CondicionalPosicion} a {clausula.Basico.CondicionalMonedaDescripcion} {clausula.Basico.CondicionalPrecio?.ToString("N", new CultureInfo("es-AR"))} " +
                    $"({MetodosUtiles.DevolverNumeroEnLetras(clausula.Basico.CondicionalPrecio.Value)}) si el ajuste de soja MATBA {clausula.Basico.CondicionalPosicion} supera dicho valor el {clausula.Basico.CondicionalFechaFormateado} (en adelante la Condición Precedente). " +
                    $"En caso de que la Condición Precedente se cumpla y el vendedor rechace la generación del nuevo boleto comprometido, las partes acuerdan que el vendedor deberá abonar en concepto de multa " +
                    $"el diferencial de precio resultante de la condición precedente es decir {clausula.Basico.CondicionalMonedaDescripcion} {clausula.Basico.CondicionalPrecio?.ToString("N", new CultureInfo("es-AR"))} " +
                    $"y el precio de ajuste MATBA de {clausula.Basico.Material} {clausula.Basico.CondicionalPosicion}.";
            }
            return res;
        }
    }
}
