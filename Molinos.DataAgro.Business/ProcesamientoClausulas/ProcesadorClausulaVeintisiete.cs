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
    public class ProcesadorClausulaVeintisiete : ProcesadorClausula<ClausulaVeintisiete>
    {
        public ProcesadorClausulaVeintisiete(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaVeintisiete clausula)
        {
            var res = new ResultadoClausula();
            if (clausula.Basico.TipoNegocioId == 1 && clausula.Basico.Canje == true)
            {
                res.Texto += $"Queda expresamente establecido que la cantidad de Mercadería a ser entregada será determinada: En función del precio de acuerdo a " +
                    $"las fijaciones realizadas por el Vendedor hasta la Fecha Límite de Fijación, y en función de las condiciones en las que la Mercadería fue " +
                    $"entregada.";
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
