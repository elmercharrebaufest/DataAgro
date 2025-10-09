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
using System.Windows.Media.Animation;

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
                var descuentos = (clausula.Basico.Descuentos != null && clausula.Basico.Descuentos.Count > 0) ? clausula.Basico.Descuentos.Find(x => x.TipoPeriodoDBId == (int)EnumTipoPeriodoDB.POR_FECHA_DE_FIJACION) : null;

                if (clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && (descuentos != null && descuentos.Id > 0))
                {
                    var descuentoGeneralFueraPrecio = clausula.Basico.Descuentos.Find(x => x.TipoPeriodoDBId == (int)EnumTipoPeriodoDB.POR_FECHA_DE_FIJACION && x.TipoDBId == (int)EnumTipoDB.POR_FUERA_DEL_PRECIO);
                    var descuentoGeneralSobrePrecio = clausula.Basico.Descuentos.Find(x => x.TipoPeriodoDBId == (int)EnumTipoPeriodoDB.POR_FECHA_DE_FIJACION && x.TipoDBId == (int)EnumTipoDB.SOBRE_EL_PRECIO);
                    
                    if (descuentoGeneralFueraPrecio != null)
                    {
                        DateTime? desdeFijacionFueraPrecio = Convert.ToDateTime(descuentoGeneralFueraPrecio.FechaDesde);
                        DateTime? hastaFijacionFueraPrecio = Convert.ToDateTime(descuentoGeneralFueraPrecio.FechaHasta);
                        decimal porcentajelFueraPrecio = (decimal)(descuentoGeneralFueraPrecio?.Porcentaje < 0 ? descuentoGeneralFueraPrecio?.Porcentaje * -1 : descuentoGeneralFueraPrecio?.Porcentaje);
                        decimal importeFueraPrecio = (decimal)(descuentoGeneralFueraPrecio?.Importe < 0 ? descuentoGeneralFueraPrecio?.Importe * -1 : descuentoGeneralFueraPrecio?.Importe);
                        decimal descuentoImporteFueraPrecio = porcentajelFueraPrecio > 0 ? (importeFueraPrecio * porcentajelFueraPrecio) / 100 : 0;

                        if (descuentoGeneralFueraPrecio?.Porcentaje > 0)
                        {
                            importeFueraPrecio = porcentajelFueraPrecio > 0 ? Math.Round((importeFueraPrecio + descuentoImporteFueraPrecio),2) : importeFueraPrecio;
                        }
                        if (descuentoGeneralFueraPrecio?.Porcentaje < 0)
                        {
                            importeFueraPrecio = porcentajelFueraPrecio > 0 ? Math.Round((importeFueraPrecio - descuentoImporteFueraPrecio), 2) : importeFueraPrecio;
                        }

                        if (descuentoGeneralFueraPrecio?.Porcentaje > 0)
                        {
                            res.Texto += $"De acuerdo a las siguientes fechas de fijación, desde el {desdeFijacionFueraPrecio.GetValueOrDefault():dd'/'MM'/'yyyy} al {hastaFijacionFueraPrecio.GetValueOrDefault():dd'/'MM'/'yyyy} " +
                                $"se descontará por fuera del precio el {porcentajelFueraPrecio}% del precio. ";
                        }
                        if (descuentoGeneralFueraPrecio?.Porcentaje < 0)
                        {
                            res.Texto += $"De acuerdo a las siguientes fechas de fijación, desde el {desdeFijacionFueraPrecio.GetValueOrDefault():dd'/'MM'/'yyyy} al {hastaFijacionFueraPrecio.GetValueOrDefault():dd'/'MM'/'yyyy} " +
                                $"se bonificará  por fuera del precio el {porcentajelFueraPrecio}% por tonelada. ";
                        }
                        if (descuentoGeneralFueraPrecio?.Importe > 0)
                        {
                            res.Texto += $"De acuerdo a las siguientes fechas de fijación, desde el {desdeFijacionFueraPrecio.GetValueOrDefault():dd'/'MM'/'yyyy} al {hastaFijacionFueraPrecio.GetValueOrDefault():dd'/'MM'/'yyyy} " +
                                $"se descontará por fuera del precio {descuentoGeneralFueraPrecio.Moneda} {importeFueraPrecio}" +
                                $" ({MetodosUtiles.DevolverNumeroEnLetrasConDivisa(importeFueraPrecio, descuentoGeneralFueraPrecio.Moneda)}) por tonelada. ";
                        }
                        if (descuentoGeneralFueraPrecio?.Importe < 0)
                        {
                            res.Texto += $"De acuerdo a las siguientes fechas de fijación, desde el {desdeFijacionFueraPrecio.GetValueOrDefault():dd'/'MM'/'yyyy} al {hastaFijacionFueraPrecio.GetValueOrDefault():dd'/'MM'/'yyyy} " +
                                $"se bonificará por fuera del precio {descuentoGeneralFueraPrecio.Moneda} {importeFueraPrecio}" +
                                $" ({MetodosUtiles.DevolverNumeroEnLetrasConDivisa(importeFueraPrecio, descuentoGeneralFueraPrecio.Moneda)}) por tonelada. ";
                        }
                    }

                    if (descuentoGeneralSobrePrecio != null)
                    {
                        DateTime? desdeFijacionSobrePrecio = Convert.ToDateTime(descuentoGeneralSobrePrecio.FechaDesde);
                        DateTime? hastaFijacionSobrePrecio = Convert.ToDateTime(descuentoGeneralSobrePrecio.FechaHasta);

                        decimal porcentajelSobrePrecio = (decimal)(descuentoGeneralSobrePrecio?.Porcentaje < 0 ? descuentoGeneralSobrePrecio?.Porcentaje * -1 : descuentoGeneralSobrePrecio?.Porcentaje);
                        decimal importeSobrePrecio = (decimal)(descuentoGeneralSobrePrecio?.Importe < 0 ? descuentoGeneralSobrePrecio?.Importe * -1 : descuentoGeneralSobrePrecio?.Importe);
                        decimal descuentoImporteFueraPrecio = descuentoGeneralSobrePrecio?.Porcentaje > 0 ? (importeSobrePrecio * porcentajelSobrePrecio) / 100 : 0;

                        if (descuentoGeneralSobrePrecio?.Porcentaje > 0)
                        {
                            importeSobrePrecio = porcentajelSobrePrecio > 0 ? Math.Round( (importeSobrePrecio + descuentoImporteFueraPrecio),2) : importeSobrePrecio;
                        }
                        if (descuentoGeneralSobrePrecio?.Porcentaje < 0)
                        {
                            importeSobrePrecio = porcentajelSobrePrecio > 0 ? Math.Round((importeSobrePrecio - descuentoImporteFueraPrecio), 2) : importeSobrePrecio;
                        }

                        // En la bonificacion sobre el precio siempre debe mostrarse lo que se tiene como apertura de precio
                        if (descuentoGeneralSobrePrecio?.Porcentaje > 0)
                        {
                            res.Texto += $"El PRECIO del contrato se modificará de acuerdo a las siguientes fechas de fijación, desde el {desdeFijacionSobrePrecio.GetValueOrDefault():dd'/'MM'/'yyyy} al {hastaFijacionSobrePrecio.GetValueOrDefault():dd'/'MM'/'yyyy} " +
                                $"se adicionará sobre el precio el  {porcentajelSobrePrecio}% del precio. ";
                        }
                        if (descuentoGeneralSobrePrecio?.Porcentaje < 0)
                        {
                            res.Texto += $"El PRECIO del contrato se modificará de acuerdo a las siguientes fechas de fijación, desde el {desdeFijacionSobrePrecio.GetValueOrDefault():dd'/'MM'/'yyyy} al {hastaFijacionSobrePrecio.GetValueOrDefault():dd'/'MM'/'yyyy} " +
                                $"se descontará  sobre el precio el  {porcentajelSobrePrecio}% por tonelada. ";
                        }
                        if (descuentoGeneralSobrePrecio?.Importe > 0)
                        {
                            res.Texto += $"El PRECIO del contrato se modificará de acuerdo a las siguientes fechas de fijación, desde el {desdeFijacionSobrePrecio.GetValueOrDefault():dd'/'MM'/'yyyy} al {hastaFijacionSobrePrecio.GetValueOrDefault():dd'/'MM'/'yyyy} " +
                                $"se adicionará sobre el precio {descuentoGeneralSobrePrecio?.Moneda} {importeSobrePrecio}" +
                                $" ({MetodosUtiles.DevolverNumeroEnLetrasConDivisa(importeSobrePrecio, descuentoGeneralSobrePrecio.Moneda)}) por tonelada. ";
                        }
                        if (descuentoGeneralSobrePrecio?.Importe < 0)
                        {
                            res.Texto += $"El PRECIO del contrato se modificará de acuerdo a las siguientes fechas de fijación, desde el {desdeFijacionSobrePrecio.GetValueOrDefault():dd'/'MM'/'yyyy} al {hastaFijacionSobrePrecio.GetValueOrDefault():dd'/'MM'/'yyyy} " +
                                $"se descontará sobre el precio {descuentoGeneralSobrePrecio?.Moneda} {importeSobrePrecio}" +
                                $" ({MetodosUtiles.DevolverNumeroEnLetrasConDivisa(importeSobrePrecio, descuentoGeneralSobrePrecio.Moneda)}) por tonelada. ";
                        }
                    }
                }
            }
            return res;
        }
    }
}
