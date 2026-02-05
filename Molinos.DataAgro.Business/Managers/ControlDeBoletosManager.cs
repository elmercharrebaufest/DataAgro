using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Dto.ControlDeBoletos;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Interfaces.Agent;
using Molinos.DataAgro.Interfaces.Managers;
using Molinos.DataAgro.Repository;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace Molinos.DataAgro.Business.Managers
{
    public class ControlDeBoletosManager : IControlDeBoletosManager
    {
        private readonly IRepositorio repositorio;
        private readonly ILogger logger;
        private readonly IMailManager mailManager;
        private readonly IModificacionContratoControlBoletoAgent modificacionContratoControlBoletoAgent;
        private readonly ISeguimientoControlBoletoAgent seguimientoControlBoletoAgent;
        public ControlDeBoletosManager(ILogger logger, IRepositorio repositorio, IMailManager mailManager, IModificacionContratoControlBoletoAgent modificacionContratoControlBoletoAgent, ISeguimientoControlBoletoAgent seguimientoControlBoletoAgent)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.mailManager = mailManager;
            this.modificacionContratoControlBoletoAgent = modificacionContratoControlBoletoAgent;
            this.seguimientoControlBoletoAgent = seguimientoControlBoletoAgent;
        }

        public Resultado AsociarConfirma(int negocioId)
        {
            throw new NotImplementedException();
        }

        #region Metodo para cargar combos
        public List<BolsaCompraNetQry> GetBolsaCompraNet()
        {
            var qry = new CombosQueries(logger, repositorio);
            return qry.GetBolsaCompraNet();
        }

        public List<ComercialCombo> GetComercial()
        {
            var qry = new CombosQueries(logger, repositorio);
            return qry.GetAbmComercialCombo();
        }

        public List<MaterialCombo> GetMaterial()
        {
            var qry = new CombosQueries(logger, repositorio);
            return qry.GetAbmMaterialCombo();
        }

        public List<ProveedorCombo> GetProveedorPorComercial(List<int> equipo)
        {
            var qry = new CombosQueries(logger, repositorio);
            return qry.GetProveedorPorComercialCombo(equipo);
        }

        public List<Provincia> GetProvincias()
        {
            var qry = new CombosQueries(logger, repositorio);
            return qry.GetProvinciaCombo();
        }

        public List<CampaniaCombo> GetCosechas()
        {
            var qry = new CombosQueries(logger, repositorio);
            return qry.GetAbmCampaniaCombo();
        }

        public List<ClasificacionCompraNet> GetClasificaciones()
        {
            var qry = new CombosQueries(logger, repositorio);
            return qry.GetClasificacionCombo();
        }

        public List<Localidad> GetProcedencias(int provinciaId)
        {
            return this.repositorio.Listar<Localidad>(x => x.ProvinciaId == provinciaId);
        }
        #endregion

        public Resultado RegistroContratoPendienteDeControl(int negocioId, int? altaIdLoteConfirma = null)
        {
            var oResultado = new Resultado();
            var negocio = repositorio.Obtener<Negocio>(negocioId);
            if (negocio != null)
            {
                var existe = repositorio.Obtener<ControlDeBoletos>(x => x.NegocioId == negocioId);
                if (existe == null)
                {
                    var controlDeBoletos = new ControlDeBoletos()
                    {
                        NegocioId = negocioId,
                        FechaCreacion = DateTime.Now,
                        EsConfirma = negocio.BoletoVentaId == (int)EnumBoletoCompraNet.CONFIRMA,
                        AltaIdLoteConfirma = (negocio.BoletoVentaId == (int)EnumBoletoCompraNet.CONFIRMA) ? altaIdLoteConfirma : (int?)null,
                        ControlDeBoletosEstadoId = (int)EnumControlDeBoletosEstado.PENDIENTE_CONTROL,
                        EstadoConfirmaId = (negocio.BoletoVentaId == (int)EnumBoletoCompraNet.CONFIRMA) ? (int)EnumEstadoConfirma.PENDIENTE : (int?)null,
                        ControlIniciado = false,
                        ControlFinalizado = false,
                        CertificacionCompletada = false,
                        RegistroDatosOblea = false
                    };
                    repositorio.Agregar(controlDeBoletos);
                    repositorio.GuardarCambios();
                }
            }
            return oResultado;
        }

        public List<ControlDeBoletosConsultaDto> GetControlBoletosPendientes(ControlDeBoletoFiltroBusquedaDto filtros)
        {
            var query = repositorio.Listar<ControlDeBoletos>()
                .Where(x => x.ControlDeBoletosEstadoId == (int)EnumControlDeBoletosEstado.PENDIENTE_CONTROL);

            if (!string.IsNullOrEmpty(filtros.ContratoSAPDesde))
                query = query.Where(x => string.Compare(x.Negocio.ContratoSAP, filtros.ContratoSAPDesde) >= 0);

            if (!string.IsNullOrEmpty(filtros.ContratoSAPHasta))
                query = query.Where(x => string.Compare(x.Negocio.ContratoSAP, filtros.ContratoSAPHasta) <= 0);

            if (filtros.MaterialId.HasValue)
                query = query.Where(x => x.Negocio.MaterialId == filtros.MaterialId.Value);

            if (filtros.EstadoControlId.HasValue)
                query = query.Where(x => x.ControlDeBoletosEstadoId == filtros.EstadoControlId.Value);

            if (filtros.EsConfirma)
                query = query.Where(x => x.EsConfirma == true);

            if (filtros.FechaCargaDesde.HasValue)
                query = query.Where(x => x.FechaCreacion >= filtros.FechaCargaDesde.Value);

            if (filtros.FechaCargaHasta.HasValue)
                query = query.Where(x => x.FechaCreacion <= filtros.FechaCargaHasta.Value);

            if (filtros.Proveedor.HasValue)
                query = query.Where(x => x.Negocio.ProveedorId == filtros.Proveedor.Value);

            if (filtros.BolsaId.HasValue)
                query = query.Where(x => x.Negocio.BolsaId == filtros.BolsaId.Value);

            if (filtros.ComercialId.HasValue)
                query = query.Where(x => x.Negocio.ComercialId == filtros.ComercialId.Value);

            // 🚀 PROYECCIÓN FINAL
            return query.Select(controlBoleto => new ControlDeBoletosConsultaDto
            {
                Id = controlBoleto.Id,
                NegocioId = controlBoleto.NegocioId,
                ControlDeBoletosEstadoId = controlBoleto.ControlDeBoletosEstadoId,
                ControlDeBoletosEstado = controlBoleto.ControlDeBoletosEstado.Descripcion,
                EsConfirma = controlBoleto.EsConfirma,
                AltaIdLoteConfirma = controlBoleto.AltaIdLoteConfirma,
                IdentificadorConfirma = controlBoleto.IdentificadorConfirma,
                FechaCreacion = controlBoleto.FechaCreacion,
                FechaModificacion = controlBoleto.FechaModificacion,
                EstadoConfirmaId = controlBoleto.EstadoConfirmaId,
                EstadoConfirma = controlBoleto.EstadoConfirmaId.HasValue ? repositorio.Obtener<EstadoConfirma>(controlBoleto.EstadoConfirmaId.Value)?.Descripcion : null,
                ControlIniciado = controlBoleto.ControlIniciado,
                ControlFinalizado = controlBoleto.ControlFinalizado,
                CertificacionCompletada = controlBoleto.CertificacionCompletada,
                RegistroDatosOblea = controlBoleto.RegistroDatosOblea,
                FechaControlIniciado = controlBoleto.FechaControlIniciado,
                FechaControlFinalizado = controlBoleto.FechaControlFinalizado,
                FechaCertificacionCompletada = controlBoleto.FechaCertificacionCompletada,
                FechaRegistroDatosOblea = controlBoleto.FechaRegistroDatosOblea,
                MaterialId = controlBoleto.Negocio.MaterialId,
                Material = controlBoleto.Negocio.Material.Descripcion,
                BolsaCompraNetId = controlBoleto.Negocio.BolsaId,
                BolsaCompraNet = controlBoleto.Negocio.Bolsa.Descripcion,
                ComercialId = controlBoleto.Negocio.ComercialId,
                Comercial = controlBoleto.Negocio.Comercial.Nombres + " " + controlBoleto.Negocio.Comercial.Apellido,
                ContratoSAP = controlBoleto.Negocio.ContratoSAP,
                ProveedorId = controlBoleto.Negocio.ProveedorId,
                Proveedor = controlBoleto.Negocio.Proveedor.RazonSocial
            }).ToList();
        }

        public Resultado ModificacionContrato(ControlDeBoletosModificacionContratoDto controlDeBoletosModificacion)
        {
            var oResultado = new Resultado();
            try
            {
                var mensaje = modificacionContratoControlBoletoAgent.ModificarContrato(controlDeBoletosModificacion.Clasificacion,
                                                                                       controlDeBoletosModificacion.Contrato, 
                                                                                       controlDeBoletosModificacion.Cosecha, 
                                                                                       controlDeBoletosModificacion.Fecha, 
                                                                                       controlDeBoletosModificacion.Hora, 
                                                                                       controlDeBoletosModificacion.Procedencia, 
                                                                                       controlDeBoletosModificacion.Provincia, 
                                                                                       controlDeBoletosModificacion.Usuario);
                return oResultado;
            }
            catch(Exception ex)
            {
                oResultado.Errores.Add(new ErrorMessage()
                {
                    Message = ex.Message,
                });
                logger.Error(ex.Message);
                return oResultado;
            }
        }

        public Resultado RegistrarAcciones(List<int> ControlDeBoletoIds, EnumControlDeBoletosAcciones accion)
        {
            var oResultado = new Resultado();

            try
            {
                var boletos = repositorio.Listar<ControlDeBoletos>(x=> ControlDeBoletoIds.Contains(x.Id));

                if (boletos.Count == 0)
                {
                    oResultado.Errores.Add(new ErrorMessage()
                    {
                        Message = "No se encontró el control de boletos.",
                    });
                    return oResultado;
                }

                var ahora = DateTime.Now;

                foreach(var boleto in boletos)
                {
                    switch (accion)
                    {
                        case EnumControlDeBoletosAcciones.ControlIniciado:
                            boleto.ControlIniciado = true;
                            boleto.FechaControlIniciado = ahora;
                            break;

                        case EnumControlDeBoletosAcciones.RegistroDatosOblea:
                            boleto.RegistroDatosOblea = true;
                            boleto.FechaRegistroDatosOblea = ahora;
                            break;

                        case EnumControlDeBoletosAcciones.CertificacionCompletada:
                            boleto.CertificacionCompletada = true;
                            boleto.FechaCertificacionCompletada = ahora;
                            break;

                        case EnumControlDeBoletosAcciones.ControlFinalizado:
                            boleto.ControlFinalizado = true;
                            boleto.FechaControlFinalizado = ahora;
                            break;

                        default:
                            oResultado.Errores.Add(new ErrorMessage()
                            {
                                Message = "Acción no válida.",
                            });
                            return oResultado;
                    }
                    repositorio.GuardarCambios();
                }
            }
            catch (Exception ex)
            {
                oResultado.Errores.Add(new ErrorMessage()
                {
                    Message = ex.Message,
                });
                return oResultado;
            }
            return oResultado;
        }

        public Resultado SeguimientoBoleto(SeguimientoControlDeBoletosDto seguimientoControlDeBoletos)
        {
            var oResultado = new Resultado();
            try
            {
                var mensaje = seguimientoControlBoletoAgent.SeguimientoBoletos(seguimientoControlDeBoletos);
                return oResultado;
            }
            catch (Exception ex)
            {
                oResultado.Errores.Add(new ErrorMessage()
                {
                    Message = ex.Message,
                });
                logger.Error(ex.Message);
                return oResultado;
            }
        }


    }
}
