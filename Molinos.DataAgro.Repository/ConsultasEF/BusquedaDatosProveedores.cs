using Kendo.DynamicLinq;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class BusquedaDatosProveedores : IConsultaEscalar<DataSourceResult>

    {
        private readonly DataSourceRequest request;
        private readonly List<int> comerciales;

        public BusquedaDatosProveedores(DataSourceRequest request,List<int> comerciales)
        {
            this.request = request;
            this.comerciales = comerciales;
        }

        private static DataSourceResult Query(DbContext contexto, DataSourceRequest request, List<int> comerciales)
        {
            var resultado =
                 from prove in contexto.Set<Proveedor>()
                  join pCom in contexto.Set<ProveedorComercial>() on prove.ProveedorId equals pCom.ProveedorId 
                  join est in contexto.Set<Estado>() on prove.EstadoId equals est.EstadoId into estados
                  from ests in estados.DefaultIfEmpty()
                  where comerciales.Contains(pCom.Comercial.ComercialId)
                  orderby prove.ProveedorId
                  
                 select new DatosProveedor()
                 {

                     AreaDeInfluencia = prove.AreaInfluencia.Descripcion,
                     Bolsa = prove.BoletoCompraNet.Descripcion,
                     Calificacion = prove.Calificacion,
                     CanalDeOperacion =(from co in contexto.Set<ProveedorCanalOperacion>() where prove.ProveedorId == co.ProveedorId select co.CanalOperacion.Descripcion).FirstOrDefault(),
                     Clasificacion = prove.ClasificacionCompraNet.Descripcion,
                     ClienteMoa = (prove.ClienteMOA.HasValue && prove.ClienteMOA.Value) ? "SI" : "NO",
                     CodPostal = prove.CodigoPostal,
                     Comentario = prove.Observaciones,
                     Comerciales = from pc in contexto.Set<ProveedorComercial>() where pc.ProveedorId == pCom.ProveedorId select pc.Comercial.Apellido,
                     Comercial = pCom.Comercial.Apellido,
                     Condicion = (from con in contexto.Set<ProveedorCondicion>() where prove.ProveedorId == con.ProveedorId  select con.Condicion.Descripcion).FirstOrDefault(),
                     Consignatario = prove.Consignatario.HasValue && prove.Consignatario.Value ? "SI" : "NO",
                     Cuit = prove.CUIT,
                     Destinatario = (from con in contexto.Set<ProveedorDestinatario>() where prove.ProveedorId == con.ProveedorId select con.Destinatario.Descripcion).FirstOrDefault(),
                     Domicilio = prove.Direccion,
                     Estado = prove.Estado.Descripcion,
                     FechaAlta = prove.FechaAlta.HasValue ? DbFunctions.TruncateTime(prove.FechaAlta.Value) : DateTime.Today,
                     Intermediario = prove.Intermediario,
                     Localidad = prove.Localidad.Nombre,
                     Provincia = prove.Provincia.Nombre,
                     RazonSocial = prove.RazonSocial,
                     Segmentacion = prove.Segmentacion.Descripcion,
                     TipoBoleto = prove.BoletoCompraNet.Descripcion,
                     Zona = prove.GrupoCompras,
                     ComisionPorcentaje = prove.ComisionPorcentaje ?? 0,
                     LocalidadCompraNet = prove.LocalidadCompraNet.Nombre,
                     ProvinciaCompraNet = prove.ProvinciaCompraNet.Nombre,
                 };

            GridHelper.TruncateTime(request.Filter, ref resultado);
            return resultado.ToDataSourceResult(request);
        }

        public virtual DataSourceResult Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, request, comerciales);
            }
        }
    }
}
