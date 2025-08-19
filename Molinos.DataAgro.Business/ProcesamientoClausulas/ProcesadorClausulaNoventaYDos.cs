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
    public class ProcesadorClausulaNoventaYDos : ProcesadorClausula<ClausulaNoventaYDos>
    {
        public ProcesadorClausulaNoventaYDos(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {
        }

        public override ResultadoClausula DevolverClausulas(ClausulaNoventaYDos clausula)
        {
            var res = new ResultadoClausula();
            if (clausula.Basico.BoletoId == (int)EnumBoletoCompraNet.CONFIRMA && clausula.Basico.CorredorId > 0)
            {
                res.Texto += $"Por medio de la presente la VENDEDORA autoriza en forma expresa e irrevocable a la COMPRADORA a compensar en los términos de los artículos 921, siguientes y concordantes del Código Civil y Comercial de la Nación, la totalidad de los CRÉDITOS que pudieran llegar a existir a su favor, en virtud de la venta de agroinsumos, subproductos, servicios y/o gastos derivados de la operación comercial con la VENDEDORA.";
            }
            return res;
        }
    }
}
