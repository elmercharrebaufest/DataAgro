using NLog;
using Molinos.DataAgro.Entities.ClausulasBoleto.Confirma;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Globalization;
using System.Linq;

namespace Molinos.DataAgro.Business.ClausulasBoleto.Confirma
{
    public class ProcesadorClausulaConfirmaTres : ProcesadorClausulaConfirma<ClausulaConfirmaTres>
    {
        public ProcesadorClausulaConfirmaTres(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
            : base(repositorio, log, estadoBoleto) { }

        public override ResultadoClausula DevolverClausulas(ClausulaConfirmaTres clausula)
        {
            var res = new ResultadoClausula();
            var datosBoleto = EstadoBoleto.EstadoBoleto(clausula.Basico.ContratoSAP, "");
            var condiciones = datosBoleto.CondicionFijacion.FirstOrDefault();
            //SI EL NEGOCIO ES DE TIPO A FIJAR Y NO ES POSICIÓN PASE
            if (clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && (!clausula.Basico.TipoPosicionCBOTId.HasValue || clausula.Basico.TipoPosicionCBOTId.Value != (int)EnumTipoPosicionCBOT.PASE) && clausula.Basico.EsFason != true && clausula.Basico.PrestamoDevolucion != true)
            {
                    res.Texto += $"El precio de la mercadería objeto del presente contrato, se fijará cualquier día hábil a elección del vendedor. " +
                                 $"El vendedor comunicará al comprador el día elegido para la fijación de precio por {clausula.Basico.CondicionFijacionDescripcion} desde el " +
                                 $"{MetodosUtiles.CorregirFormatoFecha(condiciones.FechaDesde)} hasta {MetodosUtiles.CorregirFormatoFecha(condiciones.FechaHasta)} en cualquier día hábil a elección del vendedor, siendo la cantidad de " +
                                 $"{condiciones.Meins} de fijación mínima permitida {MetodosUtiles.NumeroConSeparadoresSinNulos(condiciones.CantidadMinima)} y la cantidad máxima permitida {MetodosUtiles.NumeroConSeparadoresSinNulos(condiciones.CantidadMaxima)}. " +
                                 $"Únicamente a los efectos del impuesto de sellos las partes acuerdan que el precio de referencia corresponde a Pizarra Rosario.";
            }
            //SI EL NEGOCIO ES DE TIPO A FIJAR Y ES POSICIÓN PASE
            if (clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && clausula.Basico.TipoPosicionCBOTId.HasValue && clausula.Basico.TipoPosicionCBOTId.Value == (int)EnumTipoPosicionCBOT.PASE && clausula.Basico.EsFason != true && clausula.Basico.PrestamoDevolucion != true)
            {
                if (clausula.Basico.ImporteBonificacion.HasValue && clausula.Basico.ImporteBonificacion.Value != 0)
                {
                    //var bonificacion = clausula.Basico.AperturaPrecios.FirstOrDefault(x => x.Importe != 0 && x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Bonificaciones);

                    res.Texto += $"El precio de la mercadería objeto del presente contrato, se fijará cualquier día hábil a elección del vendedor. El precio será el resultante " +
                    $"de ajustar el precio de la mercadería del día elegido para la fijación conforme se establece más adelante, con el precio de la Posición {clausula.Basico.PosicionCBOT} " +
                    $"del Mercado a Término (MAT) más una bonificación del {clausula.Basico.MonedaBonificacion} {clausula.Basico.ImporteBonificacion?.ToString("N", new CultureInfo("es-AR"))} ({MetodosUtiles.DevolverNumeroEnLetrasConDivisa(clausula.Basico.ImporteBonificacion.Value, clausula.Basico.MonedaBonificacion)}). " +
                    $"El vendedor comunicará al comprador el día elegido para la fijación de precio por Mercado Disponible de Molinos Agro SA hasta el {MetodosUtiles.CorregirFormatoFecha(condiciones.FechaHasta)} en cualquier día hábil a elección del vendedor. " +
                    $"Dicho precio será ajustado con el precio Posición {clausula.Basico.PosicionCBOT} MAT, al que se le aplicará además una bonificación de {clausula.Basico.MonedaBonificacion} {clausula.Basico.ImporteBonificacion?.ToString("N", new CultureInfo("es-AR"))}, " +
                    $"quedando de esa manera definido el Precio de cada fijación que realice el Vendedor. " +
                    $"A los efectos el impuesto de sellos, únicamente, las partes acuerdan que el precio de referencia corresponde a Pizarra Rosario Soja/Ciega.";
                }
                else
                {
                    res.Texto += $"El precio de la mercadería objeto del presente contrato, se fijará cualquier día hábil a elección del vendedor. El precio será el resultante de " +
                    $"ajustar el precio de la mercadería del día elegido para la fijación conforme se establece más adelante, con el precio de la Posición {clausula.Basico.PosicionCBOT} " +
                    $"del Mercado a Término (MAT). El vendedor comunicará al comprador el día elegido para la fijación de precio por Mercado Disponible de Molinos Agro SA hasta el " +
                    $"{MetodosUtiles.CorregirFormatoFecha(condiciones.FechaHasta)} en cualquier día hábil a elección del vendedor. Dicho precio será ajustado con el precio Posición {clausula.Basico.PosicionCBOT} MAT, " +
                    $"quedando de esa manera definido el Precio de cada fijación que realice el Vendedor.";
                }
            }
            return res;
        }
    }
}
