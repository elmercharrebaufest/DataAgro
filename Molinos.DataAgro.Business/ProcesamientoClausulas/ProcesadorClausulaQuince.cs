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
    public class ProcesadorClausulaQuince : ProcesadorClausula<ClausulaQuince>
    {
        public ProcesadorClausulaQuince(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaQuince clausula)
        {
            //SI EL NEGOCIO ES DE TIPO A FIJAR Y FECHA DE FIJACIÓN

            var res = new ResultadoClausula();

            if (clausula.Basico.BoletoId != (int)EnumBoletoCompraNet.CARTA_OFERTA)
            {
                if (clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && clausula.Basico.HastaFijacion.HasValue)
                {

                    var aperturaPrecioPorContrato = (clausula.Basico.AperturaPrecios != null && clausula.Basico.AperturaPrecios.Count > 0) ? clausula.Basico.AperturaPrecios.Where(x => x.ConceptoAperturaPrecioId == 4 && (x.Importe != 0 || x.Porcentaje != 0)).FirstOrDefault() : null;

                    var descuentoGeneralFueraPrecio = clausula.Basico.Descuentos.Find(x => x.TipoPeriodoDBId == 1 && x.TipoDBId == 2);
                    var descuentoGeneralSobrePrecio = clausula.Basico.Descuentos.Find(x => x.TipoPeriodoDBId == 1 && x.TipoDBId == 1);
                    decimal? importeSobrePrecio = aperturaPrecioPorContrato != null ? aperturaPrecioPorContrato.Importe : descuentoGeneralSobrePrecio?.Importe;
                    decimal? porcentajeSobrePrecio = aperturaPrecioPorContrato != null ? aperturaPrecioPorContrato.Porcentaje : descuentoGeneralSobrePrecio?.Porcentaje;
                    string monedaSobrePrecio = aperturaPrecioPorContrato != null ? aperturaPrecioPorContrato.Moneda : descuentoGeneralSobrePrecio?.Moneda;

                    if (descuentoGeneralFueraPrecio?.Porcentaje > 0)
                    {
                        res.Texto += $"De acuerdo a las siguientes fechas de fijación, desde el {clausula.Basico.DesdeFijacion.GetValueOrDefault():dd'/'MM'/'yyyy} al {clausula.Basico.HastaFijacion.GetValueOrDefault():dd'/'MM'/'yyyy} " +
                            $"se descontará por fuera del precio el {descuentoGeneralFueraPrecio.Porcentaje}% del precio. ";
                    }
                    if (descuentoGeneralFueraPrecio?.Porcentaje < 0)
                    {
                        res.Texto += $"De acuerdo a las siguientes fechas de fijación, desde el {clausula.Basico.DesdeFijacion.GetValueOrDefault():dd'/'MM'/'yyyy} al {clausula.Basico.HastaFijacion.GetValueOrDefault():dd'/'MM'/'yyyy} " +
                            $"se bonificará  por fuera del precio el {descuentoGeneralFueraPrecio.Porcentaje}% por tonelada. ";
                    }
                    if (descuentoGeneralFueraPrecio?.Importe > 0)
                    {
                        res.Texto += $"De acuerdo a las siguientes fechas de fijación, desde el {clausula.Basico.DesdeFijacion.GetValueOrDefault():dd'/'MM'/'yyyy} al {clausula.Basico.HastaFijacion.GetValueOrDefault():dd'/'MM'/'yyyy} " +
                            $"se descontará por fuera del precio {descuentoGeneralFueraPrecio.Moneda} {descuentoGeneralFueraPrecio.Importe}" +
                            $" ({DevolverNumeroEnLetras(descuentoGeneralFueraPrecio.Importe)}) por tonelada. ";
                    }
                    if (descuentoGeneralFueraPrecio?.Importe < 0)
                    {
                        res.Texto += $"De acuerdo a las siguientes fechas de fijación, desde el {clausula.Basico.DesdeFijacion.GetValueOrDefault():dd'/'MM'/'yyyy} al {clausula.Basico.HastaFijacion.GetValueOrDefault():dd'/'MM'/'yyyy} " +
                            $"se bonificará por fuera del precio {descuentoGeneralFueraPrecio.Moneda} {descuentoGeneralFueraPrecio.Importe}" +
                            $" ({DevolverNumeroEnLetras(descuentoGeneralFueraPrecio.Importe)}) por tonelada. ";
                    }
                    // En la bonificacion sobre el precio siempre debe mostrarse lo que se tiene como apertura de precio
                    if (porcentajeSobrePrecio > 0)
                    {
                        res.Texto += $"De acuerdo a las siguientes fechas de fijación, desde el {clausula.Basico.DesdeFijacion.GetValueOrDefault():dd'/'MM'/'yyyy} al {clausula.Basico.HastaFijacion.GetValueOrDefault():dd'/'MM'/'yyyy} " +
                            $"se descontará sobre el precio el  {porcentajeSobrePrecio}% del precio. ";
                    }
                    if (porcentajeSobrePrecio < 0)
                    {
                        res.Texto += $"De acuerdo a las siguientes fechas de fijación, desde el {clausula.Basico.DesdeFijacion.GetValueOrDefault():dd'/'MM'/'yyyy} al {clausula.Basico.HastaFijacion.GetValueOrDefault():dd'/'MM'/'yyyy} " +
                            $"se bonificará  sobre el precio el  {porcentajeSobrePrecio}% por tonelada. ";
                    }
                    if (importeSobrePrecio > 0)
                    {
                        res.Texto += $"De acuerdo a las siguientes fechas de fijación, desde el {clausula.Basico.DesdeFijacion.GetValueOrDefault():dd'/'MM'/'yyyy} al {clausula.Basico.HastaFijacion.GetValueOrDefault():dd'/'MM'/'yyyy} " +
                            $"se descontará sobre el precio {monedaSobrePrecio} {importeSobrePrecio}" +
                            $" ({DevolverNumeroEnLetras((decimal)importeSobrePrecio)}) por tonelada. ";
                    }
                    if (importeSobrePrecio < 0)
                    {
                        res.Texto += $"De acuerdo a las siguientes fechas de fijación, desde el {clausula.Basico.DesdeFijacion.GetValueOrDefault():dd'/'MM'/'yyyy} al {clausula.Basico.HastaFijacion.GetValueOrDefault():dd'/'MM'/'yyyy} " +
                            $"se bonificará sobre el precio {monedaSobrePrecio} {importeSobrePrecio}" +
                            $" ({DevolverNumeroEnLetras((decimal)importeSobrePrecio)}) por tonelada. ";
                    }
                }
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
