using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Business.Managers
{
    public class FijacionDePrecioContratoManager : IFijacionDePrecioContratoManager
    {
        private readonly IRepositorio repositorio;
        private IProveedorManager mobjProveedorManager;
        private ILogger logger;

        public FijacionDePrecioContratoManager(ILogger logger, IRepositorio repositorio, IProveedorManager oMSProveedorManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            mobjProveedorManager = oMSProveedorManager;
        }

        //--------------------------------------------------
        //  Metodos Publicos
        //--------------------------------------------------

        public DatosIniAbmFijacionDePrecioContrato TraerDatosIniciales()
        {
            return new DatosIniAbmFijacionDePrecioContrato
            {
                material = repositorio.Listar<Material, MaterialQry>(x =>
                                                                new MaterialQry()
                                                                {
                                                                    MaterialId = x.MaterialId,
                                                                    Descripcion = x.Descripcion
                                                                }),

                moneda = repositorio.Listar<Moneda, MonedaQry>(x =>
                                                                new MonedaQry()
                                                                {
                                                                    MonedaId = x.MonedaId,
                                                                    Descripcion = x.Descripcion
                                                                }),

                comercial = repositorio.Listar<Comercial, ComercialQry>(x =>
                                                                new ComercialQry()
                                                                {
                                                                    ComercialId = x.ComercialId,
                                                                    Comercial = x.Nombres + " " + x.Apellido
                                                                }),

                proveedor = repositorio.Listar<Proveedor, ProveedorQry>(x =>
                                                                new ProveedorQry()
                                                                {
                                                                    ProveedorId = x.ProveedorId,
                                                                    Descripcion = x.RazonSocial
                                                                })
            };
        }


        public ResultIniFijacionDePrecioContrato TraerTodoFijacionDePrecio()
        {
            return new ResultIniFijacionDePrecioContrato
            {
                FijacionDePrecioContrato = BasicoFijacionPrecioContratoTraerPorFiltro(0)
            };
        }
        public ResultIniFijacionDePrecioContrato TraerFijacionDePrecioContrato(int contratoId)
        {
            return new ResultIniFijacionDePrecioContrato
            {
                FijacionDePrecioContrato = BasicoFijacionPrecioContratoTraerPorFiltro(contratoId)
            };
        }

        private List<FijacionDePrecioContratoIni> BasicoFijacionPrecioContratoTraerPorFiltro(int contratoId)
        {
            return repositorio.Listar<FijacionDePrecioContrato, FijacionDePrecioContratoIni>(
                x => new FijacionDePrecioContratoIni
                {
                    FijacionDePrecioContratoId = x.FijacionDePrecioContratoId,
                    ContratoId = x.ContratoId,
                    Proveedor = x.Proveedor.RazonSocial,
                    Fecha = x.Fecha.ToString(),
                    Comercial = x.Comercial.Nombres,
                    Material = x.Material.Descripcion,
                    Cantidad = x.Cantidad,
                    MonedaId = x.MonedaId,
                    Ampliaciones = x.Ampliaciones,
                    Estado = x.Estado.Descripcion,
                    Observacion = x.Observacion
                }, x => contratoId == 0 || x.ContratoId == contratoId);
        }


        public FijacionDePrecioContrato TraerFijacionDePrecio(int intFijacionId)
        {
            return repositorio.Obtener<FijacionDePrecioContrato>(intFijacionId) ?? new FijacionDePrecioContrato();
        }

        public GrabarContratoResult GrabarAmpliacionFijacion(FijacionDePrecioContrato oFijacion)
        {
            var oFijacionDePrecioContratoSave = TraerFijacionDePrecio(oFijacion.FijacionDePrecioContratoId);
            var oEntityErrors = new GrabarContratoResult();

            if (oFijacionDePrecioContratoSave.Estado.EstadoContratoId <= (int)EnumEstadoContrato.Con_Error)
            {
                oFijacionDePrecioContratoSave.Ampliaciones = oFijacion.Ampliaciones.Value;
                oFijacionDePrecioContratoSave.Estado = repositorio.Obtener<EstadoContrato>((int)EnumEstadoContrato.Pendiente);
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
                oEntityErrors.Error("", "La Fijación no se puede modificar");
            }
            return oEntityErrors;
        }

        private Resultado Validar(FijacionDePrecioContrato oParam, Resultado oErrorMessages) {

            if (oParam.Proveedor == null)
            {
                oErrorMessages.Error("ProveedorId", "El campo 'Proveedor' no debe estar vacio");
            }
            if (oParam.Material == null)
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
            if (oParam.Moneda == null)
            {
                oErrorMessages.Error("MonedaId", "El campo 'Moneda' no debe estar vacio");
            }
            if (oParam.Comercial == null)
            {
                oErrorMessages.Error("ComercialId", "El campo 'Comercial' no debe estar vacio");
            }
          
            return oErrorMessages;
        }

        public GrabarFijacionResult GrabarFijacionDePrecio(FijacionDePrecioContrato oFijacionDePrecio)
        {
            var oEntityErrors = new GrabarFijacionResult();

            this.Validar(oFijacionDePrecio, oEntityErrors);

            if (oEntityErrors.HayErrores)
            {
                return oEntityErrors;
            }

            if (oFijacionDePrecio.FijacionDePrecioContratoId != 0)
            {
                var oFijacionDePrecioSave = TraerFijacionDePrecio(oFijacionDePrecio.FijacionDePrecioContratoId);
                if (oFijacionDePrecioSave.Estado.EstadoContratoId > (int)EnumEstadoContrato.Con_Error)
                {
                    oEntityErrors.Error("", "La Fijación no se puede modificar");
                    return oEntityErrors;
                }

                oFijacionDePrecioSave.Precio = oFijacionDePrecio.Precio;
                oFijacionDePrecioSave.Fecha = oFijacionDePrecio.Fecha;
                oFijacionDePrecioSave.Cantidad = oFijacionDePrecio.Cantidad;
                oFijacionDePrecioSave.Ampliaciones = oFijacionDePrecio.Ampliaciones;
                oFijacionDePrecioSave.Estado = oFijacionDePrecio.Estado;
                oFijacionDePrecioSave.Observacion = oFijacionDePrecio.Observacion;
                oFijacionDePrecioSave.ProveedorId = oFijacionDePrecio.ProveedorId;
                oFijacionDePrecioSave.ComercialId = oFijacionDePrecio.ComercialId;
                oFijacionDePrecioSave.ContratoId = oFijacionDePrecio.ContratoId;
                oFijacionDePrecioSave.MonedaId = oFijacionDePrecio.MonedaId;
                oFijacionDePrecioSave.MaterialId = oFijacionDePrecio.MaterialId;
            }
            else
            {
                repositorio.Agregar(oFijacionDePrecio);
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

        public GrabarFijacionResult ConfirmarFijacion(FijacionDePrecioContrato oFijacionDePrecio)
        {
            var oEntityErrors = new GrabarFijacionResult();
            var oFijacionDePrecioSave = TraerFijacionDePrecio(oFijacionDePrecio.FijacionDePrecioContratoId);

            if (oFijacionDePrecioSave.Estado.EstadoContratoId == (int)EnumEstadoContrato.Pendiente || oFijacionDePrecioSave.Estado.EstadoContratoId == (int)EnumEstadoContrato.Oferta)
            {
                oFijacionDePrecioSave.Cantidad += oFijacionDePrecioSave.Ampliaciones.Value;
                oFijacionDePrecioSave.Ampliaciones = 0;

                oFijacionDePrecioSave.Estado = repositorio.Obtener<EstadoContrato>((int)EnumEstadoContrato.Confirmado);

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
                oEntityErrors.Error("", "La Fijación no se puede confirmar");
            }

            return oEntityErrors;
        }

        public GrabarFijacionResult FinalizarFijacion(FijacionDePrecioContrato oFijacionDePrecio, string idActiveDirectory)
        {
            var oEntityErrors = new GrabarFijacionResult();
            var oFijacionDePrecioSave = TraerFijacionDePrecio(oFijacionDePrecio.FijacionDePrecioContratoId);

            if (oFijacionDePrecioSave.Estado.EstadoContratoId == (int)EnumEstadoContrato.Confirmado || oFijacionDePrecioSave.Estado.EstadoContratoId == (int)EnumEstadoContrato.Con_Error)
            {
                oFijacionDePrecioSave.Estado = repositorio.Obtener<EstadoContrato>((int)EnumEstadoContrato.Finalizado);

                //Envio de mail
                mobjProveedorManager.EnviarEmailFijacion(oFijacionDePrecioSave, idActiveDirectory);

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
                if (oFijacionDePrecioSave.Estado.EstadoContratoId == (int)EnumEstadoContrato.Confirmado)
                {
                    oEntityErrors.Error("", "La Fijación ya se encuentra Finalizada");
                }
                else if (oFijacionDePrecioSave.Estado.EstadoContratoId == (int)EnumEstadoContrato.Rechazado)
                {
                    oEntityErrors.Error("", "La Fijación ya ha sido Rechazada");
                }
                else if (oFijacionDePrecioSave.Estado.EstadoContratoId == (int)EnumEstadoContrato.Pendiente || oFijacionDePrecioSave.Estado.EstadoContratoId == (int)EnumEstadoContrato.Oferta)
                {
                    oEntityErrors.Error("", "La Fijación debe ser Confirmada");
                }
            }
            return oEntityErrors;
        }

        public Resultado EliminarFijacionDePrecio(int intFijacionId)
        {
            var oEntityErrors = new Resultado();

            repositorio.Remover<FijacionDePrecioContrato>(intFijacionId);
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

        public GrabarContratoResult BorrarFijacion(FijacionDePrecioContrato oContrato)
        {
            var oEntityErrors = new GrabarContratoResult();
            var oContratoSave = TraerFijacionDePrecio(oContrato.FijacionDePrecioContratoId);

            if (oContratoSave.Estado.EstadoContratoId < (int)EnumEstadoContrato.Finalizado)
            {
                oContratoSave.Estado = repositorio.Obtener<EstadoContrato>((int)EnumEstadoContrato.Rechazado);

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
                oEntityErrors.Error("", "La Fijación no se puede rechazar");
            }
            return oEntityErrors;
        }
    }
}




