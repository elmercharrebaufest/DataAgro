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
    public class ProcesadorClausulaSesenta : ProcesadorClausula<ClausulaSesenta>
    {
        public ProcesadorClausulaSesenta(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto) : base(repositorio, log, estadoBoleto)
        {
        }
        public override ResultadoClausula DevolverClausulas(ClausulaSesenta clausula)
        {
            //No aplica a carta oferta porque no se sella. Es solo para boleto físico y confirma

            var res = new ResultadoClausula();
            if (clausula.Basico.BoletoId == (int)EnumBoletoCompraNet.CONFIRMA && clausula.Basico.CorredorId == 0)
            {
                res.Texto += $"Los señores {clausula.Basico.RazonSocialProveedor} con domicilio en {clausula.Basico.ProveedorDireccion},{clausula.Basico.ProveedorLocalidad}, {clausula.Basico.ProveedorProvincia} venden,";
                res.Texto += $" y los señores MOLINOS AGRO S.A. con domicilio en URUGUAY 4075 , Victoria compran la cantidad: desde {clausula.Basico.Cantidad} Kilos hasta {clausula.Basico.Cantidad} Kilos, de {clausula.Basico.Material} cosecha {clausula.Basico.Campania} según condiciones Fábrica al precio de {clausula.Basico.Moneda} {clausula.Basico.Precio} por Tonelada mas IVA puesto en SAN LORENZO- Prov Santa Fe.";
            }

            return res;
        }



    }
}
