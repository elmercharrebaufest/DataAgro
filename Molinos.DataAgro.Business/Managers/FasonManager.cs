using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Globalization;
using System.Linq;

namespace Molinos.DataAgro.Business.Managers
{
    public class FasonManager : IFasonManager
    {
        private readonly IRepositorio repositorio;
        private readonly IProveedorManager mobjProveedorManager;
        private readonly ILogger logger;
        private readonly ILogDataAgroManager logDataAgroManager;

        public FasonManager(ILogger logger, IRepositorio repositorio, IProveedorManager oMSProveedorManager, ILogDataAgroManager logDataAgroManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            mobjProveedorManager = oMSProveedorManager;
            this.logDataAgroManager = logDataAgroManager;
        }

        private Resultado Validar(Fason oParam, Resultado oErrorMessages)
        {
            var proveedor = repositorio.Obtener<Proveedor>(x => x.ProveedorId == oParam.ProveedorId);
            if (proveedor == null)
            {
                oErrorMessages.Error("ProveedorId", "El campo 'Proveedor' es obligatorio");
                return oErrorMessages;
            }
            if (proveedor.Deshabilitado.HasValue && proveedor.Deshabilitado.Value != false)
            {
                oErrorMessages.Error("ProveedorId", "Proveedor deshabilitado");
            }
            if (!oParam.ProveedorId.HasValue || oParam.ProveedorId == 0)
            {
                oErrorMessages.Error("ProveedorId", "El campo 'Fasonero' no debe estar vacio");
            }
            if (oParam.MaterialId == 0)
            {
                oErrorMessages.Error("Material", "El campo 'Material' no debe estar vacio");
            }

            if (oParam.Cantidad == 0)
            {
                oErrorMessages.Error("Cantidad", "El campo 'Cantidad' no debe estar vacio");
            }
            if (oParam.Precio == 0)
            {
                oErrorMessages.Error("Precio", "El campo 'Precio' no debe estar vacio");
            }
            if (oParam.MonedaId == null)
            {
                oErrorMessages.Error("MonedaId", "El campo 'Moneda' no debe estar vacio");
            }
            if (oParam.TipoFasonId == 0)
            {
                oErrorMessages.Error("TipoFasonId", "El campo 'Tipo Fason' no debe estar vacio");
            }
            if (oParam.CampanaId == null || oParam.CampanaId == 0)
            {
                oErrorMessages.Error("CampanaId", "El campo 'Campaña' no debe estar vacio");
            }
            if (oParam.Posicion == "" || oParam.Posicion == null)
            {
                oErrorMessages.Error("Posicion", "El campo 'Posicion' no debe estar vacio");
            }
            else
            {
                if (oParam.Posicion.Split('.').Length != 2)
                {
                    oErrorMessages.Error("Posicion", "El campo 'Posicion' no tiene el formato correcto (MM.YYYY)");
                }
                else
                {
                    int i = 0;

                    if (!int.TryParse(oParam.Posicion.Split('.')[1], out i) || !int.TryParse(oParam.Posicion.Split('.')[0], out i))
                    {
                        oErrorMessages.Error("Posicion", "El campo 'Posicion' no tiene el formato correcto (MM.YYYY)");
                    }
                    else
                    {
                        if (int.Parse(oParam.Posicion.Split('.')[0]) > 12)
                        {
                            oErrorMessages.Error("Posicion", "El campo 'Posicion' no tiene el formato correcto (MM.YYYY)");
                        }
                        if (oParam.Posicion.Split('.')[1].Length < 4)
                        {
                            oErrorMessages.Error("Posicion", "El campo 'Posicion' no tiene el formato correcto (MM.YYYY)");
                        }

                    }
                }                
            }

            if (oParam.FechaDesde.Year == 1)
            {
                oErrorMessages.Error("FechaDesde", "El campo 'Fecha Desde' no debe estar vacio");
            }
            if (oParam.FechaHasta.Year == 1)
            {
                oErrorMessages.Error("FechaHasta", "El campo 'Fecha Hasta' no debe estar vacio");
            }
            var rangosPrecio = repositorio.Listar<RangoPrecio>();
            if (rangosPrecio.Exists(x => (x.PrecioMaximo < oParam.Precio || x.PrecioMinimo > oParam.Precio) && x.MaterialId == oParam.MaterialId && x.MonedaId == oParam.MonedaId))
            {
                oErrorMessages.Error("Precio", "Precio fuera de Rango");
            }
            return oErrorMessages;
        }

