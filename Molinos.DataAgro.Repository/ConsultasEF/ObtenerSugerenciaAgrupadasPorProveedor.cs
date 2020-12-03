using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class ObtenerSugerenciaAgrupadasPorProveedor : IConsultaEscalar<IList<SugerenciaCupoDto>>
    {
        private readonly DateTime desde;
        private readonly DateTime hasta;
        private readonly int comercialId;
        private readonly int materialId;
        private readonly string centroId;

        public ObtenerSugerenciaAgrupadasPorProveedor(DateTime desde, DateTime hasta, int comercialId, int materialId, string centroId)
        {
            this.desde = desde;
            this.hasta = hasta;
            this.comercialId = comercialId;
            this.materialId = materialId;
            this.centroId = centroId;

        }
        private static IList<SugerenciaCupoDto> Query(DbContext contexto, DateTime desde, DateTime hasta, int comercialId, int materialId, string centroId)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            var temp = contexto.Set<SugerenciaCupo>()
                .Where(x => x.Centro.CodigoSap == centroId && x.FechaSugerida >= desde
                && x.FechaSugerida <= hasta && x.MaterialId == materialId
                && x.ComercialId == comercialId && x.Aceptado == null)
                .GroupBy(x => new { x.ProveedorId, x.FechaSugerida })
                .Select(sugerido => new SugerenciaCupoDto
                {
                    Id = sugerido.Key.ProveedorId.Value,
                    FechaSugerida = sugerido.Key.FechaSugerida,
                    ProveedorDesc = sugerido.Select(x => x.Proveedor.RazonSocial).FirstOrDefault(),
                    CantidadDeCupos = sugerido.Sum(x => x.CantidadDeCupos),
                    ComercialId = sugerido.Select(x => x.Comercial.ComercialId).FirstOrDefault(),
                    MaterialId = sugerido.Select(x => x.Material.MaterialId).FirstOrDefault(),
                    Aceptado = sugerido.Select(x => x.Aceptado).FirstOrDefault(),
                }).ToList();

            var adm = contexto.Set<AdministracionCupo>()
                .Where(x => x.Centro.CodigoSap == centroId && x.MaterialId == materialId
                && x.ComercialId == comercialId && x.EstadoId == (int)EnumEstadoAdministracionCupo.EstadoPendienteAdministracionCupo)
                .GroupBy(x => new { x.ProveedorId, x.Fecha })
                .Select(sugerido => new SugerenciaCupoDto
                {
                    Id = sugerido.Key.ProveedorId.Value,
                    FechaSugerida = sugerido.Key.Fecha,
                    CantidadDeCupos = sugerido.Sum(x => x.CantidadCupo) + sugerido.Sum(x => x.CantidadFleteProcedencia),
                }).ToList();

            foreach (var t in temp)
            {
                var a = adm.Where(x => x.Id == t.Id);
                if(a != null)
                {
                    t.CantidadDeCupos -= a.Sum(x => x.CantidadDeCupos);
                }
            }
            return temp;
        }

        public virtual IList<SugerenciaCupoDto> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, desde, hasta, comercialId, materialId, centroId);
            }
        }
    }
}
