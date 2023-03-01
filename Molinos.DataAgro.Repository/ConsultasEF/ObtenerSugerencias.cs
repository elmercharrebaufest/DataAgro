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
    public class ObtenerSugerencias : IConsultaEscalar<IList<SugerenciaCupoDto>>
    {
        private readonly DateTime desde;
        private readonly DateTime hasta;
        private readonly int comercialId;
        private readonly int materialId;
        private readonly string centroId;

        public ObtenerSugerencias(DateTime desde, DateTime hasta, int comercialId, int materialId, string centroId)
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
            var sugerencias = contexto.Set<SugerenciaCupo>()
                .Where(x => x.Centro.CodigoSap == centroId && x.FechaSugerida >= desde
                && x.FechaSugerida <= hasta && x.MaterialId == materialId
                && x.ComercialId == comercialId && x.Aceptado == null
                && (x.Solicitudes.All(y=> y.EstadoId != (int)EnumEstadoAdministracionCupo.Pendiente) || x.Solicitudes.Count() == 0 ))
                .Select(sugerido => new SugerenciaCupoDto
                {
                    Id = sugerido.Id,
                    FechaSugerida = sugerido.FechaSugerida,
                    ProveedorDesc = sugerido.Proveedor.RazonSocial,
                    CantidadDeCupos = sugerido.CantidadDeCupos,
                    ComercialId = sugerido.Comercial.ComercialId,
                    MaterialId = sugerido.Material.MaterialId,
                    Aceptado = sugerido.Aceptado,
                    PuntuacionesString = sugerido.Puntuaciones,
                    MonedaDesc = sugerido.Moneda.Descripcion,
                    ProveedorCUIT = sugerido.Proveedor.CUIT,
                    TipoNegocioDesc = sugerido.TipoNegocio.Descripcion,
                    ContratoSAP = sugerido.ContratoSAP,
                    ProveedorId = sugerido.ProveedorId,
                    MaterialDesc = sugerido.Material.Descripcion,
                    MonedaId = sugerido.Moneda.MonedaId,
                    ZonaDescrip = sugerido.ZonaCupo.Descripcion,
                    Precio = sugerido.Precio,
                    PuntuacionTotal = sugerido.Puntuacion,
                    KgNegocio = sugerido.KgNegocio,
                    KgPendienteAplicar = sugerido.KgPendienteAplicar,
                    Sustentable = sugerido.Negocio != null && sugerido.Negocio is Contrato && (sugerido.Negocio as Contrato).Sustentable == true ? true : false,
                    EPA = sugerido.Negocio != null && sugerido.Negocio is Contrato && (sugerido.Negocio as Contrato).EPA == true ? true : false
                }).ToList();

            //var solicitudes = contexto.Set<AdministracionCupo>()
            //    .Where(x => x.Centro.CodigoSap == centroId && x.MaterialId == materialId
            //    && x.ComercialId == comercialId && x.EstadoId == (int)EnumEstadoAdministracionCupo.EstadoPendienteAdministracionCupo
            //    && x.TipoAdministracionCupoId == (int)EnumTipoAdministracionCupo.Algoritmo)               
            //    .Select(sugerido => new SugerenciaCupoDto
            //    {
            //        ProveedorId = sugerido.ProveedorId.Value,
            //        FechaSugerida = sugerido.Fecha,
            //        CantidadDeCupos = sugerido.CantidadCupo + sugerido.CantidadFleteProcedencia,
            //        MaterialId = sugerido.Material.MaterialId,
            //    }).ToList();

            //foreach (var a in solicitudes)
            //{
            //    var sugerencia = sugerencias.Where(x => x.ProveedorId == a.ProveedorId && x.MaterialId == a.MaterialId /*&& x.FechaSugerida == a.FechaSugerida*/).FirstOrDefault();
            //    if(sugerencia != null)
            //    {
            //        sugerencias.Remove(sugerencia);                    
            //    }
            //}
            return sugerencias.ToList();
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
