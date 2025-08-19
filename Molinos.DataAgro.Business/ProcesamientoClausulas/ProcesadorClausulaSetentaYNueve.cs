using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Autofac.Extras.NLog;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Business.ProcesamientoClausulas
{
    public class ProcesadorClausulaSetentaYNueve : ProcesadorClausula<ClausulaSetentaYNueve>
    {
        public ProcesadorClausulaSetentaYNueve(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaSetentaYNueve clausula)
        {
            var res = new ResultadoClausula();
            if (clausula.Basico.BoletoId == (int)EnumBoletoCompraNet.CONFIRMA && clausula.Basico.CorredorId > 0)
            {
                res.Texto += $"A los efectos impositivos , y de acuerdo con las disposiciones legales, el vendedor declara en forma expresa que la mercadería NO es de su propia producción y NO es productor agropecuario. Operación de consignación.. Queda establecido que toda tasa, contribucion o impuesto municipal o provincial que grave la presente operación, estará a cargo del vendedor, si es que se impone por el origen de la mercadería vendida o por el lugar de descarga.";
            }
            return res;
        }
    }
}