        public GrabarFasonResult GrabarFason(Fason oFason)
        {
            var oEntityErrors = new GrabarFasonResult();

            this.Validar(oFason, oEntityErrors);

            if (oEntityErrors.HayErrores)
            {
                return oEntityErrors;
            }
            Fason oFasonSave = null;
            var tipoCambio = oFason.Id != 0 ? TipoAccionLogDataAgro.Modificar : TipoAccionLogDataAgro.Crear;
            if (oFason.Id != 0)
            {
                oFasonSave = repositorio.Obtener<Fason>(oFason.Id);
                if (oFasonSave.Estado.EstadoContratoId > (int)EnumEstadoContrato.Con_Error)
                {
                    oEntityErrors.Error("", "El Negocio Fasón no se puede modificar");
                    return oEntityErrors;
                }
                var estado = PermisosHelper.Is(PermisosDataAgro.NegociosConfirmados) ? 2 : 7;
                if (estado == 7)
                {
                    if (oFasonSave.EstadoId == (int)EnumEstadoContrato.Confirmado)
                    {
                        string jsonContrato = JsonConvert.SerializeObject(oFasonSave, new JsonSerializerSettings()
                        {
                            ContractResolver = new CamelCasePropertyNamesContractResolver(),
                            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                            PreserveReferencesHandling = PreserveReferencesHandling.Objects
                        });
                        oFasonSave.NegocioHistorico.Add(new NegocioHistorico { Datos = jsonContrato, Fecha = DateTime.Now, NegocioId = oFasonSave.Id, TipoNegocioId = oFasonSave.TipoNegocioId, ComercialId = oFasonSave.ComercialId });
                    }

                }
                oFasonSave.Precio = oFason.Precio;
                oFasonSave.Cantidad = oFason.Cantidad;
                oFasonSave.EstadoId = estado;
                oFasonSave.Proveedor = oFason.Proveedor;
                oFasonSave.ComercialId = oFason.ComercialId;
                oFasonSave.MonedaId = oFason.MonedaId;
                oFasonSave.MaterialId = oFason.MaterialId;
                oFasonSave.CampanaId = oFason.CampanaId;
                oFasonSave.Posicion = oFason.Posicion;
                oFasonSave.TipoFasonId = oFason.TipoFasonId;
                oFasonSave.FechaDesde = oFason.FechaDesde;
                oFasonSave.FechaHasta = oFason.FechaHasta;
                oFasonSave.TrigoEspecial = oFason.TrigoEspecial;
                oFasonSave.ComercialCreadorId = oFason.ComercialCreadorId;
            }
            else
            {
                var estado = PermisosHelper.Is(PermisosDataAgro.NegociosConfirmados) ? 2 : 1;
                if (PermisosHelper.Is(PermisosDataAgro.NegociosConfirmados))
                {
                    oFason.FechaConfirmacion = DateTime.Now;
                }
                oFason.Fecha = DateTime.Now;
                oFason.EstadoId = estado;
                oFason.FechaOperacion = DateTime.Now;

                repositorio.Agregar(oFason);
            }

            try
            {
                repositorio.GuardarCambios();
                logDataAgroManager.LogCambiosDataAgro(TraerFason(oFason.Id), tipoCambio, oFason.GetType());
                //logDataAgroManager.LogCambiosDataAgro(TraerFason(oFasonSave ?? oFason, (oFasonSave == null) ? TipoAccionLogDataAgro.Crear : TipoAccionLogDataAgro.Modificar);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
            return oEntityErrors;
        }
        public GrabarFasonResult FinalizarFason(int fijacionDePrecioContratoId)
        {
            var oEntityErrors = new GrabarFasonResult();
            var oFasonSave = repositorio.Obtener<Fason>(fijacionDePrecioContratoId);

            if (oFasonSave.Estado.EstadoContratoId == (int)EnumEstadoContrato.Confirmado || oFasonSave.Estado.EstadoContratoId == (int)EnumEstadoContrato.Con_Error)
            {
                try
                {
                    if (oFasonSave.Ampliaciones != null && oFasonSave.Ampliaciones != 0)
                    {
                        oFasonSave.Cantidad += oFasonSave.Ampliaciones.Value;
                        oFasonSave.Ampliaciones = null;
                    }
                    oFasonSave.Estado = repositorio.Obtener<EstadoContrato>((int)EnumEstadoContrato.Finalizado);
                    repositorio.GuardarCambios();
                    logDataAgroManager.LogCambiosDataAgro(TraerFason(oFasonSave.Id), TipoAccionLogDataAgro.Crear, oFasonSave.GetType());
                }
                catch (Exception ex)
                {
                    oFasonSave.Estado = repositorio.Obtener<EstadoContrato>((int)EnumEstadoContrato.Con_Error);
                    repositorio.GuardarCambios();
                    logDataAgroManager.LogCambiosDataAgro(TraerFason(oFasonSave.Id), TipoAccionLogDataAgro.Crear, oFasonSave.GetType());

                    oEntityErrors.Error("", ex.Message);
                    logger.Error(ex);
                }
            }
            else
            {
                if (oFasonSave.Estado.EstadoContratoId == (int)EnumEstadoContrato.Finalizado)
                {
                    oEntityErrors.Error("", "Fasón ya se encuentra Finalizadao");
                }
                else if (oFasonSave.Estado.EstadoContratoId == (int)EnumEstadoContrato.Rechazado)
                {
                    oEntityErrors.Error("", "Fasón ya ha sido Rechazado");
                }
            }
            return oEntityErrors;
        }
        public GrabarFasonResult BorrarFason(Fason oFason)
        {
            var oEntityErrors = new GrabarFasonResult();

            if (string.IsNullOrEmpty(oFason.MotivoRechazo) || string.IsNullOrWhiteSpace(oFason.MotivoRechazo))
            {
                oEntityErrors.Error("Rechazo", "Debe indicar motivo de rechazo");
                return oEntityErrors;
            }
            var oContratoSave = repositorio.Obtener<Fason>(oFason.Id);
            oFason.EstadoId = oContratoSave.EstadoId;
            oContratoSave.MotivoRechazo = oFason.MotivoRechazo;
            if (oContratoSave.Estado.EstadoContratoId == (int)EnumEstadoContrato.Pendiente
                || oContratoSave.Estado.EstadoContratoId == (int)EnumEstadoContrato.Reconfirmar
                || oContratoSave.Estado.EstadoContratoId == (int)EnumEstadoContrato.Confirmado
                || oContratoSave.Estado.EstadoContratoId == (int)EnumEstadoContrato.Con_Error
                || oContratoSave.Estado.EstadoContratoId == (int)EnumEstadoContrato.Finalizado)
            {

                if (oContratoSave.EstadoId == (int)EnumEstadoContrato.Reconfirmar)
                {
                    if (oContratoSave.Ampliaciones > 0)
                    {
                        oContratoSave.Ampliaciones = 0;
                        if (oContratoSave.EstadoId == (int)EnumEstadoContrato.Reconfirmar)
                        {
                            oContratoSave.EstadoId = (int)EnumEstadoContrato.Confirmado;
                        }
                        else
                        {
                            oContratoSave.EstadoId = (int)EnumEstadoContrato.Pendiente;
                        }
                    }
                    else
                    {
                        var historico = oContratoSave.NegocioHistorico.LastOrDefault();
                        if (historico != null)
                        {
                            Fason contratoOriginal = JsonConvert.DeserializeObject<Fason>(historico.Datos);
                            oContratoSave.MaterialId = contratoOriginal.MaterialId;
                            oContratoSave.TipoNegocioId = contratoOriginal.TipoNegocioId;
                            oContratoSave.Cantidad = contratoOriginal.Cantidad;
                            oContratoSave.Precio = contratoOriginal.Precio;
                            oContratoSave.CampanaId = contratoOriginal.CampanaId;
                            oContratoSave.FechaDesde = contratoOriginal.FechaDesde;
                            oContratoSave.FechaHasta = contratoOriginal.FechaHasta;
                            oContratoSave.ProveedorId = contratoOriginal.ProveedorId;
                            oContratoSave.MonedaId = contratoOriginal.MonedaId;
                            oContratoSave.GrupoCompra = contratoOriginal.GrupoCompra;
                            oContratoSave.ComercialId = contratoOriginal.ComercialId;
                            oContratoSave.UsuarioId = contratoOriginal.UsuarioId;
                            oContratoSave.FechaDolarizado = contratoOriginal.FechaDolarizado;
                            oContratoSave.DiasPesificado = contratoOriginal.DiasPesificado;
                            oContratoSave.TrigoEspecial = contratoOriginal.TrigoEspecial;
                            oContratoSave.EstadoId = (int)EnumEstadoContrato.Confirmado;
                            oContratoSave.UsuarioId = contratoOriginal.UsuarioId;
                            oContratoSave.Ampliaciones = contratoOriginal.Ampliaciones;
                            oContratoSave.Observacion = contratoOriginal.Observacion;
                            oContratoSave.DestinoId = contratoOriginal.DestinoId;
                            oContratoSave.CondicionFijacionId = contratoOriginal.CondicionFijacionId;
                            oContratoSave.CD = contratoOriginal.CD;
                            oContratoSave.Warrant = contratoOriginal.Warrant;
                            oContratoSave.DesdeFijacion = contratoOriginal.DesdeFijacion;
                            oContratoSave.HastaFijacion = contratoOriginal.HastaFijacion;
                            oContratoSave.ComercialCreadorId = contratoOriginal.ComercialCreadorId;
                            oContratoSave.CorredorId = contratoOriginal.CorredorId;
                            oContratoSave.PrecioNeto = contratoOriginal.PrecioNeto;
                            oContratoSave.StandardDeCalidadId = contratoOriginal.StandardDeCalidadId;
                            oContratoSave.Pizarra = contratoOriginal.Pizarra;
                            oContratoSave.PagoDiferido = contratoOriginal.PagoDiferido;
                            oContratoSave.Dolarizado = contratoOriginal.Dolarizado;
                            oContratoSave.ContratoSAP = contratoOriginal.ContratoSAP;
                            oContratoSave.CampanaId = contratoOriginal.CampanaId;
                            oContratoSave.Posicion = contratoOriginal.Posicion;
                            oContratoSave.TipoFasonId = contratoOriginal.TipoFasonId;


                            if (oContratoSave.AperturaPrecio != null)
                            {
                                for (int i = oContratoSave.AperturaPrecio.Count - 1; i > -1; i--)
                                {
                                    repositorio.Remover(oContratoSave.AperturaPrecio.First());
                                }
                            }
                            else
                            {
                                oContratoSave.AperturaPrecio = new List<AperturaPrecio>();
                            }

                            if (contratoOriginal.AperturaPrecio != null)
                            {
                                foreach (var apertura in contratoOriginal.AperturaPrecio)
                                {
                                    repositorio.Agregar(new AperturaPrecio { ConceptoAperturaPrecioId = apertura.ConceptoAperturaPrecioId, Importe = apertura.Importe, MonedaId = apertura.MonedaId, NegocioId = apertura.NegocioId, Porcentaje = apertura.Porcentaje });
                                }
                            }
                        }
                        else
                        {
                            oContratoSave.EstadoId = (int)EnumEstadoContrato.Rechazado;
                        }
                    }


                }
                else
                {
                    oContratoSave.EstadoId = (int)EnumEstadoContrato.Rechazado;
                }
                try
                {
                    repositorio.GuardarCambios();
                    logDataAgroManager.LogCambiosDataAgro(TraerFason(oContratoSave.Id), TipoAccionLogDataAgro.Eliminar, oContratoSave.GetType());


                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    oEntityErrors.Error("", "Fasón no se puede rechazar");
                }
            }
            else
            {
                oEntityErrors.Error("", "Fasón no se puede rechazar");
            }
            return oEntityErrors;
        }
        public BasicoContrato TraerFason(int contratoId)
        {
            var contrato = repositorio.Obtener<Fason, BasicoContrato>(x => x.Id == contratoId, x => new BasicoContrato
            {
                Id = x.Id,
                ProveedorId = x.ProveedorId ?? 0,
                Proveedor = x.Proveedor == null ? "" : x.Proveedor.RazonSocial + " " + "(" + x.Proveedor.CUIT + ")",
                ComercialId = x.ComercialId,
                FechaFormateado = SqlFunctions.DateName("day", x.Fecha).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)x.Fecha.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", x.Fecha),
                TipoNegocioId = 4,
                Comercial = x.Comercial.Apellido + " " + x.Comercial.Nombres,
                FechaDesde = x.FechaDesde,
                FechaHasta = x.FechaHasta,
                MaterialId = x.MaterialId,
                Campania = x.Campana.Descripcion,
                CondicionFijacionDescripcion = (x.CondicionFijacion != null) ? x.CondicionFijacion.Descripcion : "",
                StandardDeCalidadDescripcion = (x.StandardDeCalidad != null) ? x.StandardDeCalidad.Descripcion : "",
                PrecioNeto = x.PrecioNeto,
                DestinoDescripcion = x.Destino.Descripcion,
                Cantidad = x.Cantidad,
                Precio = x.Precio,
                MonedaId = x.MonedaId,
                CampanaId = x.CampanaId ?? 0,
                Estado = x.EstadoId,
                Estado_Contrato = x.Estado.Descripcion,
                Posicion = x.Posicion,
                TipoFason = x.TipoFason.Descripcion,
                TipoFasonId = x.TipoFasonId,
                FasonId = x.Id,
                Moneda = (x.Moneda != null) ? x.Moneda.Descripcion : "",
                Pizarra = x.Pizarra,
                Material = x.Material.Descripcion,
                TipoNegocio = x.TipoNegocio.Descripcion,
                TrigoEspecial = x.TrigoEspecial,
                FechaDesdeFormateado = SqlFunctions.DateName("day", x.FechaDesde).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)x.FechaDesde.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", x.FechaDesde),
                FechaHastaFormateado = SqlFunctions.DateName("day", x.FechaHasta).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)x.FechaHasta.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", x.FechaHasta)
            });
            return contrato;
        }

        public GrabarContratoResult GrabarAmpliacionFason(Fason oFason)
        {
            var oFasonSave = repositorio.Obtener<Fason>(oFason.Id);
            var oEntityErrors = new GrabarContratoResult();

            if (oFasonSave.Estado.EstadoContratoId <= (int)EnumEstadoContrato.Con_Error)
            {
                oFasonSave.Ampliaciones = oFason.Ampliaciones.Value;
                oFasonSave.EstadoId = (int)EnumEstadoContrato.Reconfirmar;
                try
                {
                    repositorio.GuardarCambios();
                    logDataAgroManager.LogCambiosDataAgro(TraerFason(oFasonSave.Id), TipoAccionLogDataAgro.Modificar, oFasonSave.GetType());

                }
                catch (Exception ex)
                {
                    oEntityErrors.Error("", "El Contrato Fasón no se puede modificar");
                    logger.Error(ex);
                    throw;
                }
            }
            else
            {
                oEntityErrors.Error("", "El Contrato Fasón no se puede modificar");
            }
            return oEntityErrors;
        }

        public Resultado ConfirmarFason(int id, int usuarioConfirmador)
        {
            var oEntityErrors = new Resultado();

            var contrato = repositorio.Obtener<Fason>(id);
            if (contrato.EstadoId == (int)EnumEstadoContrato.Pendiente || contrato.EstadoId == (int)EnumEstadoContrato.Reconfirmar)
            {
                contrato.EstadoId = (int)EnumEstadoContrato.Confirmado;

                logger.Debug("Confirmando el Fason:" + id);
                try
                {
                    contrato.UsuarioConfirmadorId = usuarioConfirmador;
                    contrato.FechaConfirmacion = DateTime.Now;
                    repositorio.GuardarCambios();
                    logDataAgroManager.LogCambiosDataAgro(TraerFason(contrato.Id), TipoAccionLogDataAgro.Crear, contrato.GetType());
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    throw;
                }
            }
            else
            {
                oEntityErrors.Error("Confirmar", "El Fason no se puede confirmar");
            }
            return oEntityErrors;
        }
    }
}




