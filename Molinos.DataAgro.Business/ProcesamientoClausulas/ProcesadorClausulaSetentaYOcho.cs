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
    public class ProcesadorClausulaSetentaYOcho : ProcesadorClausula<ClausulaSetentaYOcho>
    {
        public ProcesadorClausulaSetentaYOcho(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaSetentaYOcho clausula)
        {
            var res = new ResultadoClausula();

            if (clausula.Basico.BoletoId == (int)EnumBoletoCompraNet.CONFIRMA && clausula.Basico.CorredorId > 0)
            {
                res.Texto += $"Queda establecido que las comunicaciones a los vendedores para la entrega de la mercadería, o cualquier otro hecho referido al presente contrato, incluidas las que deba hacer en su caso la Cámara Arbitral interviniente serán cursadas indistintamente a sola opción del comprador al vendedor con domicilio en ${clausula.Basico.ProveedorDireccion} ${clausula.Basico.ProveedorLocalidad}, o al corredor con domicilio en ${clausula.Basico.ProveedorDireccion} ${clausula.Basico.ProveedorLocalidad}.";
            }
            return res;
        }
    }
}
