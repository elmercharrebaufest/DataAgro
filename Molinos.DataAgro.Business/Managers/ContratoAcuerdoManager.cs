using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Validations;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Business
{

    public class ContratoAcuerdoManager : IContratoAcuerdoManager
    {
        private ILogger logger;
        private readonly IRepositorio repositorio;

        public ContratoAcuerdoManager(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public Resultado EliminarContratoAcuerdo(int id)
        {

            var oEntityErrors = new Resultado();

            repositorio.Remover<ContratoAcuerdo>(id);

            logger.Debug("Eliminando el ContratoAcuerdo:" + id);
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

        public Resultado GrabarContratoAcuerdo(ContratoAcuerdo oContratoAcuerdo, EnumPerfil perfil)
        {
            var oEntityErrors = new Resultado();

            EntityValid.ValidateAll(oContratoAcuerdo, oEntityErrors);

            if ((oContratoAcuerdo.ProveedorId == null || oContratoAcuerdo.ProveedorId <= 0) && (oContratoAcuerdo.CorredorId  == null || oContratoAcuerdo.CorredorId <= 0))
            {
                oEntityErrors.Error("", "Debe ingresar al menos proveedor o corredor");
            }

            if (oContratoAcuerdo.Precio < 0)
            {
                oEntityErrors.Error("", "El precio debe ser mayor o igual a 0");
            }
            if (oContratoAcuerdo.Cantidad <= 0)
            {
                oEntityErrors.Error("", "La Cantidad debe ser mayor o igual a 0");
            }
            if (oContratoAcuerdo.ComercialCreadorId == 0 )
            {
                oEntityErrors.Error("", "El campo Comercial es obligatorio");
            }
            if ( oContratoAcuerdo.DestinoId == 0 )
            {
                oEntityErrors.Error("", "El campo Destino es obligatorio");
            }
            if (oContratoAcuerdo.MaterialId == 0)
            {
                oEntityErrors.Error("", "El campo Material es obligatorio");
            }
            if (oContratoAcuerdo.MonedaId == null)
            {
                oEntityErrors.Error("", "El campo Moneda es obligatorio");
            }
            if (oContratoAcuerdo.FechaHasta.ToString() == "1/1/0001 12:00:00 AM")
            {
                oEntityErrors.Error("", "La fecha es obligatoria");
            }
            if (oEntityErrors.HayErrores)
            {
                return oEntityErrors;
            }

            oContratoAcuerdo.CorredorId = (oContratoAcuerdo.CorredorId == -1) ? null : oContratoAcuerdo.CorredorId;
            oContratoAcuerdo.ProveedorId = (oContratoAcuerdo.ProveedorId == -1) ? null : oContratoAcuerdo.ProveedorId;

            if (oContratoAcuerdo.Id == 0)
            {
                if (perfil == EnumPerfil.Mesa)
                {
                    oContratoAcuerdo.EstadoId = 2;
                }
                else
                {
                    oContratoAcuerdo.EstadoId = 1;
                }
                oContratoAcuerdo.Fecha = DateTime.Now;
                repositorio.Agregar(oContratoAcuerdo);
            }
            else
            {
                var objContratoAcuerdo = repositorio.Obtener<ContratoAcuerdo>(oContratoAcuerdo.Id);
                objContratoAcuerdo.MaterialId = oContratoAcuerdo.MaterialId;
                objContratoAcuerdo.DestinoId = oContratoAcuerdo.DestinoId;
                objContratoAcuerdo.FechaDesde = oContratoAcuerdo.FechaDesde;
                objContratoAcuerdo.FechaHasta = oContratoAcuerdo.FechaHasta;
                objContratoAcuerdo.ComercialCreadorId = oContratoAcuerdo.ComercialCreadorId;
                objContratoAcuerdo.Precio = oContratoAcuerdo.Precio;
                objContratoAcuerdo.Cantidad = oContratoAcuerdo.Cantidad;
                objContratoAcuerdo.ProveedorId = oContratoAcuerdo.ProveedorId;
                objContratoAcuerdo.CorredorId = oContratoAcuerdo.CorredorId;
                objContratoAcuerdo.MonedaId = oContratoAcuerdo.MonedaId;
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

            logger.Debug("Guardando el Contrato Acuerdo");

            return oEntityErrors;
        }

        public ContratoAcuerdoDto TraerContratoAcuerdo(int id)
        {
            var a = repositorio.Obtener<ContratoAcuerdo, ContratoAcuerdoDto>(x => x.Id == id,
                    x => new ContratoAcuerdoDto
                    {
                        Id = x.Id,
                        Cantidad = x.Cantidad,
                        Precio = x.Precio,
                        ComercialId = x.ComercialCreadorId,
                        MaterialId = x.MaterialId,
                        Proveedor = (x.ProveedorId != null && x.ProveedorId > 0)? x.Proveedor.RazonSocial + " (" + x.Proveedor.CUIT + ")": "",
                        ProveedorId = x.ProveedorId ?? 0,
                        Corredor = (x.CorredorId != null && x.CorredorId > 0) ? x.Corredor.RazonSocial + " (" + x.Corredor.CUIT + ")": "",
                        CorredorId = x.CorredorId ?? 0,
                        DestinoId = x.DestinoId,
                        FechaHasta = x.FechaHasta,
                        FechaDesde = x.FechaDesde,
                        EstadoId = x.EstadoId,
                        Estado = x.Estado.Descripcion,
                        MonedaId = x.MonedaId,
                        Moneda = x.Moneda.Descripcion
                    })
                        ?? new ContratoAcuerdoDto();
            a.FechaModificacionDesde = a.FechaDesde.ToShortDateString();
            a.FechaModificacion = a.FechaHasta.ToShortDateString();
            return a;
        }

        public DatosIniAbmContratoAcuerdo TraerDatosIniciales()
        {
            throw new NotImplementedException();
        }
        public DatosIniComboContratoAcuerdo TraerDatosCombo(int perfilId)
        {
            var datosCombo = new DatosIniComboContratoAcuerdo();

            datosCombo.material = repositorio.Listar<Material, MaterialQry>(x => new MaterialQry() { MaterialId = x.MaterialId, Descripcion = x.Descripcion });
            datosCombo.destino = repositorio.Listar<Centro, CentroQry>(x => new CentroQry() { Id = x.Id, Descripcion = x.Descripcion });
            datosCombo.comercial = repositorio.Listar<Comercial, ComercialQry>(x => new ComercialQry() { ComercialId = x.ComercialId, Comercial = x.Nombres + " " + x.Apellido });
            datosCombo.moneda = repositorio.Listar<Moneda, MonedaQry>(x => new MonedaQry() { MonedaId = x.MonedaId, Descripcion = x.Descripcion });

            return datosCombo;
        }

        public ResultIniContratoAcuerdo TraerTodoContratoAcuerdo()
        {

            var contratos = new ResultIniContratoAcuerdo
            {
                ContratoAcuerdo = repositorio.Listar<ContratoAcuerdo, ContratoAcuerdoIni>(x => new ContratoAcuerdoIni()
                {
                    Cantidad = x.Cantidad,
                    Comercial = x.Comercial.Nombres + " " + x.Comercial.Apellido,
                    Destino = x.Destino.Descripcion,
                    Fecha = DbFunctions.TruncateTime(x.Fecha),
                    FechaDesde = x.FechaDesde,
                    FechaHasta = x.FechaHasta,
                    Id = x.Id,
                    Precio = x.Precio,
                    Material = x.Material.Descripcion,
                    Proveedor = x.Proveedor.RazonSocial,
                    Corredor = x.Corredor.RazonSocial,
                    EstadoId = x.EstadoId,
                    Estado = x.Estado.Descripcion,
                    MonedaId = x.MonedaId,
                    Moneda = x.Moneda.Descripcion
                })
            };

            contratos.ContratoAcuerdo.ForEach(x => x.PorcentajeCargado = Math.Round(((decimal)repositorio.Listar<Contrato>(d => d.ContratoAcuerdoId == x.Id).Sum(d => d.Cantidad) / x.Cantidad), 2) * 100);
            return contratos;
        }

        public ContratoAcuerdoDto ObtenerContratoAcuerdoParaAsociar(DateTime fecha, int destinoId, int materialId, int proveedorId)
        {
            var fechaSinTiempo = fecha.Date;
            return repositorio.Obtener<ContratoAcuerdo, ContratoAcuerdoDto>(x => DbFunctions.TruncateTime(x.Fecha) == fechaSinTiempo
                                && x.DestinoId == destinoId
                                && x.MaterialId == materialId
                                && (x.ProveedorId == proveedorId)
                                && x.EstadoId == (int)EnumEstadoContrato.Confirmado,
                                x => new ContratoAcuerdoDto()
                                {
                                    Cantidad = x.Cantidad,
                                    Comercial = x.Comercial.Nombres,
                                    Destino = x.Destino.Descripcion,
                                    FechaHasta = x.FechaHasta,
                                    Id = x.Id,
                                    Precio = x.Precio,
                                    Material = x.Material.Descripcion,
                                    Proveedor = x.Proveedor.RazonSocial,
                                    Corredor = x.Corredor.RazonSocial,
                                    EstadoId = x.EstadoId,
                                    Estado = x.Estado.Descripcion
                                }
                                );
        }

        public Resultado ConfirmarContratoAcuerdo(int id)
        {

            var oEntityErrors = new Resultado();

            var contrato = repositorio.Obtener<ContratoAcuerdo>(id);
            contrato.EstadoId = (int)EnumEstadoContrato.Confirmado;
            logger.Debug("Confirmando el ContratoAcuerdo:" + id);
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
    }
}
