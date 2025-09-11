using Humanizer;
using Molinos.DataAgro.Entities.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Business.ClausulasBoleto
{
    public static class MetodosUtiles
    {

        public static decimal ValorAbsoluto(decimal numero)
        {
            return Math.Abs(numero);
        }

        public static string DevolverNumeroEnLetrasConDivisa(decimal numero, string moneda)
        {
            numero = Math.Round(numero, 2);
            var letras = moneda == "USD" ? "Dólares " : "Pesos ";
            letras += ((int)Math.Abs(numero)).ToWords(CultureInfo.GetCultureInfo("es-AR")).ToUpper();
            var fraccion = numero - Math.Floor(numero);
            letras += $" con {((int)(fraccion * 100)).ToWords(CultureInfo.GetCultureInfo("es-AR")).ToUpper()} Centavos";
            return letras;
        }

        public static string NumeroConSeparadoresSinNulos(decimal? numero)
        {
            var objNumberFormatInfo = new NumberFormatInfo() { NumberGroupSeparator = "." };
            return numero.GetValueOrDefault().ToString("#,###.##", objNumberFormatInfo);
        }

        public static string CorregirFormatoMes(string cadena)
        {
            var date = DateTime.Parse(cadena);
            return date.ToString("MM.yyyy", CultureInfo.InvariantCulture);
        }

        public static string DivisaSimbolica(string divisa)
        {
            return divisa == "USD" ? Text.USD : Text.ARP;
        }
        
        public static string DevolverNumeroEnLetras(decimal numero)
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

        public static string DivisaEnLetras(string divisa)
        {
            return divisa == "USD" ? Text.Divisa_USD : Text.Divisa_ARP;
        }
        public static string NumeroConSeparadores(decimal? numero)
        {
            var objNumberFormatInfo = new NumberFormatInfo() { NumberGroupSeparator = "." };
            return numero.GetValueOrDefault().ToString("#,###.##", objNumberFormatInfo);
        }

        public static string CorregirFormatoFecha(string cadena)
        {
            var date = DateTime.Parse(cadena);
            string formatoFecha = date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            formatoFecha = formatoFecha.Replace("/", ".");
            return formatoFecha;
        }
    }
}
