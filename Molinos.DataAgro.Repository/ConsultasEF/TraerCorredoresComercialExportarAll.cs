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
                //from proveedorComercial in contexto.Set<ProveedorComercial>() 
                //join proveedor in contexto.Set<Proveedor>() on proveedorComercial.ProveedorId equals proveedor.ProveedorId
                //join ContactoComercial in contexto.Set<ContactoComercial>() on proveedor.ProveedorId equals ContactoComercial.ProveedorId into contactos
                //from con in contactos.DefaultIfEmpty()
                //join comercial in contexto.Set<Comercial>() on proveedorComercial.ComercialId equals comercial.ComercialId
                //join Estado in contexto.Set<Estado>() on proveedor.EstadoId equals Estado.EstadoId into estados
                //from ests in estados.DefaultIfEmpty()
                //where idComerciales.Contains(proveedorComercial.ComercialId)
                from pCom in contexto.Set<ProveedorComercial>()
                join prove in contexto.Set<Proveedor>() on pCom.ProveedorId equals prove.ProveedorId
                join comercial in contexto.Set<Comercial>() on pCom.ComercialId equals comercial.ComercialId
                join est in contexto.Set<Estado>() on prove.EstadoId equals est.EstadoId into estados
                from ests in estados.DefaultIfEmpty()
                where idComerciales.Contains(pCom.ComercialId)
                group prove by prove into provs
                select new ContactoAll
                {
                    AreaDeInfluencia = provs.Key.AreaInfluencia.Descripcion,
                    Bolsa = provs.Key.BoletoCompraNet.Descripcion,
                    Calificacion = provs.Key.Calificacion,
                    CanalDeOperacion = "",
                    Clasificacion = provs.Key.ClasificacionCompraNet.Descripcion,
                    ClienteMoa = (provs.Key.ClienteMOA.HasValue && provs.Key.ClienteMOA.Value) ? "SI" : "NO",
                    CodPostal = provs.Key.CodigoPostal,
                    Comentario = "",
                    Comercial = contexto.Set<ProveedorComercial>().Where(x => x.ProveedorId == provs.Key.ProveedorId).FirstOrDefault().Comercial.Apellido,
                    Condicion = "",
                    Consignatario = provs.Key.Consignatario.HasValue && provs.Key.Consignatario.Value ? "SI" : "NO",
                    Cuit = provs.Key.CUIT,
                    Destinatario = "",
                    Domicilio = provs.Key.Direccion,
                    Estado = provs.Key.Estado.Descripcion,
                    FechaAlta = provs.Key.FechaAlta.HasValue ? provs.Key.FechaAlta.Value : DateTime.Now,
                    Intermediario = provs.Key.Intermediario,
                    Localidad = provs.Key.Localidad.Nombre,
                    Provincia = provs.Key.Provincia.Nombre,
                    RazonSocial = provs.Key.RazonSocial,
                    Segmentacion = provs.Key.Segmentacion.Descripcion,
                    TipoBoleto = provs.Key.BoletoCompraNet.Descripcion,
                    Zona = provs.Key.GrupoCompras,
                    ComisionPorcentaje = provs.Key.ComisionPorcentaje??0,
                    LocalidadCompraNet = provs.Key.LocalidadCompraNet.Nombre,
                    ProvinciaCompraNet = provs.Key.ProvinciaCompraNet.Nombre,
                    Deshabilitado = provs.Key.Deshabilitado.HasValue && provs.Key.Deshabilitado.Value ? "SI" : "NO",

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
