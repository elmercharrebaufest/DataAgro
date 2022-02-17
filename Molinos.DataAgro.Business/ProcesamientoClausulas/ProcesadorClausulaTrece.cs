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
    public class ProcesadorClausulaTrece : ProcesadorClausula<ClausulaTrece>
    {
        public ProcesadorClausulaTrece(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaTrece clausula)
        {
            var res = new ResultadoClausula();

            if (clausula.Basico.CorredorId == 0 && clausula.Basico.Canje != true)
            {
                res.Texto += "Por medio de la presente la VENDEDORA autoriza en forma expresa e irrevocable a la COMPRADORA a compensar en los términos " +
                    "de los artículos 921, siguientes y concordantes del Código Civil y Comercial de la Nación, la totalidad de los CRÉDITOS que pudieran " +
                    "llegar a existir a su favor, en virtud de la venta de agroinsumos, subproductos, servicios y/o gastos derivados de la operación comercial " +
                    "con la VENDEDORA.";
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
