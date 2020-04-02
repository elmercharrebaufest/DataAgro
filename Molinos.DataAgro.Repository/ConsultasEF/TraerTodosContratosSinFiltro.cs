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
                        GrupoCompraDescripcion =(contrato is FijacionDePrecioContrato && (contrato as FijacionDePrecioContrato).ComercialId.HasValue) ? (contrato as FijacionDePrecioContrato).Comercial.GrupoDeCompras.Descripcion :
                                                    contrato.GrupoDeCompras.Descripcion,
                        ProvinciaId = contrato is Contrato ? (contrato as Contrato).ProvinciaId : null,
                        LocalidadId = contrato is Contrato ? (contrato as Contrato).LocalidadId : null,
                        Base = contrato is Contrato ? (contrato as Contrato).Base : null,
                        Importe_Sustentable = contrato is Contrato ? ((decimal)(contrato as Contrato).ImporteSustentable) : (decimal?)null,
                        MonedaId_Sustentable = contrato is Contrato ? (contrato as Contrato).MonedaSustentableId : "",
                        Moneda_Sustentable = !(contrato is Contrato) || (contrato as Contrato).MonedaSustentable == null ? "" : (contrato as Contrato).MonedaSustentable.Descripcion,
                        Fecha_Dolarizado = DbFunctions.TruncateTime((contrato as Contrato).FechaDolarizado),
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
                        Comercial = contrato.Comercial == null ? "" : contrato.Comercial.Nombres + " " + contrato.Comercial.Apellido,
                        Material = contrato.Material == null ? "" : contrato.Material.Descripcion,
                        Campania = contrato.Campana == null ? "" : contrato.Campana.Descripcion,
                        Provincia = !(contrato is Contrato) || (contrato as Contrato).Provincia == null ? "" : (contrato as Contrato).Provincia.Nombre,
                        TipoNegocio = (contrato.TipoNegocio == null ? "" : (contrato is Contrato && (contrato as Contrato).Madre == true) ? "CONVENIO" : (contrato is Contrato && (contrato as Contrato).Madre == false) ? "FIJ. CONVENIO" : contrato.TipoNegocio.Descripcion),
                        Localidad = !(contrato is Contrato) || (contrato as Contrato).Localidad == null ? "" : (contrato as Contrato).Localidad.Nombre,
                        Observacion = contrato.Observacion != null ? contrato.Observacion : "",
                        FijacionDePrecioContratoId = (contrato is FijacionDePrecioContrato) ? (int?)(contrato as FijacionDePrecioContrato).Id : null,
                        Sustentable = (contrato is Contrato) && (contrato as Contrato).ImporteSustentable != null && (contrato as Contrato).ImporteSustentable > 0,
                        Dolarizado = (contrato is Contrato) && (contrato as Contrato).FechaDolarizado != null,
                        Pesificado = contrato.DiasPesificado != null,
                        Negocio = contrato is ContratoAcuerdo ?contrato.Id.ToString() :(contrato is FijacionDePrecioContrato && contrato.EstadoId == (int)EnumEstadoContrato.Finalizado) ? (contrato as FijacionDePrecioContrato).FijacionSAP : contrato.ContratoSAP != "0" ? contrato.ContratoSAP : "",
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
                        ComercialCreadorId = contrato.ComercialCreadorId,
                        ComercialCreador = (contrato as FijacionDePrecioContrato).ProveedorCreadorId != null ? contrato.UsuarioId : contrato.ComercialCreador == null ? contrato.Comercial.Nombres + " " + contrato.Comercial.Apellido : contrato.ComercialCreador.Nombres + " " + contrato.ComercialCreador.Apellido,
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
                        NivelTarifa = (contrato is Contrato) ? (contrato as Contrato).NivelTarifa.Descripcion : "",
                        TarifaFlete = (contrato is Contrato) ? (contrato as Contrato).TarifaFlete : null,
                        Compensacion = (contrato is Contrato) ? (contrato as Contrato).Compensacion : null,
                        Acuerdo = (contrato is Contrato) ? (contrato as Contrato).ContratoAcuerdoId : null,
                        Rechazo = (contrato is FijacionDePrecioContrato) ? (contrato as FijacionDePrecioContrato).MotivoRechazo : null,
                        OcultarEnTablero = contrato.OcultarEnTablero,
                        FechaCierta = DbFunctions.TruncateTime((contrato as Contrato).FechaCierta),
                    };

                return queryNegocios;
            }
            else
            {
                var cuit = PermisosHelper.ObtenerCuit();
                var queryFijacion =
                    from fijac in contexto.Set<FijacionDePrecioContrato>()
                    where fijac.ProveedorCreador.CUIT == cuit
                    select new BasicoContrato()
                    {
                        Id = fijac.Id,
                        ContratoId = fijac.ContratoId.HasValue ? fijac.ContratoId.Value : 0,
                        ProveedorId = fijac.ProveedorId ?? 0,
                        CorredorId = fijac.CorredorId != null ? fijac.CorredorId.Value : 0,
                        ComercialId = fijac.ComercialId,
                        ComercialZonaId = fijac.ContratoId.HasValue ? fijac.Contrato.Comercial.GrupoDeComprasId : null,
                        ComercialZonaDescripcion = fijac.ContratoId.HasValue ? fijac.Contrato.Comercial.GrupoDeCompras.Descripcion : "",
                        MaterialId = fijac.MaterialId,
                        TipoNegocioId = 3,
                        Cantidad = fijac.Cantidad,
                        Precio = fijac.Precio,
                        PrecioPlazo = fijac.Precio.ToString(),
                        FechaEntrega = null,
                        CampanaId = fijac.CampanaId ?? 0,
                        FechaDesde = DbFunctions.TruncateTime(fijac.FechaDesde),
                        FechaHasta = DbFunctions.TruncateTime(fijac.FechaHasta),
                        MonedaId = fijac.MonedaId,
                        Moneda = fijac.Moneda == null ? "" : fijac.Moneda.Descripcion,
                        Fecha = DbFunctions.TruncateTime(fijac.Fecha),
                        Hora = SqlFunctions.DateName("hour", fijac.Fecha) + ":" + SqlFunctions.DateName("minute", fijac.Fecha),
                        Fecha_Order = fijac.Fecha,
                        GrupoCompra = 0,
                        GrupoCompraDescripcion = null,
                        ProvinciaId = null,
                        LocalidadId = null,
                        Base = null,
                        Importe_Sustentable = null,
                        MonedaId_Sustentable = "",
                        Moneda_Sustentable = "",
                        Fecha_Dolarizado = null,
                        Dias_Pesificado = fijac.DiasPesificado,
                        NoInformaSIO = null,
                        Estado = fijac.EstadoId == 9 || fijac.EstadoId == 1 || fijac.EstadoId == 7 ? 9 : fijac.EstadoId == 6 || fijac.EstadoId == 8 ? 6 : fijac.EstadoId == 5 ? 5 : 2,
                        Estado_Contrato = fijac.EstadoId == 9 || fijac.EstadoId == 1 || fijac.EstadoId == 7 ? fijac.EstadoId == 6 || fijac.EstadoId == 8 ? fijac.EstadoId == 5 ? "Finalizado" : "Rechazado" : "Carga" : "Confirmado",
                        Estado_Order = fijac.Estado.Orden,
                        UsuarioId = "",
                        ContratoSAP = fijac.ContratoSAP,
                        Ampliaciones = fijac.Ampliaciones,
                        Cuit = fijac.Proveedor == null ? "" : fijac.Proveedor.CUIT,
                        Proveedor = fijac.Proveedor == null ? "" : fijac.Proveedor.RazonSocial,
                        Corredor = fijac.Corredor == null ? "" : fijac.Corredor.RazonSocial,
                        Comercial = fijac.Comercial == null ? "" : fijac.Comercial.Nombres + " " + fijac.Comercial.Apellido,
                        Material = fijac.Material == null ? "" : fijac.Material.Descripcion,
                        Campania = fijac.Campana == null ? "" : fijac.Campana.Descripcion,
                        Provincia = "",
                        TipoNegocio = "FIJACION",
                        Localidad = "",
                        Observacion = fijac.Observacion ?? "",
                        FijacionDePrecioContratoId = fijac.Id,
                        Sustentable = false,
                        Dolarizado = false,
                        Pesificado = fijac.DiasPesificado != null && fijac.DiasPesificado > 0,
                        Negocio = fijac.EstadoId == (int)EnumEstadoContrato.Finalizado ? fijac.FijacionSAP : fijac.ContratoSAP,
                        DestinoId = fijac.DestinoId,
                        DestinoDescripcion = fijac.Destino != null ? fijac.Destino.Descripcion : "",
                        CantidadCamiones = null,
                        Consignatario = false,
                        PlanCanje = false,
                        CD = null,
                        Warrant = null,
                        PagoDirectoVendedor = null,
                        EstablecimientoPropio = null,
                        BoletoId = null,
                        BolsaId = null,
                        BoletoDescripcion = "",
                        BolsaDescripcion = "",
                        DesdeFijacion = null,
                        HastaFijacion = null,
                        CondicionFijacion = null,
                        CondicionFijacionDescripcion = "",
                        ClasificacionId = null,
                        ClasificacionDescripcion = "",
                        CalidadDescripcion = "",
                        MercsDeposito = null,
                        ComercialCreadorId = fijac.ProveedorCreadorId,
                        ComercialCreador = fijac.UsuarioId != null ? fijac.UsuarioId : fijac.ProveedorCreadorId != null ? fijac.ProveedorCreador.RazonSocial : " ",
                        ContratoCorredor = "",
                        ContratoVendedor = "",
                        SelCargoMOA = null,
                        SelCargoVendedor = null,
                        Posicion = "",
                        TipoFason = "",
                        FasonId = 0,
                        Operador = "",
                        OperadorId = 0,
                        AgenteId = 0,
                        PrecioNeto = fijac.PrecioNeto,
                        StandardCalidadId = null,
                        StandardDeCalidadDescripcion = "",
                        Pizarra = fijac.Pizarra ?? null,
                        PagoDiferido = fijac.PagoDiferido ?? null,
                        ZonaId = null,
                        ZonaDescripcion = "",
                        AcuerdoId = null,
                        ImporteFinanciero = fijac.AperturaPrecio.FirstOrDefault(t => t.ConceptoAperturaPrecioId == 1).Importe,
                        ImporteRedespacho = fijac.AperturaPrecio.FirstOrDefault(t => t.ConceptoAperturaPrecioId == 2).Importe,
                        PorcentajeComision = fijac.AperturaPrecio.FirstOrDefault(t => t.ConceptoAperturaPrecioId == 3).Porcentaje,
                        ImporteComision = fijac.AperturaPrecio.FirstOrDefault(t => t.ConceptoAperturaPrecioId == 3).Importe,
                        ImporteBonificacion = fijac.AperturaPrecio.FirstOrDefault(t => t.ConceptoAperturaPrecioId == 4).Importe,
                        PorcentajeBonificacion = fijac.AperturaPrecio.FirstOrDefault(t => t.ConceptoAperturaPrecioId == 4).Porcentaje,
                        NivelTarifa = "",
                        TarifaFlete = null,
                        Compensacion = null,
                        Acuerdo = null,
                        Rechazo = fijac.MotivoRechazo,
                        OcultarEnTablero = fijac.OcultarEnTablero
                    };
                return queryFijacion;
            }
        }
    }
}
