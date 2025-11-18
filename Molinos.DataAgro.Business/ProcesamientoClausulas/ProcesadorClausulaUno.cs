using NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using System;
using Humanizer;
using System.Globalization;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Entities.Resources;
using Molinos.DataAgro.Entities.Common.Enums;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorClausulaUno : ProcesadorClausula<ClausulaUno>
    {
        public ProcesadorClausulaUno(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto) : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaUno clausula)
        {
            var res = new ResultadoClausula();
            if (clausula.Basico.BoletoId == (int)EnumBoletoCompraNet.CONFIRMA)
            {
                res.Texto = $"Destino de la mercadería: {clausula.Basico.DestinoLocalidad}, {clausula.Basico.DestinoProvincia}. Origen: {clausula.Basico.Localidad}, {clausula.Basico.Provincia}.";
            }
            else
            {
                if (clausula.Basico.BoletoId == (int)EnumBoletoCompraNet.CARTA_OFERTA)
                {
                    res.Texto = "La VENDEDORA vende a la COMPRADORA";
                    res.Texto += $" la cantidad de {clausula.Basico.Cantidad.ToString("#,##0.##", CultureInfo.GetCultureInfo("es-ES"))} kg. " +
                                 $"(kilogramos {((int)Math.Abs(clausula.Basico.Cantidad)).ToWords(CultureInfo.GetCultureInfo("es-AR")).ToUpper()}) de {clausula.Basico.Material} {(clausula.Basico.CantidadCamiones != 0 ? " o el resultante de " + clausula.Basico.CantidadCamiones + "camiones" : "")}" +
                                 $"y demás condiciones ";
                    if (clausula.Basico.TrigoEspecial == true && clausula.Basico.StandardDeCalidadId != 7)
                    {
                        res.Texto += "CALIDAD ESPECIAL ";
                    }
                    else
                    {
                        //res.Texto += "CALIDAD " + clausula.Basico.StandardDeCalidadDescripcion.ToUpper() + " ";
                        res.Texto += $"{clausula.Basico.Material} " + clausula.Basico.StandardDeCalidadDescripcion.ToUpper() + " ";
                    }
                    if (clausula.Basico.Calidades != null && clausula.Basico.StandardDeCalidadId != 7)
                    {
                        foreach (var cal in clausula.Basico.Calidades)
                        {
                            res.Texto += cal.CalidadEspecialDesc.ToUpper() + " " + cal.Valor.ToString("N2", CultureInfo.CreateSpecificCulture("es-AR")) + "";
                            if (cal.PorcentajeDesde != null && cal.PorcentajeHasta != null)
                            {
                                res.Texto += "Porc. Desde " + cal.PorcentajeDesde + "% Hasta " + cal.PorcentajeHasta + "% ";
                            }
                        }
                    }
                    res.Texto += $"de la cosecha {clausula.Basico.Campania}, ";
                    if (clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO)
                    {
                        res.Texto += $"a {clausula.Basico.PrecioNeto.Value.ToString("#,##0.##", CultureInfo.GetCultureInfo("es-ES"))} {DivisaSimbolica(clausula.Basico.Moneda)} ({DivisaEnLetras(clausula.Basico.Moneda)} {DevolverNumeroEnLetras(clausula.Basico.PrecioNeto.Value)}) más IVA la tonelada .";
                    }
                    res.Texto += $"Procedencia de la mercadería: {clausula.Basico.Localidad} - {clausula.Basico.Provincia}.";
                    res.Texto += $"A todos los efectos impositivos los vendedores declaran que la mercadería {(clausula.Basico.ClasificacionDescripcion == "Productor" ? "SI" : "NO")} es de su propia producción{(clausula.Basico.Consignatario == true ? ", actúa en carácter de consignatario." : ".")}";
                }
                else
                {
                    res.Texto = $"Los señores {clausula.Basico.RazonSocialProveedor} (en adelante el vendedor) domiciliados en {clausula.Basico.ProveedorDireccion} de {clausula.Basico.ProveedorLocalidad}, " +
                    $"{clausula.Basico.ProveedorProvincia}, CP {clausula.Basico.ProveedorCP}, {(!string.IsNullOrEmpty(clausula.Basico.RazonSocialCorredor) ? " por intermedio de " + clausula.Basico.RazonSocialCorredor : "")}" +
                    $" entregan a Molinos Agro S.A. domiciliado en Bouchard 680 piso 12° de la Ciudad de Buenos Aires (en adelante el comprador),";

                    res.Texto += $" la cantidad de {clausula.Basico.Cantidad.ToString("#,##0.##", CultureInfo.GetCultureInfo("es-ES"))} kg. " +
                                 $"(kilogramos {((int)Math.Abs(clausula.Basico.Cantidad)).ToWords(CultureInfo.GetCultureInfo("es-AR")).ToUpper()}) de {clausula.Basico.Material} {(clausula.Basico.CantidadCamiones != 0 ? " o el resultante de " + clausula.Basico.CantidadCamiones + "camiones" : "")}" +
                                 $"y demás condiciones ";
                    if (clausula.Basico.TrigoEspecial == true && clausula.Basico.StandardDeCalidadId != 7)
                    {
                        res.Texto += "CALIDAD ESPECIAL ";
                    }
                    else
                    {
                        //res.Texto += "CALIDAD " + clausula.Basico.StandardDeCalidadDescripcion.ToUpper() + " ";
                        res.Texto += $"{clausula.Basico.Material} " + clausula.Basico.StandardDeCalidadDescripcion.ToUpper() + " ";
                    }
                    if (clausula.Basico.Calidades != null && clausula.Basico.StandardDeCalidadId != 7)
                    {
                        foreach (var cal in clausula.Basico.Calidades)
                        {
                            res.Texto += cal.CalidadEspecialDesc.ToUpper() + " " + cal.Valor.ToString("N2", CultureInfo.CreateSpecificCulture("es-AR")) + "";
                            if (cal.PorcentajeDesde != null && cal.PorcentajeHasta != null)
                            {
                                res.Texto += "Porc. Desde " + cal.PorcentajeDesde + "% Hasta " + cal.PorcentajeHasta + "% ";
                            }
                        }
                    }
                    res.Texto += $"de la cosecha {clausula.Basico.Campania}, ";
                    if (clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO)
                    {
                        res.Texto += $"a {clausula.Basico.PrecioNeto.Value.ToString("#,##0.##", CultureInfo.GetCultureInfo("es-ES"))} {DivisaSimbolica(clausula.Basico.Moneda)} ({DivisaEnLetras(clausula.Basico.Moneda)} {DevolverNumeroEnLetras(clausula.Basico.PrecioNeto.Value)}) más IVA la tonelada";

                    }
                    if (clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && clausula.Basico.Canje != true)
                    {
                        res.Texto += "con precio a fijar";
                    }
                    else if (clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && clausula.Basico.Canje == true)
                    {
                        res.Texto += $"para cancelar {clausula.Basico.Insumo} - {clausula.Basico.Monto} {clausula.Basico.MonedaCanjeId} " +
                            $"(en adelante, el {"Insumo"}) así como también para cancelar los gastos asociados a los que el Vendedor hubiere incurrido para llevar a cabo la presente operación. " +
                            $"(en adelante, los {"Gastos Asociados"}). Las Partes acuerdan que el Insumo será a retirar en puerto por el Vendedor. ";

                    }

                    string destino = clausula.Basico.DestinoDescripcion;
                    res.Texto += $", puesta sobre camión en: Planta {(destino.Equals("San Lorenzo") ? "San Lorenzo o Ricardone" : destino)}" +
                           $"{(clausula.Basico.DestinoCodigoSap.Equals("1068") ? "" : $", localidad de {clausula.Basico.DestinoLocalidad}, {clausula.Basico.DestinoProvincia}")}" +
                           $". A todos los efectos impositivos los vendedores declaran que la mercadería {(clausula.Basico.ClasificacionDescripcion == "Productor" ? "SI" : "NO")} es de su propia producción. ";
                    if (clausula.Basico.Consignatario == true)
                    {
                        res.Texto += "El vendedor actúa en carácter de consignatario. ";
                    }
                    res.Texto += $"Procedencia de la mercadería: {clausula.Basico.Localidad}, provincia de {clausula.Basico.Provincia}.";
                    if (clausula.Basico.EstablecimientoPropio == true)
                    {
                        res.Texto += "El campo es propio. ";
                    }
                    if (clausula.Basico.EstablecimientoPropio == false)
                    {
                        res.Texto += "El campo es arrendado. ";
                    }
                    if (clausula.Basico.PreciosPactados != null && clausula.Basico.PreciosPactados.Count > 0)
                    {
                        res.Texto += "El PRECIO del contrato se modificará de acuerdo a las siguientes fechas de entrega y recibos, desde el ";
                        foreach (var precio in clausula.Basico.PreciosPactados)
                        {
                            res.Texto += $"{precio.FechaDesde} al {precio.FechaHasta}" +
                                $" {precio.MonedaPactadoDesc} {precio.Precio.ToString("N2", CultureInfo.CreateSpecificCulture("es-AR"))} ({DevolverNumeroEnLetras(precio.Precio)}),";
                        }
                        res.Texto = res.Texto.EndsWith(",") ? string.Format("{0}.", res.Texto.Remove(res.Texto.Length - 1)) : res.Texto;
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

        private string DivisaEnLetras(string divisa)
        {
            return divisa == "USD" ? Text.Divisa_USD : Text.Divisa_ARP;
        }

        private string DivisaSimbolica(string divisa)
        {
            return divisa == "USD" ? Text.USD : Text.ARP;
        }

    }
}
