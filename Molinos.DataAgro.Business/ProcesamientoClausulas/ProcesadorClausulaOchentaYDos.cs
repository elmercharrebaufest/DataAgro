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
    public class ProcesadorClausulaOchentaYDos : ProcesadorClausula<ClausulaOchentaYDos>
    {
        public ProcesadorClausulaOchentaYDos(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaOchentaYDos clausula)
        {
            var res = new ResultadoClausula();
            if (clausula.Basico.BoletoId == (int)EnumBoletoCompraNet.CONFIRMA && clausula.Basico.CorredorId > 0)
            {
                res.Texto += $"A los efectos del presente boleto las partes constituyen domicilio especial en Secretaría de la Cámara Arbitral de Cereales de la Bolsa de Comercio de Rosario, calle Córdoba 1402, de la ciudad de Rosario.";
            }
            return res;
        }
    }
}
