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
    public class ProcesadorClausulaSesentaYCuatro : ProcesadorClausula<ClausulaSesentaYCuatro>
    {
        public ProcesadorClausulaSesentaYCuatro(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto) : base(repositorio, log, estadoBoleto)
        {
        }
        public override ResultadoClausula DevolverClausulas(ClausulaSesentaYCuatro clausula)
        {
            //No aplica a carta oferta porque no se sella. Es solo para boleto físico y confirma

            var res = new ResultadoClausula();

            if (clausula.Basico.BoletoId == (int)EnumBoletoCompraNet.CONFIRMA && clausula.Basico.CorredorId == 0)
            {
                if (clausula.Basico.ProveedorId > 0)
                {
                    res.Texto += $"Queda establecido que las comunicaciones a los vendedores para la entrega de la mercadería, o cualquier otro hecho referido al presente contrato serán cursadas por el comprador al vendedor con domicilio en {clausula.Basico.ProveedorDireccion}.";
                }
            }

            return res;
        }
    }
}
