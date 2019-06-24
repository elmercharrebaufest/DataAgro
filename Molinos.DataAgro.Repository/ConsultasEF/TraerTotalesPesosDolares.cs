using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerTotalesPesosDolares : IConsultaEscalar<KendoGrid<TotalPesosDolares>>
    {
        private readonly KendoGridMvcRequest request;
        private readonly List<int> equipo;
        private readonly int perfilId;
        private readonly List<int> corredoresComercial;

        public TraerTotalesPesosDolares(KendoGridMvcRequest request, int perfilId, List<int> equipo, List<int> corredoresComercial)
        {
            this.request = request;
            this.equipo = equipo;
            this.perfilId = perfilId;
            this.corredoresComercial = corredoresComercial;
        }

        private static KendoGrid<TotalPesosDolares> Query(DbContext contexto, KendoGridMvcRequest request, int perfilId, List<int> equipo, List<int> corredoresComercial)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            var queryContratos =
                from contrato in contexto.Set<Contrato>()
                where perfilId != 8 ? equipo.Contains(contrato.ComercialId != null ? contrato.ComercialId.Value : 0) ||
                equipo.Contains(contrato.ComercialCreadorId != null ? contrato.ComercialCreadorId.Value : 0) :
                    (perfilId == (int)EnumPerfil.CorredoresComercial &&
                    (corredoresComercial.Contains(contrato.ComercialId != null ? contrato.ComercialId.Value : 0) ||
                    corredoresComercial.Contains(contrato.ComercialCreadorId != null ? contrato.ComercialCreadorId.Value : 0)))
                select new TotalPesosDolares()
                {
                    Cantidad = contrato.Cantidad,                    
                    FechaDesde = DbFunctions.TruncateTime(contrato.FechaDesde),
                    FechaHasta = DbFunctions.TruncateTime(contrato.FechaHasta),
                    Fecha = DbFunctions.TruncateTime(contrato.Fecha),
                    GrupoCompraDescripcion = contrato.GrupoDeCompras.Descripcion,
                    Estado_Contrato = contrato.Estado.Descripcion,
                    Ampliaciones = contrato.Ampliaciones,
                    Proveedor = contrato.Proveedor == null ? "" : contrato.Proveedor.RazonSocial,
                    Corredor = contrato.Corredor == null ? "" : contrato.Corredor.RazonSocial,
                    Comercial = contrato.Comercial == null ? "" : contrato.Comercial.Nombres + " " + contrato.Comercial.Apellido,
                    Material = contrato.Material == null ? "" : contrato.Material.Descripcion,
                    Campania = contrato.Campana == null ? "" : contrato.Campana.Descripcion,
                    TipoNegocio = contrato.TipoNegocio == null ? "" : contrato.Madre == true ? "CONVENIO" : contrato.Madre == false ? "FIJ. CONVENIO" : contrato.TipoNegocio.Descripcion,
                    Negocio = contrato.ContratoSAP != "0" ? contrato.ContratoSAP : "",
                    DestinoDescripcion = contrato.Destino.Descripcion,
                    ComercialCreador = contrato.ComercialCreador == null ? contrato.Comercial.Nombres + " " + contrato.Comercial.Apellido : contrato.ComercialCreador.Nombres + " " + contrato.ComercialCreador.Apellido,
                    TotalDolares = (contrato.MonedaId == "USDM " ? contrato.PrecioNeto != null ? contrato.PrecioNeto.Value * ((decimal)((int)(contrato.Cantidad * 100)) / 100) : contrato.Precio : 0) * ((decimal)((int)(contrato.Cantidad * 100)) / 100),
                    TotalPesos = (contrato.MonedaId == "ARP  " ? contrato.PrecioNeto != null ? contrato.PrecioNeto.Value * ((decimal)((int)(contrato.Cantidad * 100)) / 100) : contrato.Precio : 0) * ((decimal)((int)(contrato.Cantidad * 100)) / 100),
                };

            var queryFijacion =
                from fijac in contexto.Set<FijacionDePrecioContrato>()
                where perfilId != 8 ? equipo.Contains(fijac.ComercialId) || equipo.Contains(fijac.ComercialCreadorId) :
                        (perfilId == (int)EnumPerfil.CorredoresComercial && (corredoresComercial.Contains(fijac.ComercialId) ||
                        corredoresComercial.Contains(fijac.ComercialCreadorId)))
                select new TotalPesosDolares()
                {
                    Cantidad = fijac.Cantidad,
                    FechaDesde = null,
                    FechaHasta = null,
                    Fecha = DbFunctions.TruncateTime(fijac.Fecha),
                    GrupoCompraDescripcion = null,
                    Estado_Contrato = fijac.Estado.Descripcion,
                    Ampliaciones = fijac.Ampliaciones,
                    Proveedor = fijac.Proveedor == null ? "" : fijac.Proveedor.RazonSocial,
                    Corredor = fijac.Corredor == null ? "" : fijac.Corredor.RazonSocial,
                    Comercial = fijac.Comercial == null ? "" : fijac.Comercial.Nombres + " " + fijac.Comercial.Apellido,
                    Material = fijac.Material == null ? "" : fijac.Material.Descripcion,
                    Campania = "",
                    TipoNegocio = "FIJACION",
                    Negocio = fijac.EstadoId == (int)EnumEstadoContrato.Finalizado ? fijac.FijacionSAP : fijac.ContratoSAP,
                    DestinoDescripcion = fijac.Destino != null ? fijac.Destino.Descripcion : "",
                    ComercialCreador = fijac.ComercialCreador == null ? fijac.Comercial.Nombres + " " + fijac.Comercial.Apellido : fijac.ComercialCreador.Nombres + " " + fijac.ComercialCreador.Apellido,
                    TotalDolares = (fijac.MonedaId == "USDM " ? fijac.PrecioNeto != null ? fijac.PrecioNeto.Value * ((decimal)((int)(fijac.Cantidad * 100)) / 100) : fijac.Precio : 0) * ((decimal)((int)(fijac.Cantidad * 100)) / 100),
                    TotalPesos = (fijac.MonedaId == "ARP  " ? fijac.PrecioNeto != null ? fijac.PrecioNeto.Value * ((decimal)((int)(fijac.Cantidad * 100)) / 100) : fijac.Precio : 0) * ((decimal)((int)(fijac.Cantidad * 100)) / 100),
                };

            queryContratos = queryContratos.Union(queryFijacion);
            if (perfilId == 7)
            {
                var queryFason =
                    from fas in contexto.Set<Fason>()
                    where equipo.Contains(fas.ComercialId)
                    select new TotalPesosDolares()
                    {
                        Cantidad = fas.Cantidad,
                        FechaDesde = null,
                        FechaHasta = null,
                        Fecha = DbFunctions.TruncateTime(fas.Fecha),
                        GrupoCompraDescripcion = null,
                        Estado_Contrato = fas.Estado.Descripcion,
                        Ampliaciones = fas.Ampliaciones,
                        Proveedor = fas.Fasonero == null ? "" : fas.Fasonero.RazonSocial,
                        Corredor = "",
                        Comercial = fas.Comercial == null ? "" : fas.Comercial.Nombres + " " + fas.Comercial.Apellido,
                        Material = fas.Material == null ? "" : fas.Material.Descripcion,
                        Campania = fas.Campana == null ? "" : fas.Campana.Descripcion,
                        TipoNegocio = "FASON",
                        Negocio = null,
                        DestinoDescripcion = "",                        
                        ComercialCreador = fas.ComercialCreador == null ? fas.Comercial.Nombres + " " + fas.Comercial.Apellido : fas.ComercialCreador.Nombres + " " + fas.ComercialCreador.Apellido,                        
                        TotalDolares = (fas.MonedaId == "USDM " ? fas.Precio : 0) * ((decimal)((int)(fas.Cantidad * 100)) / 100),
                        TotalPesos = (fas.MonedaId == "ARP  " ? fas.Precio : 0) * ((decimal)((int)(fas.Cantidad * 100)) / 100),
                    };

                queryContratos = queryContratos.Union(queryFason);

                var queryAgente =
                    from age in contexto.Set<AgenteCompra>()
                    where equipo.Contains(age.ComercialId)
                    select new TotalPesosDolares()
                    {                       
                        Cantidad = age.Cantidad,
                        FechaDesde = null,
                        FechaHasta = null,
                        Fecha = DbFunctions.TruncateTime(age.Fecha),
                        GrupoCompraDescripcion = null,
                        Estado_Contrato = age.Estado.Descripcion,
                        Ampliaciones = age.Ampliaciones,
                        Proveedor = age.Operador.Descripcion,
                        Corredor = "",
                        Comercial = age.Comercial == null ? "" : age.Comercial.Nombres + " " + age.Comercial.Apellido,
                        Material = age.Material == null ? "" : age.Material.Descripcion,
                        Campania = "",
                        TipoNegocio = "AGENTE COMPRAS",
                        Negocio = null,
                        DestinoDescripcion = "",
                        ComercialCreador = age.ComercialCreador == null ? age.Comercial.Nombres + " " + age.Comercial.Apellido : age.ComercialCreador.Nombres + " " + age.ComercialCreador.Apellido,
                        TotalDolares = (age.MonedaId == "USDM " ? age.Precio : 0) * ((decimal)((int)(age.Cantidad * 100)) / 100),
                        TotalPesos = (age.MonedaId == "ARP  " ? age.Precio : 0) * ((decimal)((int)(age.Cantidad * 100)) / 100),
                    };

                queryContratos = queryContratos.Union(queryAgente);
                var queryAcuerdo =
                    from acu in contexto.Set<ContratoAcuerdo>()
                    where equipo.Contains(acu.ComercialCreadorId)
                    select new TotalPesosDolares()
                    {
                        
                        Cantidad = acu.Cantidad,
                        FechaDesde = DbFunctions.TruncateTime(acu.FechaDesde),
                        FechaHasta = DbFunctions.TruncateTime(acu.FechaHasta),
                        Fecha = DbFunctions.TruncateTime(acu.Fecha),
                        GrupoCompraDescripcion = null,
                        Estado_Contrato = acu.Estado.Descripcion,
                        Ampliaciones = null,
                        Proveedor = acu.Proveedor != null ? acu.Proveedor.RazonSocial : "",
                        Corredor = "",
                        Comercial = acu.Comercial == null ? "" : acu.Comercial.Nombres + " " + acu.Comercial.Apellido,
                        Material = acu.Material == null ? "" : acu.Material.Descripcion,
                        Campania = "",
                        TipoNegocio = "CONTRATO ACUERDO",
                        Negocio = null,
                        DestinoDescripcion = acu.Destino != null ? acu.Destino.Descripcion : "",
                        ComercialCreador = acu.Comercial == null ? acu.Comercial.Nombres + " " + acu.Comercial.Apellido : acu.Comercial.Nombres + " " + acu.Comercial.Apellido,
                        TotalDolares = 0,
                        TotalPesos = 0,
                    };

                queryContratos = queryContratos.Union(queryAcuerdo);
            }
            return new KendoGrid<TotalPesosDolares>(request, queryContratos);
        }

        public virtual KendoGrid<TotalPesosDolares> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, request, perfilId, equipo, corredoresComercial);
            }
        }
    }
}
