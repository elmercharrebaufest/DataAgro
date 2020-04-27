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
                .Where(x=> x.Centro.CodigoSap == centroId) 
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
                }).Where(x => x.FechaSugerida >= desde && x.FechaSugerida <= hasta  && x.MaterialId == materialId && x.ComercialId == comercialId && x.Aceptado == null).ToList();
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
