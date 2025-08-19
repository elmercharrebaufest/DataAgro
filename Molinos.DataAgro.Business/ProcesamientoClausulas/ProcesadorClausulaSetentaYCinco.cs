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
    public class ProcesadorClausulaSetentaYCinco : ProcesadorClausula<ClausulaSetentaYCinco>
    {
        public ProcesadorClausulaSetentaYCinco(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto) : base(repositorio, log, estadoBoleto)
        {
        }
        public override ResultadoClausula DevolverClausulas(ClausulaSetentaYCinco clausula)
        {
            var res = new ResultadoClausula();

            if (clausula.Basico.BoletoId == (int)EnumBoletoCompraNet.CONFIRMA && clausula.Basico.CorredorId > 0)
            {
                res.Texto += $"Las entregas y recibos se efectuarán desde el {clausula.Basico.FechaDesdeFormateado} hasta el {clausula.Basico.FechaHastaFormateado} inclusive, en SAN LORENZO- Prov Santa Fe sobre Camion. Procedencia de la mercadería {clausula.Basico.ProveedorLocalidad}, {clausula.Basico.ProveedorProvincia}.";
            }

            return res;
        }
    }
}
