using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
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
        private ILogger logger;

        public AgenteCompraManager(ILogger logger, IRepositorio repositorio, IHedgeManager oHedgeManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.oHedgeManager = oHedgeManager;
        }

        //--------------------------------------------------
        //  Metodos Publicos
        //--------------------------------------------------

        private Resultado Validar(AgenteCompra oParam, Resultado oErrorMessages)
        {           
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
            if (oParam.OperadorId == 0)
            {
                oErrorMessages.Error("OperadorId", "El campo 'Operador' no debe estar vacio");
            }            
            if (oParam.Posicion == ""|| oParam.Posicion == null)
            {
                oErrorMessages.Error("Posicion", "El campo 'Posicion' no debe estar vacio");
            }
            var rangosPrecio = repositorio.Listar<RangoPrecio>();
            if (rangosPrecio.Exists(x => x.PrecioMaximo < oParam.Precio || x.PrecioMinimo > oParam.Precio))
            {
                oErrorMessages.Error("Precio", "Precio fuera de Rango");
            }
            if (oHedgeManager.Dia())
            {
                oErrorMessages.Error("","El Día de Operación ya se ha cerrado");
            }
            return oErrorMessages;
        }

        public GrabarAgenteResult GrabarAgente(AgenteCompra oAgente)
        {
            var oEntityErrors = new GrabarAgenteResult();

            this.Validar(oAgente, oEntityErrors);

            if (oEntityErrors.HayErrores)
            {
                return oEntityErrors;
            }

            if (oAgente.Id != 0)
            {
                var oFasonSave = repositorio.Obtener<AgenteCompra>(oAgente.Id);
                if (oFasonSave.Estado.EstadoContratoId > (int)EnumEstadoContrato.Con_Error)
                {
                    oEntityErrors.Error("", "El Agente de Compras no se puede modificar");
                    return oEntityErrors;
                }
                oFasonSave.Precio = oAgente.Precio;
                oFasonSave.Fecha = oAgente.Fecha;
                oFasonSave.Cantidad = oAgente.Cantidad;
                oFasonSave.EstadoId = 2;
                oFasonSave.OperadorId = oAgente.OperadorId;
                oFasonSave.ComercialId = oAgente.ComercialId;
                oFasonSave.MonedaId = oAgente.MonedaId;
                oFasonSave.MaterialId = oAgente.MaterialId;
                oFasonSave.Posicion = oAgente.Posicion;               
            }
            else
            {
                repositorio.Agregar(oAgente);
            }

            try
            {
                repositorio.GuardarCambios();
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

            if (oFasonSave.Estado.EstadoContratoId == (int)EnumEstadoContrato.Confirmado || oFasonSave.Estado.EstadoContratoId == (int)EnumEstadoContrato.Con_Error)
            {                
                try
                {
                    if(oFasonSave.Ampliaciones != null && oFasonSave.Ampliaciones != 0)
                    {
                        oFasonSave.Cantidad += oFasonSave.Ampliaciones.Value;
                        oFasonSave.Ampliaciones = null;
                    }
                    oFasonSave.Estado = repositorio.Obtener<EstadoContrato>((int)EnumEstadoContrato.Finalizado);
                    repositorio.GuardarCambios();
                }
                catch (Exception ex)
                {
                    oFasonSave.Estado = repositorio.Obtener<EstadoContrato>((int)EnumEstadoContrato.Con_Error);
                    repositorio.GuardarCambios();
                    oEntityErrors.Error("", ex.Message);
                    logger.Error(ex);
                }
            }
            else
            {
                if (oFasonSave.Estado.EstadoContratoId == (int)EnumEstadoContrato.Finalizado)
                {
                    oEntityErrors.Error("", "Agente de Compras ya se encuentra Finalizadao");
                }
                else if (oFasonSave.Estado.EstadoContratoId == (int)EnumEstadoContrato.Rechazado)
                {
                    oEntityErrors.Error("", "Agente de Compras ya ha sido Rechazado");
                }
            }
            return oEntityErrors;
        }
        public GrabarAgenteResult BorrarAgente(AgenteCompra oAgente)
        {
            var oEntityErrors = new GrabarAgenteResult();
            var oContratoSave = repositorio.Obtener<AgenteCompra>(oAgente.Id);

            if (oContratoSave.Estado.EstadoContratoId == (int)EnumEstadoContrato.Confirmado|| oContratoSave.Estado.EstadoContratoId == (int)EnumEstadoContrato.Con_Error)
            {
                oContratoSave.Estado = repositorio.Obtener<EstadoContrato>((int)EnumEstadoContrato.Rechazado);

                try
                {
                    repositorio.GuardarCambios();                    
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
            }
            else
            {
                oEntityErrors.Error("", "Agente de Compras no se puede rechazar");
            }
            return oEntityErrors;
        }
        public BasicoContrato TraerAgente(int contratoId)
        {
            var contrato = repositorio.Obtener<AgenteCompra, BasicoContrato>(x => x.Id == contratoId, x => new BasicoContrato
            {                
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
                AgenteId = x.Id
            });
            return contrato;
        }
        public GrabarAgenteResult GrabarAmpliacionAgente(AgenteCompra oAgente)
        {
            var oAgenteSave = repositorio.Obtener<AgenteCompra>(oAgente.Id);
            
            var oEntityErrors = new GrabarAgenteResult();
            if (oHedgeManager.Dia())
            {
                oEntityErrors.Error("", "El Día de Operación ya se ha cerrado");
                return oEntityErrors;
            }
            if (oAgenteSave.Estado.EstadoContratoId <= (int)EnumEstadoContrato.Con_Error)
            {
                oAgenteSave.Ampliaciones = oAgente.Ampliaciones.Value;
                try
                {
                    repositorio.GuardarCambios();
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
    }
}




