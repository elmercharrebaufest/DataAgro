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
    public class FasonManager : IFasonManager
    {
        private readonly IRepositorio repositorio;
        private IProveedorManager mobjProveedorManager;
        private ILogger logger;

        public FasonManager(ILogger logger, IRepositorio repositorio, IProveedorManager oMSProveedorManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            mobjProveedorManager = oMSProveedorManager;
        }

        //--------------------------------------------------
        //  Metodos Publicos
        //--------------------------------------------------

        private Resultado Validar(Fason oParam, Resultado oErrorMessages) {

            if (oParam.FasoneroId == 0)
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
            if (oParam.CampanaId == 0)
            {
                oErrorMessages.Error("CampanaId", "El campo 'Campaña' no debe estar vacio");
            }
            if (oParam.Posicion == ""|| oParam.Posicion == null)
            {
                oErrorMessages.Error("Posicion", "El campo 'Posicion' no debe estar vacio");
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
            if (rangosPrecio.Exists(x => x.PrecioMaximo < oParam.Precio || x.PrecioMinimo > oParam.Precio))
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

            if (oFason.Id != 0)
            {
                var oFasonSave = repositorio.Obtener<Fason>(oFason.Id);
                if (oFasonSave.Estado.EstadoContratoId > (int)EnumEstadoContrato.Con_Error)
                {
                    oEntityErrors.Error("", "El Negocio Fasón no se puede modificar");
                    return oEntityErrors;
                }
                oFasonSave.Precio = oFason.Precio;
                oFasonSave.Fecha = oFason.Fecha;
                oFasonSave.Cantidad = oFason.Cantidad;
                oFasonSave.EstadoId = 2;
                oFasonSave.FasoneroId = oFason.FasoneroId;
                oFasonSave.ComercialId = oFason.ComercialId;
                oFasonSave.MonedaId = oFason.MonedaId;
                oFasonSave.MaterialId = oFason.MaterialId;
                oFasonSave.CampanaId = oFason.CampanaId;
                oFasonSave.Posicion = oFason.Posicion;
                oFasonSave.TipoFasonId = oFason.TipoFasonId;
                oFasonSave.FechaDesde = oFason.FechaDesde;
                oFasonSave.FechaHasta = oFason.FechaHasta;
                oFasonSave.Especial = oFason.Especial;
            }
            else
            {
                repositorio.Agregar(oFason);
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
        public GrabarFasonResult FinalizarFason(int fijacionDePrecioContratoId)
        {
            var oEntityErrors = new GrabarFasonResult();
            var oFasonSave = repositorio.Obtener<Fason>(fijacionDePrecioContratoId);

            if (oFasonSave.Estado.EstadoContratoId == (int)EnumEstadoContrato.Confirmado || oFasonSave.Estado.EstadoContratoId == (int)EnumEstadoContrato.Con_Error )
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
            var oContratoSave = repositorio.Obtener<Fason>(oFason.Id);

            if (oContratoSave.Estado.EstadoContratoId == (int)EnumEstadoContrato.Confirmado || oContratoSave.Estado.EstadoContratoId == (int)EnumEstadoContrato.Con_Error || oContratoSave.Estado.EstadoContratoId == (int)EnumEstadoContrato.Finalizado)
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
                oEntityErrors.Error("", "Fasón no se puede rechazar");
            }
            return oEntityErrors;
        }
        public BasicoContrato TraerFason(int contratoId)
        {
            var contrato = repositorio.Obtener<Fason, BasicoContrato>(x => x.Id == contratoId, x => new BasicoContrato
            {
                
                ProveedorId = x.FasoneroId,
                Proveedor = x.Fasonero == null ? "" : x.Fasonero.RazonSocial + " " + "(" + x.Fasonero.CUIT + ")",
                ComercialId = x.ComercialId,
                FechaFormateado = SqlFunctions.DateName("day", x.Fecha).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)x.Fecha.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", x.Fecha),
                TipoNegocioId = 4,
                MaterialId = x.MaterialId,
                Cantidad = x.Cantidad,
                Precio = x.Precio,
                MonedaId = x.MonedaId,
                CampanaId = x.CampanaId,
                Estado = x.EstadoId,
                Posicion = x.Posicion,
                TipoFason = x.TipoFason.Descripcion,
                TipoFasonId = x.TipoFasonId,
                FasonId = x.Id,
                TrigoEspecial = x.Especial,
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
                oEntityErrors.Error("", "El Contrato Fasón no se puede modificar");
            }
            return oEntityErrors;
        }
    }
}




