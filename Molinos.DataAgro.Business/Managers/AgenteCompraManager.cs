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
    public class AgenteCompraManager : IAgenteCompraManager
    {
        private readonly IRepositorio repositorio;
        private readonly IHedgeManager oHedgeManager;
        private readonly ILogDataAgroManager logDataAgroManager;
        private ILogger logger;
        private readonly IDiasHabilesAgent oDiasHabilesAgent;


        public AgenteCompraManager(ILogger logger, IRepositorio repositorio, IHedgeManager oHedgeManager, ILogDataAgroManager logDataAgroManager, IDiasHabilesAgent oDiasHabilesAgent)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.oHedgeManager = oHedgeManager;
            this.logDataAgroManager = logDataAgroManager;
            this.oDiasHabilesAgent = oDiasHabilesAgent;
        }

        //--------------------------------------------------
        //  Metodos Publicos
        //--------------------------------------------------

        private Resultado Validar(AgenteCompra oParam, Resultado oErrorMessages)
        {
            bool posicionIncorrecta = false;
            if (oParam.MaterialId == 0)
            {
                oErrorMessages.Error("Material", "El campo 'Material' no debe estar vacío");
            }

            if (oParam.Cantidad == 0)
            {
                oErrorMessages.Error("Cantidad", "El campo 'Cantidad' no debe estar vacío");
            }
            if (oParam.Precio == 0)
            {
                oErrorMessages.Error("Precio", "El campo 'Precio' no debe estar vacío");
            }
            if (oParam.CampanaId == null || oParam.CampanaId == 0)
            {
                oErrorMessages.Error("Campaña", "El campo 'Campaña' no debe estar vacío");
            }
            if (oParam.MonedaId == null)
            {
                oErrorMessages.Error("MonedaId", "El campo 'Moneda' no debe estar vacío");
            }
            if (oParam.OperadorId == 0)
            {
                oErrorMessages.Error("OperadorId", "El campo 'Operador' no debe estar vacío");
            }
            if (oParam.TipoAgenteCompraId == 0 || oParam.TipoAgenteCompraId == null)
            {
                oErrorMessages.Error("TipoAgenteCompraId", "El campo 'Tipo Agente' no debe estar vacío");
            }
            if (oParam.Posicion == "" || oParam.Posicion == null)
            {
                oErrorMessages.Error("Posicion", "El campo 'Posicion' no debe estar vacio");
                posicionIncorrecta = true;
            }
            else
            {
                if (oParam.Posicion.Split('.').Length != 2)
                {
                    oErrorMessages.Error("Posicion", "El campo 'Posicion' no tiene el formato correcto (MM.YYYY)");
                    posicionIncorrecta = true;
                }
                else
                {
                    int i = 0;

                    if (!int.TryParse(oParam.Posicion.Split('.')[1], out i) || !int.TryParse(oParam.Posicion.Split('.')[0], out i))
                    {
                        oErrorMessages.Error("Posicion", "El campo 'Posicion' no tiene el formato correcto (MM.YYYY)");
                        posicionIncorrecta = true;
                    }
                    else
                    {
                        if (int.Parse(oParam.Posicion.Split('.')[0]) > 12)
                        {
                            oErrorMessages.Error("Posicion", "El campo 'Posicion' no tiene el formato correcto (MM.YYYY)");
                            posicionIncorrecta = true;
                        }
                        if (oParam.Posicion.Split('.')[1].Length < 4)
                        {
                            oErrorMessages.Error("Posicion", "El campo 'Posicion' no tiene el formato correcto (MM.YYYY)");
                            posicionIncorrecta = true;
                        }

                    }
                }
            }
            var rangosPrecio = repositorio.Listar<RangoPrecio>();
            if (rangosPrecio.Exists(x => x.MaterialId == oParam.MaterialId && x.MonedaId == oParam.MonedaId && (x.PrecioMaximo < oParam.Precio || x.PrecioMinimo > oParam.Precio)))
            {
                oErrorMessages.Error("Precio", "Precio fuera de Rango");
            }
            var dia = oHedgeManager.Dia();
            if (dia != null)
            {
                if (dia.Cerrado.Value)
                {
                    oErrorMessages.Error("", "El Día de Operación ya se ha cerrado");
                }
            }
            if (oParam.FechaOperacion > DateTime.Now.Date)
            {
                oErrorMessages.Error("FechaOperacion", "La Fecha tiene que ser menor o igual al día de la fecha.");
            }
            //else
            //{
            //    var agente = repositorio.Obtener<AgenteCompra>(oParam.Id);
            //    if (oParam.Id > 0)
            //    {
            //        if ((oParam.FechaOperacion != agente.Fecha.Date && oParam.FechaOperacion < agente.Fecha.Date))
            //        {

            //            var diaAnterior = oDiasHabilesAgent.UltimoDiaHabil(agente.Fecha.Date);

            //            if ((oParam.FechaOperacion < diaAnterior && !PermisosHelper.Is(PermisosDataAgro.NegociosFechaMayorDiaAnterior)))
            //            {
            //                oErrorMessages.Error("FechaOperacion", "La Fecha Operación no puede ser anterior al ultimo día habil." + diaAnterior.ToString("dd/MM/yyyy"));
            //            }
            //        }
            //    }
            //    else
            //    {
            //        if (oParam.FechaOperacion < DateTime.Now.Date)
            //        {
            //            var diaAnterior = oDiasHabilesAgent.UltimoDiaHabil(null);

            //            if ((oParam.FechaOperacion < diaAnterior && !PermisosHelper.Is(PermisosDataAgro.NegociosFechaMayorDiaAnterior)))
            //            {
            //                oErrorMessages.Error("FechaOperacion", "La Fecha Operación no puede ser anterior al ultimo día habil." + diaAnterior.ToString("dd/MM/yyyy"));
            //            }
            //        }

            //    }
            //}

            //if (ValidarFechaAgenteMP(oParam))
            //{
            //    oErrorMessages.Error("FechaOperacion", "La fecha de operación para Agente de Compras MP no puede ser uno de los últimos 5 días hábiles del mes.");
            //}

            if (!posicionIncorrecta)
            {
                int cantDiasHabilesRestar = 5;
                int mes = Int32.Parse(oParam.Posicion.Split('.')[0]);
                int anio = Int32.Parse(oParam.Posicion.Split('.')[1]);
                DateTime fechaInicioMes = new DateTime(anio, mes, 1);
                var ultimosDiasHabilesDelMes = oDiasHabilesAgent.ObtenerDiasHabilesDelMes(fechaInicioMes);
                if (ultimosDiasHabilesDelMes != null)
                {
                    var posicionFecha = ultimosDiasHabilesDelMes.Count - cantDiasHabilesRestar;
                    var fechaLimite = ultimosDiasHabilesDelMes[posicionFecha];
                    if (oParam.FechaOperacion >= fechaLimite)
                    {
                        oErrorMessages.Error("FechaOperacion", "La fecha de operación no puede ser uno de los últimos 5 días hábiles del mes de la posición.");
                    }
                }
            }

            return oErrorMessages;
        }

        public GrabarAgenteResult GrabarAgente(AgenteCompra oAgente)
        {
            var oEntityErrors = new GrabarAgenteResult();
            AgenteCompra oAgenteSave = null;
            this.Validar(oAgente, oEntityErrors);

            if (oEntityErrors.HayErrores)
            {
                return oEntityErrors;
            }
            if (oAgente.Id != 0)
            {
                oAgenteSave = repositorio.Obtener<AgenteCompra>(oAgente.Id);
                if (oAgenteSave.EstadoId > (int)EnumEstadoContrato.Con_Error)
                {
                    oEntityErrors.Error("", "El Agente de Compras no se puede modificar");
                    return oEntityErrors;
                }
                var estado = PermisosHelper.Is(PermisosDataAgro.NegociosConfirmados) ? 2 : 7;
                if (estado == 7 && oAgenteSave.EstadoId == (int)EnumEstadoContrato.Confirmado)
                {
                    string jsonContrato = JsonConvert.SerializeObject(oAgenteSave, new JsonSerializerSettings()
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver(),
                        ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                        PreserveReferencesHandling = PreserveReferencesHandling.Objects
                    });
                    oAgenteSave.NegocioHistorico.Add(new NegocioHistorico { Datos = jsonContrato, Fecha = DateTime.Now, NegocioId = oAgenteSave.Id, TipoNegocioId = oAgenteSave.TipoNegocioId, ComercialId = oAgenteSave.ComercialId });
                }

                oAgenteSave.Precio = oAgente.Precio;
                oAgenteSave.Cantidad = oAgente.Cantidad;
                oAgenteSave.EstadoId = estado;
                oAgenteSave.OperadorId = oAgente.OperadorId;
                oAgenteSave.ComercialId = oAgente.ComercialId;
                oAgenteSave.MonedaId = oAgente.MonedaId;
                oAgenteSave.MaterialId = oAgente.MaterialId;
                oAgenteSave.Posicion = oAgente.Posicion;
                oAgenteSave.FechaDesde = oAgente.FechaDesde;
                oAgenteSave.FechaHasta = oAgente.FechaHasta;
                oAgenteSave.ComercialCreadorId = oAgente.ComercialCreadorId;
                oAgenteSave.CampanaId = oAgente.CampanaId;
                oAgenteSave.FechaOperacion = oAgente.FechaOperacion;


            }
            else
            {
                var estado = (PermisosHelper.Is(PermisosDataAgro.NegociosConfirmados) || PermisosHelper.Is(PermisosDataAgro.CrearNegociosAgente)) ? 2 : 1;
                if (PermisosHelper.Is(PermisosDataAgro.NegociosConfirmados) || PermisosHelper.Is(PermisosDataAgro.CrearNegociosAgente))
                {
                    oAgente.FechaConfirmacion = DateTime.Now;
                }
                oAgente.Fecha = DateTime.Now;
                oAgente.EstadoId = estado;
                oAgente.FechaOperacion = oAgente.FechaOperacion;
                repositorio.Agregar(oAgente);
            }

            try
            {
                repositorio.GuardarCambios();
                var logCambios = oAgenteSave ?? oAgente;
                var tipoDeAccion = (oAgenteSave == null) ? TipoAccionLogDataAgro.Crear : TipoAccionLogDataAgro.Modificar;
                logDataAgroManager.LogCambiosDataAgro(TraerAgente(logCambios.Id), tipoDeAccion, logCambios.GetType());
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }

            return oEntityErrors;
        }
        public GrabarAgenteResult FinalizarAgente(int agenteId)
        {
            var oEntityErrors = new GrabarAgenteResult();
            var oFasonSave = repositorio.Obtener<AgenteCompra>(agenteId);

            if (oFasonSave.EstadoId == (int)EnumEstadoContrato.Confirmado ||
                oFasonSave.EstadoId == (int)EnumEstadoContrato.Pendiente ||
                oFasonSave.EstadoId == (int)EnumEstadoContrato.Reconfirmar ||
                oFasonSave.EstadoId == (int)EnumEstadoContrato.Con_Error)
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
                    logDataAgroManager.LogCambiosDataAgro(TraerAgente(oFasonSave.Id), TipoAccionLogDataAgro.Crear, oFasonSave.GetType());

                }
                catch (Exception ex)
                {
                    oFasonSave.Estado = repositorio.Obtener<EstadoContrato>((int)EnumEstadoContrato.Con_Error);
                    repositorio.GuardarCambios();
                    logDataAgroManager.LogCambiosDataAgro(TraerAgente(oFasonSave.Id), TipoAccionLogDataAgro.Crear, oFasonSave.GetType());

                    oEntityErrors.Error("", ex.Message);
                    logger.Error(ex);
                }
            }
            else
            {
                if (oFasonSave.EstadoId == (int)EnumEstadoContrato.Finalizado)
                {
                    oEntityErrors.Error("", "Agente de Compras ya se encuentra Finalizadao");
                }
                else if (oFasonSave.EstadoId == (int)EnumEstadoContrato.Rechazado)
                {
                    oEntityErrors.Error("", "Agente de Compras ya ha sido Rechazado");
                }
            }
            return oEntityErrors;
        }
        public GrabarAgenteResult BorrarAgente(AgenteCompra oAgente)
        {
            var oEntityErrors = new GrabarAgenteResult();

            if (string.IsNullOrEmpty(oAgente.MotivoRechazo) || string.IsNullOrWhiteSpace(oAgente.MotivoRechazo))
            {
                oEntityErrors.Error("Rechazo", "Debe indicar motivo de rechazo");
                return oEntityErrors;
            }
            var oContratoSave = repositorio.Obtener<AgenteCompra>(oAgente.Id);
            oContratoSave.MotivoRechazo = oAgente.MotivoRechazo;
            if (oContratoSave.EstadoId != (int)EnumEstadoContrato.Rechazado && oContratoSave.EstadoId != (int)EnumEstadoContrato.Eliminado)
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
                            AgenteCompra contratoOriginal = JsonConvert.DeserializeObject<AgenteCompra>(historico.Datos);
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
                    logDataAgroManager.LogCambiosDataAgro(TraerAgente(oContratoSave.Id), TipoAccionLogDataAgro.Eliminar, oContratoSave.GetType());

                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
            }
            else
            {
                oEntityErrors.Error("", "Este Negocio no se puede rechazar por estar Rechazado o Eliminado");
            }
            return oEntityErrors;
        }
        public BasicoContrato TraerAgente(int contratoId)
        {
            var contrato = repositorio.Obtener<AgenteCompra, BasicoContrato>(x => x.Id == contratoId, x => new BasicoContrato
            {
                Id = x.Id,
                ComercialId = x.ComercialId,
                FechaFormateado = SqlFunctions.DateName("day", x.Fecha).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)x.Fecha.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", x.Fecha),
                TipoNegocioId = 5,
                MaterialId = x.MaterialId,
                Cantidad = x.Cantidad,
                Precio = x.Precio,
                MonedaId = x.MonedaId,
                Estado = x.EstadoId,
                Posicion = x.Posicion,
                Operador = x.Operador.Descripcion,
                OperadorId = x.OperadorId,
                AgenteId = x.Id,
                FechaHasta = x.FechaHasta,
                FechaDesde = x.FechaDesde,
                FechaHastaFormateado = SqlFunctions.DateName("day", x.FechaHasta).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)x.FechaHasta.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", x.FechaHasta),
                FechaDesdeFormateado = SqlFunctions.DateName("day", x.FechaDesde).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)x.FechaDesde.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", x.FechaDesde),
                Proveedor = x.Proveedor.RazonSocial,
                ProveedorId = x.ProveedorId ?? 0,
                PrecioNeto = x.PrecioNeto,
                Moneda = x.Moneda.Descripcion,
                Material = x.Material.Descripcion,
                StandardDeCalidadDescripcion = x.StandardDeCalidad.Descripcion,
                DestinoDescripcion = x.Destino.Descripcion,
                Campania = x.Campana.Descripcion,
                Comercial = x.Comercial.Apellido + " " + x.Comercial.Nombres,
                Fecha_Dolarizado = x.FechaDolarizado,
                CampanaId = x.CampanaId ?? 0,
                TipoNegocio = x.TipoNegocio.Descripcion,
                TipoAgenteCompraId = x.TipoAgenteCompraId,
                FechaOperacion = x.FechaOperacion,
                FechaOperacionFormateado = SqlFunctions.DateName("day", x.FechaOperacion).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)x.FechaOperacion.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", x.FechaOperacion),
            });
            return contrato;
        }
        public GrabarAgenteResult GrabarAmpliacionAgente(AgenteCompra oAgente)
        {
            var oAgenteSave = repositorio.Obtener<AgenteCompra>(oAgente.Id);

            var oEntityErrors = new GrabarAgenteResult();
            var heedgeDia = oHedgeManager.Dia();
            if (heedgeDia != null && (heedgeDia.Cerrado ?? false))
            {
                oEntityErrors.Error("", "El Día de Operación ya se ha cerrado");
                return oEntityErrors;
            }
            if (oAgenteSave.EstadoId <= (int)EnumEstadoContrato.Con_Error)
            {
                oAgenteSave.Ampliaciones = oAgente.Ampliaciones.Value;
                oAgenteSave.EstadoId = (int)EnumEstadoContrato.Reconfirmar;
                try
                {
                    repositorio.GuardarCambios();
                    logDataAgroManager.LogCambiosDataAgro(TraerAgente(oAgenteSave.Id), TipoAccionLogDataAgro.Modificar, oAgenteSave.GetType());
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    throw;
                }
            }
            else
            {
                oEntityErrors.Error("", "El Agente de Compras no se puede modificar");
            }
            return oEntityErrors;
        }
        public Resultado ConfirmarAgenteCompra(int id, int usuarioConfirmador)
        {
            var oEntityErrors = new Resultado();

            var contrato = repositorio.Obtener<AgenteCompra>(id);
            if (contrato.EstadoId == (int)EnumEstadoContrato.Pendiente || contrato.EstadoId == (int)EnumEstadoContrato.Reconfirmar)
            {
                contrato.EstadoId = (int)EnumEstadoContrato.Confirmado;

                logger.Debug("Confirmando el Agente:" + id);
                try
                {
                    contrato.UsuarioConfirmadorId = usuarioConfirmador;
                    contrato.FechaConfirmacion = DateTime.Now;
                    repositorio.GuardarCambios();
                    logDataAgroManager.LogCambiosDataAgro(TraerAgente(contrato.Id), TipoAccionLogDataAgro.Crear, contrato.GetType());
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    throw;
                }
            }
            else
            {
                oEntityErrors.Error("Confirmar", "El Agente no se puede confirmar");
            }
            return oEntityErrors;
        }

        //private bool ValidarFechaAgenteMP(AgenteCompra contrato)
        //{
        //    var fechaElegida = contrato.FechaOperacion;
        //    var diasHabiles = oDiasHabilesAgent.ObtenerDiasHabilesDelMes(fechaElegida);
        //    bool fechaInvalida = false;
        //    int cont = 0;
        //    for (int i = diasHabiles.Count - 1; cont < 5 && !fechaInvalida; i--)
        //    {
        //        if (fechaElegida == diasHabiles[i])
        //            fechaInvalida = true;
        //        cont++;
        //    }

        //    return fechaInvalida;
        //}

    }
}




