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
    public class ProcesadorClausulaVeintitres : ProcesadorClausula<ClausulaVeintitres>
    {
        public ProcesadorClausulaVeintitres(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaVeintitres clausula)
        {
            var res = new ResultadoClausula();

            if (clausula.Basico.TipoNegocioId == 1 && clausula.Basico.Canje == true)
            {
                res.Texto += $"A los efectos del presente contrato LAS PARTES acuerdan que el valor total de EL INSUMO es de {clausula.Basico.MonedaCanjeDescripcion } " +
                    $"(ARP ES PESOS ARGENTINOS Y USD ES DÓLAR AMERICANO)  {clausula.Basico.Monto } ({DevolverNumeroEnLetras(clausula.Basico.Monto.Value)}) " +
                    $"(en adelante el {"COSTO DEL INSUMO"}). EL COSTO DEL INSUMO comprende el costo total de EL INSUMO en dólares estadounidenses con más todos los impuestos nacionales, " +
                    "provinciales y/o municipales que resulten aplicables a los mismos.";
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
