using Autofac.Extras.NLog;
using Humanizer;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Business.ProcesamientoClausulas
{
    public class ProcesadorClausulaSesentaYUno : ProcesadorClausula<ClausulaSesentaYUno>
    {
        public ProcesadorClausulaSesentaYUno(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto) : base(repositorio, log, estadoBoleto)
        {
        }
        public override ResultadoClausula DevolverClausulas(ClausulaSesentaYUno clausula)
        {
            //No aplica a carta oferta porque no se sella. Es solo para boleto físico y confirma

            var res = new ResultadoClausula();


            if (clausula.Basico.BoletoId == (int)EnumBoletoCompraNet.CONFIRMA && clausula.Basico.CorredorId == 0)
            {
                res.Texto += $"Las entregas y recibos se efectuarán desde el {clausula.Basico.FechaDesdeFormateado} hasta el {clausula.Basico.FechaHastaFormateado} inclusive, en SAN LORENZO- Prov Santa Fe sobre Camion. Procedencia de la mercadería {clausula.Basico.ProveedorLocalidad}, {clausula.Basico.ProveedorProvincia}.";
            }
            return res;
        }
    }
}
