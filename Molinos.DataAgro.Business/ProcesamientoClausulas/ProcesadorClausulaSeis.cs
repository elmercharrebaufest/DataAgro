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
    public class ProcesadorClausulaSeis : ProcesadorClausula<ClausulaSeis>
    {
        public ProcesadorClausulaSeis(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaSeis clausula)
        {
            var res = new ResultadoClausula();

            res.Texto += $"El importe del sellado del presente Contrato como los honorarios de la Cámara Arbitral de la Bolsa de Cereales de { clausula.Basico.BolsaDescripcion }, " +
                 $"será abonado por: ";
            if (clausula.Basico.SelCargoVendedor == true)
            {
                res.Texto += $"el vendedor ";
            }
            else if (clausula.Basico.SelCargoMOA == true)
            {
                res.Texto += $"el comprador ";
            }
            else
            {
                res.Texto += $"las partes en partes iguales.";
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
