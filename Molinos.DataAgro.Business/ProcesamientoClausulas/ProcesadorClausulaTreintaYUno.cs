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
    public class ProcesadorClausulaTreintaYUno : ProcesadorClausula<ClausulaTreintaYUno>
    {
        public ProcesadorClausulaTreintaYUno(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaTreintaYUno clausula)
        {
            var res = new ResultadoClausula();
            if (clausula.Basico.TipoNegocioId == 1 && clausula.Basico.Canje == true)
            {
                res.Texto += $"En caso de que el Vendedor entregare la Mercadería en forma total y la Mercadería no cumpliera -total o parcialmente- con las " +
                    $"condiciones Fábrica, y ello implicaré un saldo a favor del Comprador, entonces el Vendedor quedará obligado a proceder en idénticas " +
                    $"condiciones a las mencionadas en la cláusula previa de este Boleto.";
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
