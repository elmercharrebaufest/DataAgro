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
    public class ProcesadorClausulaSiete : ProcesadorClausula<ClausulaSiete>
    {
        public ProcesadorClausulaSiete(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaSiete clausula)
        {
            var res = new ResultadoClausula();

            if (clausula.Basico.MonedaId != "ARP  ")
            {
                res.Texto += "Las Partes acuerdan que la obligación será pagadera en pesos argentinos al tipo de cambio comprador publicado por el Banco de la " +
                    "Nación Argentina, salvo en el caso en que dicho tipo de cambio no sea el que resulte aplicable para la liquidación de divisas proveniente " +
                    "de la exportación de granos, en cuyo caso se tomará este último. En ese sentido, las Partes establecen de común acuerdo que la única forma " +
                    "de pago válida de las liquidaciones parciales y finales es la que se plasma en el presente instrumento, y la que rige según usos y " +
                    "costumbres del mercado de granos excluyendo cualquier otra que se pretenda o intente aplicar que atente contra el principio de autonomía " +
                    "de voluntad de las partes y de dichos usos y costumbres.";
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
