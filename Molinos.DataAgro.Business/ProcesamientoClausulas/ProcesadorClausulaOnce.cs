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
    public class ProcesadorClausulaOnce : ProcesadorClausula<ClausulaOnce>
    {
        public ProcesadorClausulaOnce(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaOnce clausula)
        {
            var res = new ResultadoClausula();

            var descuentoGeneralFueraPrecio = clausula.Basico.Descuentos.Where(x => x.TipoPeriodoDBId == 1 && x.TipoDBId == 2).FirstOrDefault();
            var descuentoGeneralSobrePrecio = clausula.Basico.Descuentos.Where(x => x.TipoPeriodoDBId == 1 && x.TipoDBId == 1).FirstOrDefault();

            if (descuentoGeneralSobrePrecio.Porcentaje > 0)
            {
                res.Texto += $"Se bonificará sobre el precio el { descuentoGeneralSobrePrecio.Porcentaje }% por tonelada. ";
            }
            if (descuentoGeneralSobrePrecio.Porcentaje < 0)
            {
                res.Texto += $"Se descontará sobre el precio el { descuentoGeneralSobrePrecio.Porcentaje }% por tonelada. ";
            }
            if (descuentoGeneralSobrePrecio.Importe > 0)
            {
                res.Texto += $"Se bonificará sobre el precio el { descuentoGeneralSobrePrecio.Importe } { descuentoGeneralSobrePrecio.Moneda } " +
                    $" ({DevolverNumeroEnLetras(descuentoGeneralSobrePrecio.Importe) }) por tonelada. ";
            }
            if (descuentoGeneralSobrePrecio.Importe < 0 && !clausula.Basico.TipoPosicionCBOTId.HasValue)
            {
                res.Texto += $"Se descontará sobre el precio el { descuentoGeneralSobrePrecio.Importe } { descuentoGeneralSobrePrecio.Moneda } " +
                    $" ({DevolverNumeroEnLetras(descuentoGeneralSobrePrecio.Importe) }) por tonelada. ";
            }


            if (descuentoGeneralFueraPrecio.Porcentaje > 0)
            {
                res.Texto += $"Se bonificará por fuera del precio el { descuentoGeneralFueraPrecio.Porcentaje }% por tonelada. ";
            }
            if (descuentoGeneralFueraPrecio.Porcentaje < 0)
            {
                res.Texto += $"Se descontará por fuera del precio el { descuentoGeneralFueraPrecio.Porcentaje }% por tonelada. ";
            }
            if (descuentoGeneralFueraPrecio.Importe > 0)
            {
                res.Texto += $"Se bonificará por fuera del precio el { descuentoGeneralFueraPrecio.Importe } { descuentoGeneralFueraPrecio.Moneda }" +
                    $" ({DevolverNumeroEnLetras(descuentoGeneralFueraPrecio.Importe) }) por tonelada. ";
            }
            if (descuentoGeneralFueraPrecio.Importe < 0 && !clausula.Basico.TipoPosicionCBOTId.HasValue)
            {
                res.Texto += $"Se descontará por fuera del precio el { descuentoGeneralFueraPrecio.Importe } { descuentoGeneralFueraPrecio.Moneda }" +
                    $" ({DevolverNumeroEnLetras(descuentoGeneralFueraPrecio.Importe)}) por tonelada. ";
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
