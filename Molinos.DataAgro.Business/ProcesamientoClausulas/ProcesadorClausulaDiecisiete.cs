using NLog;
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
    public class ProcesadorClausulaDiecisiete : ProcesadorClausula<ClausulaDiecisiete>
    {
        public ProcesadorClausulaDiecisiete(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {
        }

        public override ResultadoClausula DevolverClausulas(ClausulaDiecisiete clausula)
        {
            var res = new ResultadoClausula();
            var datosBoleto = EstadoBoleto.EstadoBoleto(clausula.Basico.ContratoSAP, "");
            var condiciones = datosBoleto.CondicionFijacion.FirstOrDefault();
            //SI EL NEGOCIO ES DE TIPO A FIJAR Y NO ES POSICIÓN PASE
            if (clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && (!clausula.Basico.TipoPosicionCBOTId.HasValue || clausula.Basico.TipoPosicionCBOTId.Value != (int)EnumTipoPosicionCBOT.PASE) && clausula.Basico.EsFason != true && clausula.Basico.PrestamoDevolucion != true)
            {
                if (clausula.Basico.BoletoId != (int)EnumBoletoCompraNet.CARTA_OFERTA)
                {
                    res.Texto += $"El precio de la mercadería objeto del presente contrato, se fijará cualquier día hábil a elección del vendedor. " +
                                 $"El vendedor comunicará al comprador el día elegido para la fijación de precio por {clausula.Basico.CondicionFijacionDescripcion} desde el " +
                                 $"{CorregirFormatoFecha(condiciones.FechaDesde)} hasta {CorregirFormatoFecha(condiciones.FechaHasta)} en cualquier día hábil a elección del vendedor, siendo la cantidad de " +
                                 $"{condiciones.Meins} de fijación mínima permitida {NumeroConSeparadores(condiciones.CantidadMinima)} Kg y la cantidad máxima permitida {NumeroConSeparadores(condiciones.CantidadMaxima)} Kg. " +
                                 $"Únicamente a los efectos del impuesto de sellos las partes acuerdan que el precio de referencia corresponde a Pizarra Rosario.";
                }
                else
                {
                    res.Texto += $"El precio de la mercadería objeto del presente contrato, se fijará cualquier día hábil a elección del vendedor. " +
                                 $"El vendedor comunicará al comprador el día elegido para la fijación de precio por {clausula.Basico.CondicionFijacionDescripcion} desde el " +
                                 $"{CorregirFormatoFecha(condiciones.FechaDesde)} hasta el {CorregirFormatoFecha(condiciones.FechaHasta)} en cualquier día hábil a elección del vendedor, siendo la cantidad de " +
                                 $"{condiciones.Meins} de fijación mínima permitida {NumeroConSeparadores(condiciones.CantidadMinima)} Kg. y la cantidad máxima permitida {NumeroConSeparadores(condiciones.CantidadMaxima)} Kg.";
                }
            }
            //SI EL NEGOCIO ES DE TIPO A FIJAR Y ES POSICIÓN PASE
            if (clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && clausula.Basico.TipoPosicionCBOTId.HasValue && clausula.Basico.TipoPosicionCBOTId.Value == (int)EnumTipoPosicionCBOT.PASE && clausula.Basico.EsFason != true && clausula.Basico.PrestamoDevolucion != true)
            {
                if (clausula.Basico.ImporteBonificacion.HasValue && clausula.Basico.ImporteBonificacion.Value != 0)
                {
                    //var bonificacion = clausula.Basico.AperturaPrecios.FirstOrDefault(x => x.Importe != 0 && x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Bonificaciones);

                    res.Texto += $"El precio de la mercadería objeto del presente contrato, se fijará cualquier día hábil a elección del vendedor. El precio será el resultante " +
                    $"de ajustar el precio de la mercadería del día elegido para la fijación conforme se establece más adelante, con el precio de la Posición {clausula.Basico.PosicionCBOT} " +
                    $"del Mercado a Término (MAT) más una bonificación del {clausula.Basico.MonedaBonificacion} {clausula.Basico.ImporteBonificacion?.ToString("N", new CultureInfo("es-AR"))} ({DevolverNumeroEnLetras(clausula.Basico.ImporteBonificacion.Value)}). " +
                    $"El vendedor comunicará al comprador el día elegido para la fijación de precio por Mercado Disponible de Molinos Agro SA hasta el {CorregirFormatoFecha(condiciones.FechaHasta)} en cualquier día hábil a elección del vendedor. " +
                    $"Dicho precio será ajustado con el precio Posición {clausula.Basico.PosicionCBOT} MAT, al que se le aplicará además una bonificación de {clausula.Basico.MonedaBonificacion} {clausula.Basico.ImporteBonificacion?.ToString("N", new CultureInfo("es-AR"))}, " +
                    $"quedando de esa manera definido el Precio de cada fijación que realice el Vendedor. " +
                    $"A los efectos el impuesto de sellos, únicamente, las partes acuerdan que el precio de referencia corresponde a Pizarra Rosario Soja/Ciega.";
                }
                else
                {
                    res.Texto += $"El precio de la mercadería objeto del presente contrato, se fijará cualquier día hábil a elección del vendedor. El precio será el resultante de " +
                    $"ajustar el precio de la mercadería del día elegido para la fijación conforme se establece más adelante, con el precio de la Posición {clausula.Basico.PosicionCBOT} " +
                    $"del Mercado a Término (MAT). El vendedor comunicará al comprador el día elegido para la fijación de precio por Mercado Disponible de Molinos Agro SA hasta el " +
                    $"{CorregirFormatoFecha(condiciones.FechaHasta)} en cualquier día hábil a elección del vendedor. Dicho precio será ajustado con el precio Posición {clausula.Basico.PosicionCBOT} MAT, " +
                    $"quedando de esa manera definido el Precio de cada fijación que realice el Vendedor.";
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

        private string NumeroConSeparadores(decimal? numero)
        {
            var objNumberFormatInfo = new NumberFormatInfo() { NumberGroupSeparator = "." };
            return numero.GetValueOrDefault().ToString("#,###.##", objNumberFormatInfo);
        }

        private string CorregirFormatoFecha(string cadena)
        {
            var date = DateTime.Parse(cadena);
            string formatoFecha = date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            formatoFecha = formatoFecha.Replace("/", ".");
            return formatoFecha;
        }
    }
}