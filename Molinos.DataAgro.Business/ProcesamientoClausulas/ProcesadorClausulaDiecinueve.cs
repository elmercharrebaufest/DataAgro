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
    public class ProcesadorClausulaDiecinueve : ProcesadorClausula<ClausulaDiecinueve>
    {
        public ProcesadorClausulaDiecinueve(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaDiecinueve clausula)
        {
            var res = new ResultadoClausula();

            if (clausula.Basico.MaterialId == 3)
            {
                res.Texto += "El vendedor/procesador/acopiador/solicitante de servicios de elevación, fazón o acondicionamiento acepta que el grano " +
                    "de soja será analizado y en caso de detectarse la presencia de tecnologías patentadas se proporcionará al propietario de la tecnología " +
                    "o a quien éste designe la información relativa a dichos cargamentos. Toda controversia derivada de la aplicación de esta cláusula y/o pago " +
                    "de regalías será resuelta con el propietario de la tecnología patentada por la Cámara Arbitral de Cereales de la jurisdicción elegida por la " +
                    "parte que se considere con derecho al reclamo, a cuyo efecto las partes se sujetan y dan por aceptadas las condiciones establecidas en la " +
                    "reglamentación general aplicable. El tribunal elegido actuará como amigable componedor, con aplicación de las Reglas y Usos del Comercio de " +
                    "Granos y del Reglamento de Procedimientos aprobado por Decreto 931/98 y/o sus normas complementarias. La ejecución del laudo arbitral se " +
                    "efectuará ante los Tribunales Ordinarios de la Ciudad Autónoma de Buenos Aires. Si por cualquier motivo la controversia no pudiere ser " +
                    "resuelta por el tribunal arbitral elegido, queda pactado que serán los Tribunales Ordinarios de la Ciudad Autónoma de Buenos Aires los " +
                    "que resuelvan el asunto";
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
