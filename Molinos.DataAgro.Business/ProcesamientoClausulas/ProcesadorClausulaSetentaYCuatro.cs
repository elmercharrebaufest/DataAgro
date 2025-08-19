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
    public class ProcesadorClausulaSetentaYCuatro : ProcesadorClausula<ClausulaSetentaYCuatro>
    {
        public ProcesadorClausulaSetentaYCuatro(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto) : base(repositorio, log, estadoBoleto)
        {
        }
        public override ResultadoClausula DevolverClausulas(ClausulaSetentaYCuatro clausula)
        {
            var res = new ResultadoClausula();


            if (clausula.Basico.BoletoId == (int)EnumBoletoCompraNet.CONFIRMA &&clausula.Basico.CorredorId > 0)
            {
                res.Texto += $"El/los señores {clausula.Basico.RazonSocialProveedor} vende/n por cuenta y orden de los señores {clausula.Basico.Corredor} con domicilio en {clausula.Basico.ProveedorDireccion} {clausula.Basico.ProveedorLocalidad} y el/los señor/es MOLINOS AGRO S.A. con domicilio en URUGUAY 4075, Victoria compra/n la cantidad: desde {clausula.Basico.Cantidad} Kilos hasta {clausula.Basico.Cantidad} Kilos, de {clausula.Basico.Material} cosecha {clausula.Basico.Campania} ";
                res.Texto += $"según condiciones Fábrica al precio de {clausula.Basico.Moneda} {clausula.Basico.Precio} por Tonelada mas IVA puesto en SAN LORENZO- Prov Santa Fe.";
            }
            return res;
        }
    }
}
