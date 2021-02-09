using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Security.Claims;
using System.Threading;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public static class TraerTodosContratosSinFiltro
    {
        public static IQueryable<BasicoContrato> QueryBase(DbContext contexto, List<int> equipo)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            if (!PermisosHelper.Is(PermisosDataAgro.IngresoExterno))
            {
                var crearFason = PermisosHelper.Is(PermisosDataAgro.CrearNegociosFason);
                var crearAgente = PermisosHelper.Is(PermisosDataAgro.CrearNegociosAgente);
                var crearAcuerdos = PermisosHelper.Is(PermisosDataAgro.CrearNegociosAcuerdos);
                var queryNegocios =
                    from contrato in contexto.Set<Negocio>()
                    where ((contrato.ComercialId != null && equipo.Contains(contrato.ComercialId.Value)) || (
                    contrato.ComercialCreadorId != null && equipo.Contains(contrato.ComercialCreadorId.Value))) &&
                    ((crearFason && contrato is Fason) || (crearAgente && contrato is AgenteCompra) || (crearAcuerdos && contrato is ContratoAcuerdo) || (!(contrato is Fason) && !(contrato is AgenteCompra) && !(contrato is ContratoAcuerdo)))
                    select new BasicoContrato()
                    {
                        Id = contrato.Id,
                        ContratoId = contrato is Contrato ? contrato.Id : contrato is FijacionDePrecioContrato ? (contrato as FijacionDePrecioContrato).ContratoId.HasValue ? (contrato as FijacionDePrecioContrato).ContratoId.Value : 0 : 0,
                        ProveedorId = contrato.ProveedorId ?? 0,
                        CorredorId = contrato.CorredorId != null ? contrato.CorredorId.Value : 0,
                        ComercialId = contrato.ComercialId,
                        ComercialZonaId = contrato.Comercial.GrupoDeComprasId,
                        ComercialZonaDescripcion = contrato.Comercial.GrupoDeCompras.Descripcion,
                        MaterialId = contrato.MaterialId,
                        TipoNegocioId = contrato.TipoNegocioId,
                        Cantidad = contrato.Cantidad,
                        Precio = contrato.Precio,
                        PrecioPlazo = contrato.TipoNegocioId == 1 ? SqlFunctions.DateName("day", (contrato as Contrato).HastaFijacion) + "/" + SqlFunctions.DatePart("month", (contrato as Contrato).HastaFijacion) + "/" + SqlFunctions.DateName("year", (contrato as Contrato).HastaFijacion) : contrato.Precio.ToString(),
                        FechaEntrega = contrato is Contrato ? (contrato as Contrato).FechaEntrega : (DateTime?)null,
                        CampanaId = contrato.CampanaId ?? 0,
                        FechaDesde = DbFunctions.TruncateTime(contrato.FechaDesde),
                        FechaHasta = DbFunctions.TruncateTime(contrato.FechaHasta),
                        MonedaId = contrato.MonedaId,
                        Moneda = contrato.Moneda == null ? "" : contrato.Moneda.Descripcion,
                        Fecha = DbFunctions.TruncateTime(contrato.Fecha),
                        Hora = SqlFunctions.DateName("hh", contrato.Fecha) + ":" + SqlFunctions.DateName("mi", contrato.Fecha),
                        Fecha_Order = contrato.Fecha,
                        GrupoCompra = (contrato is FijacionDePrecioContrato && (contrato as FijacionDePrecioContrato).ComercialId.HasValue) ? (contrato as FijacionDePrecioContrato).Comercial.GrupoDeComprasId.Value :
                                        contrato.GrupoCompra.HasValue ? contrato.GrupoCompra.Value : 0,
                        GrupoCompraDescripcion = (contrato is FijacionDePrecioContrato && (contrato as FijacionDePrecioContrato).ComercialId.HasValue) ? (contrato as FijacionDePrecioContrato).Comercial.GrupoDeCompras.Descripcion :
                                                    contrato.GrupoDeCompras.Descripcion,
                        ProvinciaId = contrato is Contrato ? (contrato as Contrato).ProvinciaId : null,
                        LocalidadId = contrato is Contrato ? (contrato as Contrato).LocalidadId : null,
                        Base = contrato is Contrato ? (contrato as Contrato).Base : null,
                        Importe_Sustentable = contrato is Contrato ? ((decimal)(contrato as Contrato).ImporteSustentable) : (decimal?)null,
                        MonedaId_Sustentable = contrato is Contrato ? (contrato as Contrato).MonedaSustentableId : "",
                        Moneda_Sustentable = !(contrato is Contrato) || (contrato as Contrato).MonedaSustentable == null ? "" : (contrato as Contrato).MonedaSustentable.Descripcion,
                        Fecha_Dolarizado = contrato is Contrato ? DbFunctions.TruncateTime((contrato as Contrato).FechaDolarizado) : contrato is FijacionDePrecioContrato ? DbFunctions.TruncateTime((contrato as FijacionDePrecioContrato).FechaDolarizado) : (DateTime?)null,
                        Dias_Pesificado = contrato.DiasPesificado,
                        NoInformaSIO = contrato is Contrato ? (contrato as Contrato).NoInformaSio : (bool?)null,
                        Estado = contrato.EstadoId,
                        Estado_Contrato = contrato.Estado.Descripcion,
                        Estado_Order = contrato.Estado.Orden,
                        UsuarioId = contrato.UsuarioId,
                        ContratoSAP = contrato.ContratoSAP,
                        Ampliaciones = contrato.Ampliaciones,
                        Cuit = contrato.Proveedor == null ? "" : contrato.Proveedor.CUIT,
                        Proveedor = (contrato is AgenteCompra) ? (contrato as AgenteCompra).Operador.Descripcion : contrato.Proveedor == null ? "" : contrato.Proveedor.RazonSocial,
                        Corredor = contrato.Corredor == null ? "" : contrato.Corredor.RazonSocial,
                        CUITCorredor = contrato.Corredor == null ? "" : contrato.Corredor.CUIT,
                        Comercial = contrato.Comercial == null ? "" : contrato.Comercial.Nombres + " " + contrato.Comercial.Apellido,
                        Material = contrato.Material == null ? "" : contrato.Material.Descripcion,
                        Campania = contrato.Campana == null ? "" : contrato.Campana.Descripcion,
                        Provincia = !(contrato is Contrato) || (contrato as Contrato).Provincia == null ? "" : (contrato as Contrato).Provincia.Nombre,
                        TipoNegocio = (contrato.TipoNegocio == null ? "" : (contrato is Contrato && (contrato as Contrato).Madre == true) ? "CONVENIO" : (contrato is Contrato && (contrato as Contrato).Madre == false) ? "FIJ. CONVENIO" : 
                        (contrato is Contrato && (contrato as Contrato).EsFason == true) ? "FASON MP" : 
                        (contrato is Contrato && (contrato as Contrato).TipoAgenteCompraId > 0) ? "AGENTE DE COMPRAS MP" : 
                        (contrato is ContratoAcuerdo && (contrato as ContratoAcuerdo).TipoAgenteCompraId > 0) ? "ACUERDO AGENTE" :
                        (contrato is Contrato && (contrato as Contrato).Canje == true) ? "CANJE" : (contrato is Contrato && (contrato as Contrato).PrestamoDevolucion == true) ? "PRÉSTAMO DEVOLUCIÓN" :
                        (contrato is Contrato && (contrato as Contrato).Venta == true) ? "VENTA" : contrato.TipoNegocio.Descripcion),
                        Localidad = !(contrato is Contrato) || (contrato as Contrato).Localidad == null ? "" : (contrato as Contrato).Localidad.Nombre,
                        Observacion = contrato.Observacion != null ? contrato.Observacion : "",
                        FijacionDePrecioContratoId = (contrato is FijacionDePrecioContrato) ? (int?)(contrato as FijacionDePrecioContrato).Id : null,
                        Sustentable = (contrato is Contrato) && (contrato as Contrato).ImporteSustentable != null && (contrato as Contrato).ImporteSustentable > 0,
                        Dolarizado = contrato.Dolarizado.Value,
                        Pesificado = contrato.DiasPesificado != null,
                        Negocio = contrato is ContratoAcuerdo ? contrato.Id.ToString() : (contrato is FijacionDePrecioContrato && (contrato.EstadoId == (int)EnumEstadoContrato.Finalizado || contrato.EstadoId == (int)EnumEstadoContrato.Eliminado)) ? (contrato as FijacionDePrecioContrato).FijacionSAP : contrato.ContratoSAP != "0" ? contrato.ContratoSAP : "",
                        DestinoId = contrato.DestinoId,
                        DestinoDescripcion = contrato.Destino.Descripcion,
                        CantidadCamiones = (contrato is Contrato) ? (contrato as Contrato).CantidadCamiones : (int?)null,
                        Consignatario = (contrato is Contrato) ? (contrato as Contrato).Consignatario : false,
                        PlanCanje = (contrato is Contrato) ? (contrato as Contrato).PlanCanje : false,
                        CD = (contrato is Contrato) ? (contrato as Contrato).CD : null,
                        Warrant = (contrato is Contrato) ? (contrato as Contrato).Warrant : null,
                        PagoDirectoVendedor = (contrato is Contrato) ? (contrato as Contrato).PagoDirectoVendedor : null,
                        EstablecimientoPropio = (contrato is Contrato) ? (contrato as Contrato).EstablecimientoPropio : null,
                        BoletoId = (contrato is Contrato) ? (contrato as Contrato).BoletoId : null,
                        BolsaId = (contrato is Contrato) ? (contrato as Contrato).BolsaId : null,
                        BoletoDescripcion = (contrato is Contrato) ? (contrato as Contrato).Boleto.Descripcion : "",
                        BolsaDescripcion = (contrato is Contrato) ? (contrato as Contrato).Bolsa.Descripcion : "",
                        DesdeFijacion = (contrato is Contrato) ? DbFunctions.TruncateTime((contrato as Contrato).DesdeFijacion) : null,
                        HastaFijacion = (contrato is Contrato) ? DbFunctions.TruncateTime((contrato as Contrato).HastaFijacion) : null,
                        CondicionFijacion = (contrato is Contrato) ? (contrato as Contrato).CondicionFijacionId : null,
                        CondicionFijacionDescripcion = (contrato is Contrato) ? (contrato as Contrato).CondicionFijacion.Descripcion : "",
                        ClasificacionId = (contrato is Contrato) ? (contrato as Contrato).ClasificacionId : (int?)null,
                        ClasificacionDescripcion = (contrato is Contrato) ? (contrato as Contrato).Clasificacion.Descripcion : "",
                        CalidadDescripcion = contrato.TrigoEspecial == true ? "Especial" : "Cámara",
                        MercsDeposito = (contrato is Contrato) ? ((contrato as Contrato).MercsDeposito == true ? (contrato as Contrato).MercsDeposito : false) : null,
                        ComercialCreadorId = contrato.ComercialCreadorId ?? contrato.ProveedorCreadorId,
                        ComercialCreador = contrato.ProveedorCreadorId != null ? contrato.ProveedorCreador.RazonSocial : contrato.ComercialCreador == null ? contrato.Comercial.Nombres + " " + contrato.Comercial.Apellido : contrato.ComercialCreador.Nombres + " " + contrato.ComercialCreador.Apellido,
                        ContratoCorredor = (contrato is Contrato) ? (contrato as Contrato).ContratoCorredor : "",
                        ContratoVendedor = (contrato is Contrato) ? (contrato as Contrato).ContratoVendedor : "",
                        SelCargoMOA = (contrato is Contrato) ? (contrato as Contrato).SelCargoMOA : null,
                        SelCargoVendedor = (contrato is Contrato) ? (contrato as Contrato).SelCargoVendedor : null,
                        Posicion = (contrato is Fason) ? (contrato as Fason).Posicion : (contrato is AgenteCompra) ? (contrato as AgenteCompra).Posicion : "",
                        TipoFason = (contrato is Fason) ? (contrato as Fason).TipoFason.Descripcion : "",
                        FasonId = (contrato is Fason) ? (contrato as Fason).Id : 0,
                        Operador = (contrato is AgenteCompra) ? (contrato as AgenteCompra).Operador.Descripcion : "",
                        OperadorId = (contrato is AgenteCompra) ? (contrato as AgenteCompra).Operador.Id : 0,
                        AgenteId = (contrato is AgenteCompra) ? (contrato as AgenteCompra).Id : 0,
                        PrecioNeto = contrato.PrecioNeto,
                        StandardCalidadId = contrato.StandardDeCalidadId,
                        StandardDeCalidadDescripcion = contrato.StandardDeCalidad.Descripcion,
                        Pizarra = contrato.Pizarra ?? null,
                        PagoDiferido = contrato.PagoDiferido ?? null,
                        ZonaId = (contrato is Contrato) ? (contrato as Contrato).ZonaId : null,
                        ZonaDescripcion = (contrato is Contrato) && (contrato as Contrato).Zona != null ? (contrato as Contrato).Zona.Descripcion : "",
                        AcuerdoId = (contrato is ContratoAcuerdo) ? (int?)(contrato as ContratoAcuerdo).Id : null,
                        ImporteFinanciero = contrato.AperturaPrecio.FirstOrDefault(t => t.ConceptoAperturaPrecioId == 1).Importe,
                        ImporteRedespacho = contrato.AperturaPrecio.FirstOrDefault(t => t.ConceptoAperturaPrecioId == 2).Importe,
                        PorcentajeComision = contrato.AperturaPrecio.FirstOrDefault(t => t.ConceptoAperturaPrecioId == 3).Porcentaje,
                        ImporteComision = contrato.AperturaPrecio.FirstOrDefault(t => t.ConceptoAperturaPrecioId == 3).Importe,
                        ImporteBonificacion = contrato.AperturaPrecio.FirstOrDefault(t => t.ConceptoAperturaPrecioId == 4).Importe,
                        PorcentajeBonificacion = contrato.AperturaPrecio.FirstOrDefault(t => t.ConceptoAperturaPrecioId == 4).Porcentaje,
                        MonedaBonificacion = contrato.AperturaPrecio.FirstOrDefault(t => t.ConceptoAperturaPrecioId == 4).Moneda.Descripcion,
                        NivelTarifa = (contrato is Contrato) ? (contrato as Contrato).NivelTarifa.Descripcion : "",
                        TarifaFlete = (contrato is Contrato) ? (contrato as Contrato).TarifaFlete : null,
                        Compensacion = (contrato is Contrato) ? (contrato as Contrato).Compensacion : null,
                        Acuerdo = (contrato is Contrato) ? (contrato as Contrato).ContratoAcuerdoId : null,
                        Rechazo = contrato.MotivoRechazo,
                        OcultarEnTablero = contrato.OcultarEnTablero,
                        FechaCierta = DbFunctions.TruncateTime(contrato.FechaCierta) ?? null,
                        EsFason = contrato is Contrato ? (contrato as Contrato).EsFason : false,
                        PorcentajeDePago = contrato is Contrato ? (contrato as Contrato).PorcentajeDePago : null,
                        FechaOperacion = DbFunctions.TruncateTime((contrato as Negocio).FechaOperacion),
                        MotivoOperacionAnterior = contrato.MotivoOperacionAnterior,
                        UsuarioConfirmador = contrato.EstadoId == 1 ? "" : contrato.ComercialConfirmador != null ? contrato.ComercialConfirmador.Nombres + " " + contrato.ComercialConfirmador.Apellido : "Automática",
                        FechaConfirmacion = contrato.FechaConfirmacion != null ? contrato.FechaConfirmacion : (DateTime?)null,
                        ChequeElectronicoValor = contrato.ChequeElectronico.HasValue ? (contrato.ChequeElectronico.Value ? "Si" : "No") : "",
                        DolarizadoExpress = contrato.DolarizadoExpress.Value,
                        DolarizadoCorredor = contrato.DolarizadoCorredor.Value,
                        PagoCBU = contrato.PagoCBU,
                        CalidadTercero = contrato.CalidadTercero,
                        DolarizadoTercero = contrato.DolarizadoTercero,
                        PagoDiferidoTercero = contrato.PagoDiferidoTercero,
                        ObservacionTercero = contrato.ObservacionTercero,
                        Canje = contrato.Canje,
                        MonedaCanjeId = contrato.MonedaCanjeId,
                        Monto = contrato.Monto,
                        Insumo = contrato.Insumo,
                        PrestamoDevolucion = contrato.PrestamoDevolucion.HasValue ? contrato.PrestamoDevolucion.Value : false,
                        PlantaDestinoId = contrato.PlantaDestinoId.HasValue ? contrato.PlantaDestinoId.Value : 0,
                        PlantaDestinoDescripcion = contrato.PlantaDestino != null ? contrato.PlantaDestino.Descripcion : "",
                        SustentableTercero = contrato.SustentableTercero,
                        Venta = contrato.Venta,

                    };

                return queryNegocios;
            }
            else
            {
                var cuit = PermisosHelper.ObtenerCuit();
                long lcuit = 0;
                if (!long.TryParse(cuit, out lcuit))
                {
                    cuit = "";
                }
                var queryNegocios =
                    from contrato in contexto.Set<Negocio>()
                    where contrato.ProveedorCreador.CUIT == cuit || cuit == ""
                    select new BasicoContrato()
                    {
                        Id = contrato.Id,
                        ContratoId = contrato is Contrato ? contrato.Id : contrato is FijacionDePrecioContrato ? (contrato as FijacionDePrecioContrato).ContratoId.HasValue ? (contrato as FijacionDePrecioContrato).ContratoId.Value : 0 : 0,
                        ProveedorId = contrato.ProveedorId ?? 0,
                        CorredorId = contrato.CorredorId != null ? contrato.CorredorId.Value : 0,
                        ComercialId = contrato.ComercialId,
                        ComercialZonaId = contrato.Comercial.GrupoDeComprasId,
                        ComercialZonaDescripcion = contrato.Comercial.GrupoDeCompras.Descripcion,
                        MaterialId = contrato.MaterialId,
                        TipoNegocioId = contrato.TipoNegocioId,
                        Cantidad = contrato.Cantidad,
                        Precio = contrato.Precio,
                        PrecioPlazo = contrato.TipoNegocioId == 1 ? SqlFunctions.DateName("day", (contrato as Contrato).HastaFijacion) + "/" + SqlFunctions.DatePart("month", (contrato as Contrato).HastaFijacion) + "/" + SqlFunctions.DateName("year", (contrato as Contrato).HastaFijacion) : contrato.Precio.ToString(),
                        FechaEntrega = contrato is Contrato ? (contrato as Contrato).FechaEntrega : (DateTime?)null,
                        CampanaId = contrato.CampanaId ?? 0,
                        FechaDesde = DbFunctions.TruncateTime(contrato.FechaDesde),
                        FechaHasta = DbFunctions.TruncateTime(contrato.FechaHasta),
                        MonedaId = contrato.MonedaId,
                        Moneda = contrato.Moneda == null ? "" : contrato.Moneda.Descripcion,
                        Fecha = DbFunctions.TruncateTime(contrato.Fecha),
                        Hora = SqlFunctions.DateName("hh", contrato.Fecha) + ":" + SqlFunctions.DateName("mi", contrato.Fecha),
                        Fecha_Order = contrato.Fecha,
                        GrupoCompra = (contrato is FijacionDePrecioContrato && (contrato as FijacionDePrecioContrato).ComercialId.HasValue) ? (contrato as FijacionDePrecioContrato).Comercial.GrupoDeComprasId.Value :
                                        contrato.GrupoCompra.HasValue ? contrato.GrupoCompra.Value : 0,
                        GrupoCompraDescripcion = (contrato is FijacionDePrecioContrato && (contrato as FijacionDePrecioContrato).ComercialId.HasValue) ? (contrato as FijacionDePrecioContrato).Comercial.GrupoDeCompras.Descripcion :
                                                    contrato.GrupoDeCompras.Descripcion,
                        ProvinciaId = contrato is Contrato ? (contrato as Contrato).ProvinciaId : null,
                        LocalidadId = contrato is Contrato ? (contrato as Contrato).LocalidadId : null,
                        Base = contrato is Contrato ? (contrato as Contrato).Base : null,
                        Importe_Sustentable = contrato is Contrato ? ((decimal)(contrato as Contrato).ImporteSustentable) : (decimal?)null,
                        MonedaId_Sustentable = contrato is Contrato ? (contrato as Contrato).MonedaSustentableId : "",
                        Moneda_Sustentable = !(contrato is Contrato) || (contrato as Contrato).MonedaSustentable == null ? "" : (contrato as Contrato).MonedaSustentable.Descripcion,
                        Fecha_Dolarizado = contrato is Contrato ? DbFunctions.TruncateTime((contrato as Contrato).FechaDolarizado) : contrato is FijacionDePrecioContrato ? DbFunctions.TruncateTime((contrato as FijacionDePrecioContrato).FechaDolarizado) : (DateTime?)null,
                        Dias_Pesificado = contrato.DiasPesificado,
                        NoInformaSIO = contrato is Contrato ? (contrato as Contrato).NoInformaSio : (bool?)null,
                        Estado = contrato.EstadoId == 9 ? 9 :  contrato.EstadoId == 1 || contrato.EstadoId == 7 ? 1 : contrato.EstadoId == 6 || contrato.EstadoId == 8 ? 6 : contrato.EstadoId == 5 ? 5 : 2,
                        Estado_Contrato = contrato.EstadoId == 9 || contrato.EstadoId == 1 || contrato.EstadoId == 7 ? "Carga" : contrato.EstadoId == 6 || contrato.EstadoId == 8 ? "Rechazado" : contrato.EstadoId == 5 ? "Finalizado" : "Confirmado",
                        Estado_Order = contrato.Estado.Orden,
                        UsuarioId = contrato.UsuarioId,
                        ContratoSAP = contrato.ContratoSAP,
                        Ampliaciones = contrato.Ampliaciones,
                        Cuit = contrato.Proveedor == null ? "" : contrato.Proveedor.CUIT,
                        Proveedor = (contrato is AgenteCompra) ? (contrato as AgenteCompra).Operador.Descripcion : contrato.Proveedor == null ? "" : contrato.Proveedor.RazonSocial,
                        Corredor = contrato.Corredor == null ? "" : contrato.Corredor.RazonSocial,
                        CUITCorredor = contrato.Corredor == null ? "" : contrato.Corredor.CUIT,
                        Comercial = contrato.Comercial == null ? "" : contrato.Comercial.Nombres + " " + contrato.Comercial.Apellido,
                        Material = contrato.Material == null ? "" : contrato.Material.Descripcion,
                        Campania = contrato.Campana == null ? "" : contrato.Campana.Descripcion,
                        Provincia = !(contrato is Contrato) || (contrato as Contrato).Provincia == null ? "" : (contrato as Contrato).Provincia.Nombre,
                        TipoNegocio = (contrato.TipoNegocio == null ? "" : (contrato is Contrato && (contrato as Contrato).Madre == true) ? "CONVENIO" : (contrato is Contrato && (contrato as Contrato).Madre == false) ? "FIJ. CONVENIO" : (contrato is Contrato && (contrato as Contrato).EsFason == true) ? "FASON MP" : (contrato is Contrato && (contrato as Contrato).TipoAgenteCompraId > 0) ? "AGENTE DE COMPRAS MP" : (contrato is ContratoAcuerdo && (contrato as ContratoAcuerdo).TipoAgenteCompraId > 0) ? "ACUERDO AGENTE" : contrato.TipoNegocio.Descripcion),
                        Localidad = !(contrato is Contrato) || (contrato as Contrato).Localidad == null ? "" : (contrato as Contrato).Localidad.Nombre,
                        Observacion = contrato.Observacion != null ? contrato.Observacion : "",
                        FijacionDePrecioContratoId = (contrato is FijacionDePrecioContrato) ? (int?)(contrato as FijacionDePrecioContrato).Id : null,
                        Sustentable = (contrato is Contrato) && (contrato as Contrato).ImporteSustentable != null && (contrato as Contrato).ImporteSustentable > 0,
                        Dolarizado = contrato.Dolarizado.Value,
                        Pesificado = contrato.DiasPesificado != null,
                        Negocio = contrato is ContratoAcuerdo ? contrato.Id.ToString() : (contrato is FijacionDePrecioContrato && (contrato.EstadoId == (int)EnumEstadoContrato.Finalizado || contrato.EstadoId == (int)EnumEstadoContrato.Eliminado)) ? (contrato as FijacionDePrecioContrato).FijacionSAP : contrato.ContratoSAP != "0" ? contrato.ContratoSAP : "",
                        DestinoId = contrato.DestinoId,
                        DestinoDescripcion = contrato.Destino.Descripcion,
                        CantidadCamiones = (contrato is Contrato) ? (contrato as Contrato).CantidadCamiones : (int?)null,
                        Consignatario = (contrato is Contrato) ? (contrato as Contrato).Consignatario : false,
                        PlanCanje = (contrato is Contrato) ? (contrato as Contrato).PlanCanje : false,
                        CD = (contrato is Contrato) ? (contrato as Contrato).CD : null,
                        Warrant = (contrato is Contrato) ? (contrato as Contrato).Warrant : null,
                        PagoDirectoVendedor = (contrato is Contrato) ? (contrato as Contrato).PagoDirectoVendedor : null,
                        EstablecimientoPropio = (contrato is Contrato) ? (contrato as Contrato).EstablecimientoPropio : null,
                        BoletoId = (contrato is Contrato) ? (contrato as Contrato).BoletoId : null,
                        BolsaId = (contrato is Contrato) ? (contrato as Contrato).BolsaId : null,
                        BoletoDescripcion = (contrato is Contrato) ? (contrato as Contrato).Boleto.Descripcion : "",
                        BolsaDescripcion = (contrato is Contrato) ? (contrato as Contrato).Bolsa.Descripcion : "",
                        DesdeFijacion = (contrato is Contrato) ? DbFunctions.TruncateTime((contrato as Contrato).DesdeFijacion) : null,
                        HastaFijacion = (contrato is Contrato) ? DbFunctions.TruncateTime((contrato as Contrato).HastaFijacion) : null,
                        CondicionFijacion = (contrato is Contrato) ? (contrato as Contrato).CondicionFijacionId : null,
                        CondicionFijacionDescripcion = (contrato is Contrato) ? (contrato as Contrato).CondicionFijacion.Descripcion : "",
                        ClasificacionId = (contrato is Contrato) ? (contrato as Contrato).ClasificacionId : (int?)null,
                        ClasificacionDescripcion = (contrato is Contrato) ? (contrato as Contrato).Clasificacion.Descripcion : "",
                        CalidadDescripcion = contrato.TrigoEspecial == true ? "Especial" : "Cámara",
                        MercsDeposito = (contrato is Contrato) ? ((contrato as Contrato).MercsDeposito == true ? (contrato as Contrato).MercsDeposito : false) : null,
                        ComercialCreadorId = contrato.ComercialCreadorId ?? contrato.ProveedorCreadorId,
                        ComercialCreador = contrato.ProveedorCreadorId != null ? contrato.ProveedorCreador.RazonSocial : contrato.ComercialCreador == null ? contrato.Comercial.Nombres + " " + contrato.Comercial.Apellido : contrato.ComercialCreador.Nombres + " " + contrato.ComercialCreador.Apellido,
                        ContratoCorredor = (contrato is Contrato) ? (contrato as Contrato).ContratoCorredor : "",
                        ContratoVendedor = (contrato is Contrato) ? (contrato as Contrato).ContratoVendedor : "",
                        SelCargoMOA = (contrato is Contrato) ? (contrato as Contrato).SelCargoMOA : null,
                        SelCargoVendedor = (contrato is Contrato) ? (contrato as Contrato).SelCargoVendedor : null,
                        Posicion = (contrato is Fason) ? (contrato as Fason).Posicion : (contrato is AgenteCompra) ? (contrato as AgenteCompra).Posicion : "",
                        TipoFason = (contrato is Fason) ? (contrato as Fason).TipoFason.Descripcion : "",
                        FasonId = (contrato is Fason) ? (contrato as Fason).Id : 0,
                        Operador = (contrato is AgenteCompra) ? (contrato as AgenteCompra).Operador.Descripcion : "",
                        OperadorId = (contrato is AgenteCompra) ? (contrato as AgenteCompra).Operador.Id : 0,
                        AgenteId = (contrato is AgenteCompra) ? (contrato as AgenteCompra).Id : 0,
                        PrecioNeto = contrato.PrecioNeto,
                        StandardCalidadId = contrato.StandardDeCalidadId,
                        StandardDeCalidadDescripcion = contrato.StandardDeCalidad.Descripcion,
                        Pizarra = contrato.Pizarra ?? null,
                        PagoDiferido = contrato.PagoDiferido ?? null,
                        ZonaId = (contrato is Contrato) ? (contrato as Contrato).ZonaId : null,
                        ZonaDescripcion = (contrato is Contrato) && (contrato as Contrato).Zona != null ? (contrato as Contrato).Zona.Descripcion : "",
                        AcuerdoId = (contrato is ContratoAcuerdo) ? (int?)(contrato as ContratoAcuerdo).Id : null,
                        ImporteFinanciero = contrato.AperturaPrecio.FirstOrDefault(t => t.ConceptoAperturaPrecioId == 1).Importe,
                        ImporteRedespacho = contrato.AperturaPrecio.FirstOrDefault(t => t.ConceptoAperturaPrecioId == 2).Importe,
                        PorcentajeComision = contrato.AperturaPrecio.FirstOrDefault(t => t.ConceptoAperturaPrecioId == 3).Porcentaje,
                        ImporteComision = contrato.AperturaPrecio.FirstOrDefault(t => t.ConceptoAperturaPrecioId == 3).Importe,
                        ImporteBonificacion = contrato.AperturaPrecio.FirstOrDefault(t => t.ConceptoAperturaPrecioId == 4).Importe,
                        PorcentajeBonificacion = contrato.AperturaPrecio.FirstOrDefault(t => t.ConceptoAperturaPrecioId == 4).Porcentaje,
                        MonedaBonificacion = contrato.AperturaPrecio.FirstOrDefault(t => t.ConceptoAperturaPrecioId == 4).Moneda.Descripcion,
                        NivelTarifa = (contrato is Contrato) ? (contrato as Contrato).NivelTarifa.Descripcion : "",
                        TarifaFlete = (contrato is Contrato) ? (contrato as Contrato).TarifaFlete : null,
                        Compensacion = (contrato is Contrato) ? (contrato as Contrato).Compensacion : null,
                        Acuerdo = (contrato is Contrato) ? (contrato as Contrato).ContratoAcuerdoId : null,
                        Rechazo = contrato.MotivoRechazo,
                        OcultarEnTablero = contrato.OcultarEnTablero,
                        FechaCierta = DbFunctions.TruncateTime(contrato.FechaCierta) ?? null,
                        EsFason = contrato is Contrato ? (contrato as Contrato).EsFason : false,
                        PorcentajeDePago = contrato is Contrato ? (contrato as Contrato).PorcentajeDePago : null,
                        FechaOperacion = DbFunctions.TruncateTime((contrato as Negocio).FechaOperacion),
                        MotivoOperacionAnterior = contrato.MotivoOperacionAnterior,
                        UsuarioConfirmador = contrato.EstadoId == 1 ? "" : contrato.ComercialConfirmador != null ? contrato.ComercialConfirmador.Nombres + " " + contrato.ComercialConfirmador.Apellido : "Automática",
                        FechaConfirmacion = contrato.FechaConfirmacion != null ? contrato.FechaConfirmacion : (DateTime?)null,
                        ChequeElectronicoValor = contrato.ChequeElectronico.HasValue ? (contrato.ChequeElectronico.Value ? "Si" : "No") : "",
                        DolarizadoExpress = contrato.DolarizadoExpress.Value,
                        PagoCBU = contrato.PagoCBU,
                        CalidadTercero = contrato.CalidadTercero,
                        DolarizadoTercero = contrato.DolarizadoTercero,
                        PagoDiferidoTercero = contrato.PagoDiferidoTercero,
                        ObservacionTercero = contrato.ObservacionTercero,
                        DolarizadoCorredor = contrato.DolarizadoCorredor.Value,
                        SustentableTercero = contrato.SustentableTercero,

                    };

                return queryNegocios;
            }      
            
        }
    }
}
