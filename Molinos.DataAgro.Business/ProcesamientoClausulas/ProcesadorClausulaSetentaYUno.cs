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
    public class ProcesadorClausulaSetentaYUno : ProcesadorClausula<ClausulaSetentaYUno>
    {
        public ProcesadorClausulaSetentaYUno(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto) : base(repositorio, log, estadoBoleto)
        {
        }

        public override ResultadoClausula DevolverClausulas(ClausulaSetentaYUno clausula)
        {
            var res = new ResultadoClausula();

            if (clausula.Basico.BoletoId == (int)EnumBoletoCompraNet.CONFIRMA && clausula.Basico.CorredorId == 0)
            {
                res.Texto += $"Todas las comunicaciones relativas al presente contrato, incluidas las que deba hacer en su caso la Cámara Arbitral interviniente, se considerarán válidamente efectuadas si se realizan al domicilio que la parte ha constituido en la cláusula primera de este contrato.";
            }
            return res;
        }

    }
}
