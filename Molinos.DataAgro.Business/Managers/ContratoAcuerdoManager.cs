using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Entities.Validations;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;

namespace Molinos.DataAgro.Business
{

    public class ContratoAcuerdoManager : IContratoAcuerdoManager
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly IDiasHabilesAgent diasHabilesAgent;

        public ContratoAcuerdoManager(ILogger logger, IRepositorio repositorio, IDiasHabilesAgent diasHabilesAgent)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.diasHabilesAgent = diasHabilesAgent;
        }

        public GrabarAcuerdoResult BorrarAcuerdo(ContratoAcuerdo oAcuerdo)
        {
            var oEntityErrors = new GrabarAcuerdoResult();
            if (!repositorio.Existe<Contrato>(x=>x.ContratoAcuerdoId == oAcuerdo.Id))
            {
                var oContratoSave = repositorio.Obtener<ContratoAcuerdo>(oAcuerdo.Id);

                if (oContratoSave.EstadoId == (int)EnumEstadoContrato.Confirmado ||
                    oContratoSave.EstadoId == (int)EnumEstadoContrato.Con_Error || 
                    oContratoSave.EstadoId == (int)EnumEstadoContrato.Pendiente || 
                    oContratoSave.EstadoId == (int)EnumEstadoContrato.Reconfirmar || 
                    oContratoSave.EstadoId == (int)EnumEstadoContrato.Finalizado)
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
                    oEntityErrors.Error("", "El Contrato Acuerdo no se puede rechazar");
                }
            }
            else
            {
                oEntityErrors.Error("", "El Contrato Acuerdo ya se ha utilizado y no se puede rechazar");
            }
            return oEntityErrors;
        }

        public GrabarAcuerdoResult GrabarAcuerdo(ContratoAcuerdo oContratoAcuerdo)
        {
            var oEntityErrors = new GrabarAcuerdoResult();

            EntityValid.ValidateAll(oContratoAcuerdo, oEntityErrors);

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
            if (oContratoAcuerdo.FechaHasta == new DateTime())
            {
                oEntityErrors.Error("", "La fecha es obligatoria");
            }
            if (oEntityErrors.HayErrores)
            {

                return oEntityErrors;
            }

            oContratoAcuerdo.CorredorId = (oContratoAcuerdo.CorredorId == -1) ? null : oContratoAcuerdo.CorredorId;
            oContratoAcuerdo.ProveedorId = (oContratoAcuerdo.ProveedorId == -1) ? null : oContratoAcuerdo.ProveedorId;
            var estado = PermisosHelper.Is(PermisosDataAgro.NegociosConfirmados) ? 2 : 1;

            if (oContratoAcuerdo.Id == 0)
            {
                oContratoAcuerdo.Fecha = DateTime.Now;
                oContratoAcuerdo.EstadoId = estado;
                repositorio.Agregar(oContratoAcuerdo);
            }
            else
            {
                var objContratoAcuerdo = repositorio.Obtener<ContratoAcuerdo>(oContratoAcuerdo.Id);

                objContratoAcuerdo.MaterialId = oContratoAcuerdo.MaterialId;
                objContratoAcuerdo.DestinoId = oContratoAcuerdo.DestinoId;
                objContratoAcuerdo.FechaDesde = oContratoAcuerdo.FechaDesde;
                objContratoAcuerdo.FechaHasta = oContratoAcuerdo.FechaHasta;
                objContratoAcuerdo.EstadoId = estado == 1? 7:2;
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

        public BasicoContrato TraerAcuerdo(int id)
        {
            var contrato = repositorio.Obtener<ContratoAcuerdo, BasicoContrato>(x => x.Id == id, x => new BasicoContrato
            {                
                Id = x.Id,
                Cantidad = x.Cantidad,
                Precio = x.Precio,
                ComercialId = x.ComercialCreadorId,
                MaterialId = x.MaterialId,
                FechaFormateado = SqlFunctions.DateName("day", x.Fecha).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)x.Fecha.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", x.Fecha),
                FechaDesdeFormateado = SqlFunctions.DateName("day", x.FechaDesde).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)x.FechaDesde.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", x.FechaDesde),
                FechaHastaFormateado = SqlFunctions.DateName("day", x.FechaHasta).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)x.FechaHasta.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", x.FechaHasta),
                Proveedor = (x.ProveedorId != null && x.ProveedorId > 0) ? x.Proveedor.RazonSocial + " (" + x.Proveedor.CUIT + ")" : "",
                ProveedorId = x.ProveedorId ?? 0,
                Corredor = (x.CorredorId != null && x.CorredorId > 0) ? x.Corredor.RazonSocial + " (" + x.Corredor.CUIT + ")" : "",
                CorredorId = x.CorredorId ?? 0,
                DestinoId = x.DestinoId,
                TipoNegocioId = 6,
                FechaHasta = x.FechaHasta,
                FechaDesde = x.FechaDesde,
                Estado = x.EstadoId,
                MonedaId = x.MonedaId,
                Moneda = x.Moneda.Descripcion
            });
            return contrato;            
        }

        public DatosIniAbmContratoAcuerdo TraerDatosIniciales()
        {
            throw new NotImplementedException();
        }
        public DatosIniComboContratoAcuerdo TraerDatosCombo()
        {
            var datosCombo = new DatosIniComboContratoAcuerdo
            {
                material = repositorio.Listar<Material, MaterialQry>(x => new MaterialQry() { MaterialId = x.MaterialId, Descripcion = x.Descripcion }),
                destino = repositorio.Listar<Centro, CentroQry>(x => new CentroQry() { Id = x.Id, Descripcion = x.Descripcion }),
                comercial = repositorio.Listar<Comercial, ComercialQry>(x => new ComercialQry() { ComercialId = x.ComercialId, Comercial = x.Nombres + " " + x.Apellido }),
                moneda = repositorio.Listar<Moneda, MonedaQry>(x => new MonedaQry() { MonedaId = x.MonedaId, Descripcion = x.Descripcion })
            };

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
                                });
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

        public GrabarAcuerdoResult FinalizarAcuerdo(int acuerdoId)
        {
            var oEntityErrors = new GrabarAcuerdoResult();
            var oAcuerdoSave = repositorio.Obtener<ContratoAcuerdo>(acuerdoId);

            if (oAcuerdoSave.EstadoId == (int)EnumEstadoContrato.Confirmado || oAcuerdoSave.EstadoId == (int)EnumEstadoContrato.Con_Error)
            {
                try
                {
                    oAcuerdoSave.Estado = repositorio.Obtener<EstadoContrato>((int)EnumEstadoContrato.Finalizado);
                    repositorio.GuardarCambios();
                }
                catch (Exception ex)
                {
                    oAcuerdoSave.Estado = repositorio.Obtener<EstadoContrato>((int)EnumEstadoContrato.Con_Error);
                    repositorio.GuardarCambios();
                    oEntityErrors.Error("", ex.Message);
                    logger.Error(ex);
                }
            }
            else
            {
                if (oAcuerdoSave.EstadoId == (int)EnumEstadoContrato.Finalizado)
                {
                    oEntityErrors.Error("", "El Acuerdo ya se encuentra Finalizado");
                }
                else if (oAcuerdoSave.EstadoId == (int)EnumEstadoContrato.Rechazado)
                {
                    oEntityErrors.Error("", "El Acuerdo ya ha sido Rechazado");
                }
            }
            return oEntityErrors;
        }

        public void AnularAcuerdos()
        {
            var dia = diasHabilesAgent.UltimoDiaHabil();
            var listaAcuerdo = repositorio.Listar<ContratoAcuerdo>(x => x.Fecha < dia && x.EstadoId == 2); 
            
            foreach(var acuerdo in listaAcuerdo)
            {
                var cantidad = repositorio.Listar<Contrato,double>(x => x.Cantidad, x => x.ContratoAcuerdoId == acuerdo.Id).Sum();
                acuerdo.Cantidad = (int)cantidad;
                acuerdo.EstadoId = 5;
            }

           repositorio.GuardarCambios();
        }
    }
}
