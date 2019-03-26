using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerCorredoresComercialExportarAll : IConsulta<ContactoAll>
    {
        private readonly List<int> idComerciales;
        private readonly int comercialOriginalId;

        public TraerCorredoresComercialExportarAll(List<int> idComerciales, int comercialOriginalId)
        {
            this.idComerciales = idComerciales;
            this.comercialOriginalId = comercialOriginalId;
        }

        private static List<ContactoAll> Query(DbContext contexto, List<int> idComerciales, int comercialOriginalId)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            var resultado =
                from proveedorComercial in contexto.Set<ProveedorComercial>() 
                join proveedor in contexto.Set<Proveedor>() on proveedorComercial.ProveedorId equals proveedor.ProveedorId
                join ContactoComercial in contexto.Set<ContactoComercial>() on proveedor.ProveedorId equals ContactoComercial.ProveedorId into contactos
                from con in contactos.DefaultIfEmpty()
                join comercial in contexto.Set<Comercial>() on proveedorComercial.ComercialId equals comercial.ComercialId
                join Estado in contexto.Set<Estado>() on proveedor.EstadoId equals Estado.EstadoId into estados
                from ests in estados.DefaultIfEmpty()
                where idComerciales.Contains(proveedorComercial.ComercialId)
                select new ContactoAll
                {
                    AreaDeInfluencia = proveedor.AreaInfluencia.Descripcion,
                    Bolsa = proveedor.BoletoCompraNet.Descripcion,
                    Calificacion = proveedor.Calificacion,
                    CanalDeOperacion = "",
                    Clasificacion = proveedor.ClasificacionCompraNet.Descripcion,
                    ClienteMoa = (proveedor.ClienteMOA.HasValue && proveedor.ClienteMOA.Value) ? "SI" : "NO",
                    CodPostal = proveedor.CodigoPostal,
                    Comentario = "",
                    Comercial = comercial.Apellido + " " + comercial.Nombres,
                    Condicion = "",
                    Consignatario = proveedor.Consignatario.HasValue && proveedor.Consignatario.Value ? "SI" : "NO",
                    Cuit = proveedor.CUIT,
                    Destinatario = "",
                    Domicilio = proveedor.Direccion,
                    Estado = proveedor.Estado.Descripcion,
                    FechaAlta = proveedor.FechaAlta.HasValue ? proveedor.FechaAlta.Value : DateTime.Now,
                    Intermediario = proveedor.Intermediario,
                    Localidad = proveedor.LocalidadCompraNet.Nombre,
                    Provincia = proveedor.ProvinciaCompraNet.Nombre,
                    RazonSocial = proveedor.RazonSocial,
                    Segmentacion = proveedor.Segmentacion.Descripcion,
                    TipoBoleto = proveedor.BoletoCompraNet.Descripcion,
                    Zona = proveedor.GrupoCompras

                };

            return resultado.Distinct().ToList();
        }

        public virtual List<ContactoAll> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, idComerciales, comercialOriginalId);
            }
        }
    }
}
