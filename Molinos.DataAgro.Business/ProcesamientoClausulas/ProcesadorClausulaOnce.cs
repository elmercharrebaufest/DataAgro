using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using System;
using Humanizer;
using System.Globalization;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Entities.Resources;

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
            //SI ESTÁ SELECCIONADA BONIFICACIONES SOBRE PRECIO Y/O POR FUERA DE PRECIO

            var res = new ResultadoClausula();

            // Se modifica por peticion de Santiago para que cuando sea posicion CBOT no se muestre estas clausulas
            bool esPosicionCBOT = clausula.Basico.TipoPosicionCBOTId !=null ? (clausula.Basico.TipoPosicionCBOTId == 1 ? true: false) : false;


            var descuentoGeneralSobrePrecio = esPosicionCBOT ? null : clausula.Basico.Descuentos.Find(x => x.TipoPeriodoDBId == 1 && x.TipoDBId == 1);
            var descuentoGeneralFueraPrecio = esPosicionCBOT ? null : clausula.Basico.Descuentos.Find(x => x.TipoPeriodoDBId == 1 && x.TipoDBId == 2);

            res.Texto += DevolverClausulaBonificacionSobrePrecio(descuentoGeneralSobrePrecio);
            res.Texto += DevolverClausulaBonificacionFueraPrecio(descuentoGeneralFueraPrecio);

            if (!esPosicionCBOT)
            {
                if (clausula.Basico.EPA || clausula.Basico.Sustentable)
                {
                    if (clausula.Basico.SustentableTipoDBId == 1)
                    {
                        res.Texto += DevolverClausulaBonificacionSobrePrecioAdicionales(clausula.Basico);
                    }
                    if (clausula.Basico.SustentableTipoDBId == 2)
                    {
                        res.Texto += DevolverClausulaBonificacionFueraPrecioAdicionales(clausula.Basico);
                    }
                }
            }

            return res;
        }

        private string DevolverClausulaBonificacionSobrePrecio(DescuentoBonificacionDto descuentoGeneralSobrePrecio)
        {
            string clausula = string.Empty;
            if (descuentoGeneralSobrePrecio?.Porcentaje > 0)
            {
                clausula += $"Se bonificará sobre el precio el {descuentoGeneralSobrePrecio.Porcentaje}% por tonelada. ";
            }
            if (descuentoGeneralSobrePrecio?.Porcentaje < 0)
            {
                clausula += $"Se descontará sobre el precio el {descuentoGeneralSobrePrecio.Porcentaje}% por tonelada. ";
            }
            if (descuentoGeneralSobrePrecio?.Importe > 0)
            {
                clausula += $"Se bonificará sobre el precio {DivisaSimbolica(descuentoGeneralSobrePrecio.Moneda)} {ValorAbsoluto(descuentoGeneralSobrePrecio.Importe)} " +
                    $" ({DevolverNumeroEnLetrasConDivisa(descuentoGeneralSobrePrecio.Importe, descuentoGeneralSobrePrecio.Moneda)}) por tonelada. ";
            }
            if (descuentoGeneralSobrePrecio?.Importe < 0)
            {
                clausula += $"Se descontará sobre el precio {DivisaSimbolica(descuentoGeneralSobrePrecio.Moneda)} {ValorAbsoluto(descuentoGeneralSobrePrecio.Importe)} " +
                    $" ({DevolverNumeroEnLetrasConDivisa(descuentoGeneralSobrePrecio.Importe, descuentoGeneralSobrePrecio.Moneda)}) por tonelada. ";
            }
            return clausula;
        }

        private string DevolverClausulaBonificacionFueraPrecio(DescuentoBonificacionDto descuentoGeneralFueraPrecio)
        {
            string clausula = string.Empty;
            if (descuentoGeneralFueraPrecio?.Porcentaje > 0)
            {
                clausula += $"Se bonificará por fuera del precio el {descuentoGeneralFueraPrecio.Porcentaje}% por tonelada. ";
            }
            if (descuentoGeneralFueraPrecio?.Porcentaje < 0)
            {
                clausula += $"Se descontará por fuera del precio el {descuentoGeneralFueraPrecio.Porcentaje}% por tonelada. ";
            }
            if (descuentoGeneralFueraPrecio?.Importe > 0)
            {
                clausula += $"Se bonificará por fuera del precio {DivisaSimbolica(descuentoGeneralFueraPrecio.Moneda)} {ValorAbsoluto(descuentoGeneralFueraPrecio.Importe)}" +
                    $" ({DevolverNumeroEnLetrasConDivisa(descuentoGeneralFueraPrecio.Importe, descuentoGeneralFueraPrecio.Moneda)}) por tonelada. ";
            }
            if (descuentoGeneralFueraPrecio?.Importe < 0)
            {
                clausula += $"Se descontará por fuera del precio {DivisaSimbolica(descuentoGeneralFueraPrecio.Moneda)} {ValorAbsoluto(descuentoGeneralFueraPrecio.Importe)}" +
                    $" ({DevolverNumeroEnLetrasConDivisa(descuentoGeneralFueraPrecio.Importe, descuentoGeneralFueraPrecio.Moneda)}) por tonelada. ";
            }
            return clausula;
        }

        private string DevolverClausulaBonificacionSobrePrecioAdicionales(BasicoContrato descuentoGeneralSobrePrecio)
        {
            string clausula = string.Empty;
            if (descuentoGeneralSobrePrecio?.Importe_Sustentable > 0)
            {
                clausula += $"Se bonificará sobre el precio {DivisaSimbolica(descuentoGeneralSobrePrecio.Moneda_Sustentable)} {ValorAbsoluto((decimal)descuentoGeneralSobrePrecio.Importe_Sustentable)} " +
                    $" ({DevolverNumeroEnLetrasConDivisa((decimal)descuentoGeneralSobrePrecio.Importe_Sustentable, descuentoGeneralSobrePrecio.Moneda)}) por tonelada. ";
            }
            if (descuentoGeneralSobrePrecio?.Importe_Sustentable < 0)
            {
                clausula += $"Se descontará sobre el precio {DivisaSimbolica(descuentoGeneralSobrePrecio.Moneda_Sustentable)} {ValorAbsoluto((decimal)descuentoGeneralSobrePrecio.Importe_Sustentable)} " +
                    $" ({DevolverNumeroEnLetrasConDivisa((decimal)descuentoGeneralSobrePrecio?.Importe_Sustentable, descuentoGeneralSobrePrecio.Moneda)}) por tonelada. ";
            }
            return clausula;
        }

        private string DevolverClausulaBonificacionFueraPrecioAdicionales(BasicoContrato descuentoGeneralFueraPrecio)
        {
            string clausula = string.Empty;
            if (descuentoGeneralFueraPrecio?.Importe_Sustentable > 0)
            {
                clausula += $"Se bonificará por fuera del precio {DivisaSimbolica(descuentoGeneralFueraPrecio.Moneda_Sustentable)} {ValorAbsoluto((decimal)descuentoGeneralFueraPrecio.Importe_Sustentable)}" +
                    $" ({DevolverNumeroEnLetrasConDivisa((decimal)descuentoGeneralFueraPrecio?.Importe_Sustentable, descuentoGeneralFueraPrecio.Moneda)}) por tonelada. ";
            }
            if (descuentoGeneralFueraPrecio?.Importe_Sustentable < 0)
            {
                clausula += $"Se descontará por fuera del precio {DivisaSimbolica(descuentoGeneralFueraPrecio.Moneda_Sustentable)} {ValorAbsoluto((decimal)descuentoGeneralFueraPrecio.Importe_Sustentable)}" +
                    $" ({DevolverNumeroEnLetrasConDivisa((decimal)descuentoGeneralFueraPrecio?.Importe_Sustentable, descuentoGeneralFueraPrecio.Moneda)}) por tonelada. ";
            }
            return clausula;
        }


        private decimal ValorAbsoluto(decimal numero)
        {
            return Math.Abs(numero);
        }

        private string DevolverNumeroEnLetrasConDivisa(decimal numero, string moneda)
        {
            numero = Math.Round(numero, 2);
            var letras = moneda == "USD" ? "Dólares " : "Pesos ";
            letras += ((int)Math.Abs(numero)).ToWords(CultureInfo.GetCultureInfo("es-AR")).ToUpper();
            var fraccion = numero - Math.Floor(numero);
            letras += $" con {((int)(fraccion * 100)).ToWords(CultureInfo.GetCultureInfo("es-AR")).ToUpper()} Centavos";
            return letras;
        }

        private string DivisaSimbolica(string divisa)
        {
            return divisa == "USD" ? Text.USD : Text.ARP;
        }

        private string NumeroConSeparadores(decimal? numero)
        {
            var objNumberFormatInfo = new NumberFormatInfo() { NumberGroupSeparator = "." };
            // Obtener el valor absoluto del número
            decimal valorAbsoluto = Math.Abs(numero.GetValueOrDefault());
            return valorAbsoluto.ToString("#,###.##", objNumberFormatInfo);
        }
    }
}
