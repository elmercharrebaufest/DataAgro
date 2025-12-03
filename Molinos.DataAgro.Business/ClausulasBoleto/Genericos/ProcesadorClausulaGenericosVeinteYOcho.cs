using NLog;
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
    public class ProcesadorClausulaGenericosVeinteYOcho : ProcesadorClausulaGenericos<ClausulaGenericosVeinteYOcho>
    {
        public ProcesadorClausulaGenericosVeinteYOcho(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }

        public override ResultadoClausula DevolverClausulas(ClausulaGenericosVeinteYOcho clausula)
        {
            var res = new ResultadoClausula();
            if (clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && clausula.Basico.CondicionalContratoId > 0 && clausula.Basico.CondicionalPrecio.HasValue)
            {
                res.Texto += $"Las partes acuerdan que el presente boleto deberá ser fijado a {clausula.Basico.CondicionalMonedaDescripcion} {clausula.Basico.CondicionalPrecio?.ToString("N", new CultureInfo("es-AR"))} ({MetodosUtiles.DevolverNumeroEnLetrasConDivisa(clausula.Basico.CondicionalPrecio.Value, clausula.Basico.CondicionalMonedaDescripcion)}) " +
                    $"si el ajuste de {clausula.Basico.Material} MATBA {clausula.Basico.CondicionalPosicion} supera dicho valor el {clausula.Basico.CondicionalFechaFormateado} (en adelante la Condición Precedente), " +
                    $"caso contrario se fijará por mercado comprador hasta {clausula.Basico.CondicionalFechaFormateado}. En caso de que la Condición Precedente se cumpla y el vendedor rechace la generación del nuevo boleto " +
                    $"comprometido, las partes acuerdan que el vendedor deberá abonar en concepto de multa el diferencial de precio resultante de la condición " +
                    $"precedente, es decir {clausula.Basico.CondicionalMonedaDescripcion} {clausula.Basico.CondicionalPrecio?.ToString("N", new CultureInfo("es-AR"))} " +
                    $"y el precio de ajuste MATBA de {clausula.Basico.Material} {clausula.Basico.CondicionalPosicion}.";
            }
            return res;
        }
    }
}
