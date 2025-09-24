using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.ClausulasBoleto.Genericos;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Business.ClausulasBoleto.Genericos
{
    public class ProcesadorClausulaGenericosCinco : ProcesadorClausulaGenericos<ClausulaGenericosCinco>
    {
        public ProcesadorClausulaGenericosCinco(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }

        public override ResultadoClausula DevolverClausulas(ClausulaGenericosCinco clausula)
        {
            var res = new ResultadoClausula();

            if (clausula.Basico.BoletoId != (int)EnumBoletoCompraNet.CARTA_OFERTA)
            {
                var descuentos = clausula.Basico.Descuentos.Find(x => x.TipoPeriodoDBId == (int)EnumTipoPeriodoDB.POR_FECHA_DE_FIJACION);

                if (clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && (descuentos != null && descuentos.Id > 0))
                {
                    var descuentoGeneralFueraPrecio = clausula.Basico.Descuentos.Find(x => x.TipoPeriodoDBId == (int)EnumTipoPeriodoDB.POR_FECHA_DE_FIJACION && x.TipoDBId == (int)EnumTipoDB.POR_FUERA_DEL_PRECIO);
                    var descuentoGeneralSobrePrecio = clausula.Basico.Descuentos.Find(x => x.TipoPeriodoDBId == (int)EnumTipoPeriodoDB.POR_FECHA_DE_FIJACION && x.TipoDBId == (int)EnumTipoDB.SOBRE_EL_PRECIO);
                    
                    DateTime? desdeFijacion = Convert.ToDateTime(descuentoGeneralFueraPrecio.FechaDesde);
                    DateTime? hastaFijacion = Convert.ToDateTime(descuentoGeneralFueraPrecio.FechaHasta);

                    if (descuentoGeneralFueraPrecio?.Porcentaje > 0)
                    {
                        res.Texto += $"De acuerdo a las siguientes fechas de fijación, desde el {desdeFijacion.GetValueOrDefault():dd'/'MM'/'yyyy} al {hastaFijacion.GetValueOrDefault():dd'/'MM'/'yyyy} " +
                            $"se descontará por fuera del precio el {descuentoGeneralFueraPrecio.Porcentaje}% del precio. ";
                    }
                    if (descuentoGeneralFueraPrecio?.Porcentaje < 0)
                    {
                        res.Texto += $"De acuerdo a las siguientes fechas de fijación, desde el {desdeFijacion.GetValueOrDefault():dd'/'MM'/'yyyy} al {hastaFijacion.GetValueOrDefault():dd'/'MM'/'yyyy} " +
                            $"se bonificará  por fuera del precio el {descuentoGeneralFueraPrecio.Porcentaje}% por tonelada. ";
                    }
                    if (descuentoGeneralFueraPrecio?.Importe > 0)
                    {
                        res.Texto += $"De acuerdo a las siguientes fechas de fijación, desde el {desdeFijacion.GetValueOrDefault():dd'/'MM'/'yyyy} al {hastaFijacion.GetValueOrDefault():dd'/'MM'/'yyyy} " +
                            $"se descontará por fuera del precio {descuentoGeneralFueraPrecio.Moneda} {descuentoGeneralFueraPrecio.Importe}" +
                            $" ({MetodosUtiles.DevolverNumeroEnLetras(descuentoGeneralFueraPrecio.Importe)}) por tonelada. ";
                    }
                    if (descuentoGeneralFueraPrecio?.Importe < 0)
                    {
                        res.Texto += $"De acuerdo a las siguientes fechas de fijación, desde el {clausula.Basico.DesdeFijacion.GetValueOrDefault():dd'/'MM'/'yyyy} al {hastaFijacion.GetValueOrDefault():dd'/'MM'/'yyyy} " +
                            $"se bonificará por fuera del precio {descuentoGeneralFueraPrecio.Moneda} {descuentoGeneralFueraPrecio.Importe}" +
                            $" ({MetodosUtiles.DevolverNumeroEnLetras(descuentoGeneralFueraPrecio.Importe)}) por tonelada. ";
                    }
                    // En la bonificacion sobre el precio siempre debe mostrarse lo que se tiene como apertura de precio
                    if (descuentoGeneralSobrePrecio?.Porcentaje > 0)
                    {
                        res.Texto += $"De acuerdo a las siguientes fechas de fijación, desde el {desdeFijacion.GetValueOrDefault():dd'/'MM'/'yyyy} al {hastaFijacion.GetValueOrDefault():dd'/'MM'/'yyyy} " +
                            $"se descontará sobre el precio el  {descuentoGeneralSobrePrecio?.Porcentaje}% del precio. ";
                    }
                    if (descuentoGeneralSobrePrecio?.Porcentaje < 0)
                    {
                        res.Texto += $"De acuerdo a las siguientes fechas de fijación, desde el {desdeFijacion.GetValueOrDefault():dd'/'MM'/'yyyy} al {hastaFijacion.GetValueOrDefault():dd'/'MM'/'yyyy} " +
                            $"se bonificará  sobre el precio el  {descuentoGeneralSobrePrecio?.Porcentaje}% por tonelada. ";
                    }
                    if (descuentoGeneralSobrePrecio?.Importe > 0)
                    {
                        res.Texto += $"De acuerdo a las siguientes fechas de fijación, desde el {desdeFijacion.GetValueOrDefault():dd'/'MM'/'yyyy} al {hastaFijacion.GetValueOrDefault():dd'/'MM'/'yyyy} " +
                            $"se descontará sobre el precio {descuentoGeneralSobrePrecio?.Moneda} {descuentoGeneralSobrePrecio?.Importe}" +
                            $" ({MetodosUtiles.DevolverNumeroEnLetras((decimal)descuentoGeneralSobrePrecio?.Importe)}) por tonelada. ";
                    }
                    if (descuentoGeneralSobrePrecio?.Importe < 0)
                    {
                        res.Texto += $"De acuerdo a las siguientes fechas de fijación, desde el {desdeFijacion.GetValueOrDefault():dd'/'MM'/'yyyy} al {hastaFijacion.GetValueOrDefault():dd'/'MM'/'yyyy} " +
                            $"se bonificará sobre el precio {descuentoGeneralSobrePrecio?.Moneda} {descuentoGeneralSobrePrecio?.Importe}" +
                            $" ({MetodosUtiles.DevolverNumeroEnLetras((decimal)descuentoGeneralSobrePrecio?.Importe)}) por tonelada. ";
                    }
                }
            }
            return res;
        }
    }
}
