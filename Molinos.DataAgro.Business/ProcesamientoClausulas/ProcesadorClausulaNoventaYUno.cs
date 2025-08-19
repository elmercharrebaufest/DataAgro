using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Business.ProcesamientoClausulas
{
    public class ProcesadorClausulaNoventaYUno : ProcesadorClausula<ClausulaNoventaYUno>
    {
        public ProcesadorClausulaNoventaYUno(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {
        }

        public override ResultadoClausula DevolverClausulas(ClausulaNoventaYUno clausula)
        {
            var res = new ResultadoClausula();
            if (clausula.Basico.BoletoId == (int)EnumBoletoCompraNet.CONFIRMA && clausula.Basico.CorredorId > 0)
            {
                res.Texto += $"Para descargas a realizarse en la planta ubicada en la Localidad de San Lorenzo, Pcia. de Santa Fe, el comprador otorgará el cupo con un código alfanumérico que obligatoriamente debe consignarse en el campo observaciones de cada carta de porte. En caso de que el vendedor remita camiones sin poseer cupo para la descarga, el comprador podrá, a su exclusiva opción, proceder a la descarga de los mismos, debiendo en tal caso el vendedor abonar al comprador U$S 10 (diez dólares) por tonelada en concepto de gastos extras por descargas no otorgadas.";
            }
            return res;
        }
    }
}
