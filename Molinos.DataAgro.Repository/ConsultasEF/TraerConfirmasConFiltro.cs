using Kendo.DynamicLinq;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Entities.Seguridad;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Transactions;
using System.Web.Http.Results;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerConfirmasConFiltro : IConsultaEscalar<DataSourceResult>
    {
        private readonly DataSourceRequest request;
        private readonly List<int> equipo;

        public TraerConfirmasConFiltro(DataSourceRequest request, List<int> equipo)
        {
            this.request = request;
            this.equipo = equipo;
        }

        private static DataSourceResult Query(DbContext contexto, DataSourceRequest request, List<int> equipo)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            var crearFason = PermisosHelper.Is(PermisosDataAgro.CrearNegociosFason);
            var crearAgente = PermisosHelper.Is(PermisosDataAgro.CrearNegociosAgente);
            var crearAcuerdos = PermisosHelper.Is(PermisosDataAgro.CrearNegociosAcuerdos);

            try
            {
                var fijacionId = (int)EnumTipoNegocio.FIJACION;
                var confirmaId = (int)EnumBoletoCompraNet.CONFIRMA;
                var ne = contexto.Set<Negocio>();

                var queryBasicoConfirmas = from negocio in ne
                                           join confirma in (
                                               from c in contexto.Set<Confirma>()
                                               group c by c.NegocioId into g
                                               select g.OrderByDescending(c => c.Version).FirstOrDefault()
                                           ) on negocio.Id equals confirma.NegocioId into c
                                           from confirma in c.DefaultIfEmpty()
                                           join np in (
                                               from n in ne
                                               join parent in ne
                                                   on n.ContratoSAP equals parent.ContratoSAP
                                               where parent.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR
                                               && (
                                                   (n.ComercialId != null && equipo.Contains(n.ComercialId.Value)) ||
                                                   (n.ComercialCreadorId != null && equipo.Contains(n.ComercialCreadorId.Value))
                                               )
                                               select new
                                               {
                                                   n.ContratoSAP,
                                                   parent.BoletoId,
                                                   parent.BolsaId,
                                                   parent.Canje
                                               }
                                           ) on negocio.ContratoSAP equals np.ContratoSAP into npGroup
                                           from np in npGroup.DefaultIfEmpty()

                                           join proveedorCorredor in contexto.Set<Proveedor>()
                                               .Select(p => new { p.ProveedorId, p.RazonSocial })
                                               on negocio.CorredorId equals proveedorCorredor.ProveedorId into pCorredor
                                           from proveedorCorredor in pCorredor.DefaultIfEmpty()

                                           join proveedorVendedor in contexto.Set<Proveedor>()
                                               .Select(p => new { p.ProveedorId, p.RazonSocial })
                                               on negocio.ProveedorId equals proveedorVendedor.ProveedorId into pVendedor
                                           from proveedorVendedor in pVendedor.DefaultIfEmpty()

                                           where
                                           (
                                               (negocio.ComercialId != null && equipo.Contains(negocio.ComercialId.Value)) ||
                                               (negocio.ComercialCreadorId != null && equipo.Contains(negocio.ComercialCreadorId.Value))
                                           )
                                           &&
                                           (
                                               (crearFason && negocio is Fason) ||
                                               (crearAgente && negocio is AgenteCompra) ||
                                               (crearAcuerdos && negocio is ContratoAcuerdo) ||
                                               (!(negocio is Fason) && !(negocio is AgenteCompra) && !(negocio is ContratoAcuerdo))
                                           )
                                           && negocio.ConfirmadoSAP == true
                                           && negocio.EstadoId == (int)EnumEstadoContrato.Finalizado
                                           &&
                                           (
                                               (negocio.TipoNegocioId == fijacionId && np.BoletoId == confirmaId) ||
                                               (negocio.TipoNegocioId != fijacionId && negocio.BoletoId == confirmaId)
                                           )
                                           orderby negocio.Id descending
                                           select new BasicoConfirma
                                           {
                                               Id = negocio.Id,
                                               Estado_Version = confirma.FechaAnulacion != null ? "Anulado" : (confirma.FechaGeneracion != null ? "Vigente" : "Pendiente"),
                                               ClaseNegocioId = negocio is Contrato ? "1" : "2",
                                               NegocioSAP = negocio is FijacionDePrecioContrato &&
                                                            (negocio.EstadoId == (int)EnumEstadoContrato.Finalizado ||
                                                             negocio.EstadoId == (int)EnumEstadoContrato.Eliminado)
                                                            ? (negocio as FijacionDePrecioContrato).FijacionSAP
                                                            : negocio.ContratoSAP,
                                               ContratoSAP = negocio.ContratoSAP,
                                               TipoBoleto = negocio is FijacionDePrecioContrato ? (negocio as FijacionDePrecioContrato).Contrato.Boleto.Descripcion : negocio.Boleto.Descripcion,
                                               TipoNegocio = (negocio.TipoNegocio == null ? "" :
                                               (negocio is Contrato && (negocio as Contrato).Madre == true) ? "CONVENIO" :
                                               (negocio is Contrato && (negocio as Contrato).Madre == false) ? "FIJ. CONVENIO" :
                                               (negocio is Contrato && (negocio as Contrato).EsFason == true) ? "FASON MP" :
                                               (negocio is Contrato && (negocio as Contrato).TipoAgenteCompraId > 0) ? "AGENTE DE COMPRAS MP" :
                                               (negocio is ContratoAcuerdo && (negocio as ContratoAcuerdo).TipoAgenteCompraId > 0) ? "ACUERDO AGENTE" :
                                               (negocio is Contrato && (negocio as Contrato).Canje == true) ? "CANJE" :
                                               (negocio is Contrato && (negocio as Contrato).PrestamoDevolucion == true) ? "PRESTAMO DEVOLUCION" :
                                               (negocio is Contrato && (negocio as Contrato).Venta == true) ? "VENTA" :
                                               (negocio is Contrato && (negocio as Contrato).TipoPosicionCBOTId == 3) ? "A FIJAR PASE" :
                                               (negocio is FijacionDePrecioContrato && (negocio as FijacionDePrecioContrato).Virtual == true) ? "FIJACION VIRTUAL" :
                                               (negocio is FijacionDePrecioContrato && (negocio as FijacionDePrecioContrato).Canje == true) ? "FIJACION CANJE" :
                                               (negocio is FijacionDePrecioContrato && (negocio as FijacionDePrecioContrato).TipoPosicionCBOTId == 3) ? "FIJACION PASE" :
                                               negocio.TipoNegocio.Descripcion),
                                               Canje = negocio.TipoNegocioId == fijacionId ? (np.Canje == true ? "SI" : "NO") : (negocio.Canje == true ? "SI" : "NO"),
                                               Bolsa = negocio is FijacionDePrecioContrato ? (negocio as FijacionDePrecioContrato).Contrato.Bolsa.Descripcion : negocio.Bolsa.Descripcion,
                                               Precio = negocio.Precio,
                                               Moneda = negocio.MonedaId == "ARP" ? "ARP" : (negocio.MonedaId == "USDM" ? "USD" : string.Empty),
                                               FechaGeneracion = confirma.FechaGeneracion,
                                               FechaOperacion = negocio.FechaOperacion,
                                               FechaCarga = negocio.Fecha,
                                               FechaConfirmacion = negocio.FechaConfirmacion,
                                               FechaAnulacion = confirma.FechaAnulacion,
                                               Corredor = proveedorCorredor.RazonSocial ?? "",
                                               Vendedor = proveedorVendedor.RazonSocial ?? "",
                                               TipoNegocioId = negocio.TipoNegocioId,
                                               Material = negocio.Material.Descripcion,
                                               ContratoVendedor = negocio.ContratoVendedor,
                                               ContratoCorredor = negocio.ContratoCorredor,
                                               Comercial = negocio.Comercial.Apellido + ", " + negocio.Comercial.Nombres,
                                               Version = confirma.Version > 1 ? confirma.Version : 1,
                                               MaterialId = negocio.MaterialId,
                                               BoletoId = negocio is FijacionDePrecioContrato ? (negocio as FijacionDePrecioContrato).Contrato.BoletoId : negocio.BoletoId,

                                           };
                return queryBasicoConfirmas.ToDataSourceResult(request);               
                
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error en TraerConfirmasConFiltro: " + ex.Message);
                throw;
            }
        }


        public virtual DataSourceResult Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, request, equipo);
            }
        }
    }
}