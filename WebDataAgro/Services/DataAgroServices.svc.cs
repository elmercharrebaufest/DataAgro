using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Web.Mvc;
using WebDataAgro.Atributos;

namespace WebDataAgro.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "DataAgroServices" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select DataAgroServices.svc or DataAgroServices.svc.cs at the Solution Explorer and start debugging.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class DataAgroServices : IDataAgroServices
    {
        private readonly ILogger logger;
        private readonly IRiesgoComercialManager riesgoComercial;
        private readonly ICampaniaActualManager campanaActual;
        private readonly ICampaniaMaterialManager campaniaMaterial;
        private readonly IInformeComercialManager informeComercial;
        private readonly IContratoManager contratoManager;
        private readonly IRepositorio repositorio;

        public DataAgroServices(ILogger logger,
            IRiesgoComercialManager riesgoComercial,
            ICampaniaActualManager campanaActual,
            ICampaniaMaterialManager campaniaMaterial,
            IInformeComercialManager informeComercial,
            IContratoManager contratoManager,
            IRepositorio repositorio)
        {
            this.logger = logger;
            this.riesgoComercial = riesgoComercial;
            this.campanaActual = campanaActual;
            this.campaniaMaterial = campaniaMaterial;
            this.informeComercial = informeComercial;
            this.contratoManager = contratoManager;
            this.repositorio = repositorio;
        }
        #region Servicios de DataAgro

        public ResultadoSap Ping()
        {
            return new ResultadoSap();
        }

        public ResultadoSap GrabarRiesgoComercial(RiesgoComercial oRiesgos)
        {
            var oEntityErrors = new ResultadoSap();
            try
            {
                logger.Debug("GrabarRiesgoComercial" + oRiesgos.ToXml());
                var resultado = riesgoComercial.ActualizacionDeRiesgoComercial(oRiesgos);
                oEntityErrors.ListaErrores.AddRange(resultado.Errores);
            }
            catch (Exception ex)
            {
                oEntityErrors.ListaErrores.Add(new ErrorMessage()
                {
                    Message = ex.Message
                });
            }
            oEntityErrors.HayError = oEntityErrors.ListaErrores.Any();
            return oEntityErrors;
        }

        public ResultadoSap GrabarCampaniaActual(CampaniaActual oCampania)
        {
            var oEntityErrors = new ResultadoSap();

            try
            {
                logger.Debug("GrabarCampaniaActual " + oCampania.ToXml());
                var resultado = campanaActual.ActualizacionCampaniaActual(oCampania);
                oEntityErrors.ListaErrores.AddRange(resultado.Errores);
            }
            catch (Exception ex)
            {
                oEntityErrors.ListaErrores.Add(new ErrorMessage()
                {
                    Message = ex.Message
                });
            }
            oEntityErrors.HayError = oEntityErrors.ListaErrores.Any();
            return oEntityErrors;
        }

        public ResultadoSap ActualizarCampaniaMaterial(List<CampaniaMaterialSAPDTO> oCampaniaMaterialSAP)
        {
            var oEntityErrors = new ResultadoSap();

            try
            {
                logger.Debug("ActualizarCampaniaMaterial " + oCampaniaMaterialSAP.ToXml());
                string aux = string.Empty;

                foreach (var sap in oCampaniaMaterialSAP)
                {
                    aux = aux + "//  CUIT: " + sap.CUIT.ToString() + "/  Campania: " + sap.Campania.ToString() + "/  Material: " + sap.Material.ToString() + "/  Mes: " + sap.Mes.ToString()
                        + "/  Anio: " + sap.Anio.ToString() + "/  Toneladas: " + sap.Toneladas.ToString() + "/  Comercial: " + sap.Comercial.ToString() + "\r\n";
                }
                logger.Debug(aux);

                oEntityErrors.ListaErrores.AddRange(campaniaMaterial.TraerCampañasPorGrano(oCampaniaMaterialSAP).Errores);
            }
            catch (Exception ex)
            {
                oEntityErrors.ListaErrores.Add(new ErrorMessage()
                {
                    Message = ex.Message
                });
            }
            oEntityErrors.HayError = oEntityErrors.ListaErrores.Any();
            return oEntityErrors;
        }

        public ResultadoSap ActualizarEstadoComercial(List<InformeComercialSAPDTO> LoInformeComercialSAP)
        {
            var oEntityErrors = new ResultadoSap();

            try
            {
                logger.Debug("ActualizarEstadoComercial " + LoInformeComercialSAP.ToXml());

                string aux = string.Empty;

                foreach (var oInformeComercialSAP in LoInformeComercialSAP)
                {
                    aux = aux + "//  CUIT: " + oInformeComercialSAP.CUIT.ToString() + "/  Material: " + oInformeComercialSAP.Material.ToString() + "/  MensajeSap: " + oInformeComercialSAP.RptSap.ToString() + "\\r\\n";
                    var rtaSap = String.Empty;
                    if (oInformeComercialSAP.RptSap.ToLower() != "ok")
                        rtaSap = oInformeComercialSAP.RptSap;

                    oEntityErrors.ListaErrores.AddRange(informeComercial.RespuestaDeSapCapacidadProductiva(oInformeComercialSAP.CUIT, oInformeComercialSAP.Material, rtaSap).ListaErrores);
                }

                logger.Debug(aux);
            }
            catch (Exception ex)
            {
                oEntityErrors.ListaErrores.Add(new ErrorMessage()
                {
                    Message = ex.Message
                });
            }
            oEntityErrors.HayError = oEntityErrors.ListaErrores.Any();
            return oEntityErrors;
        }
        public ResultadoSap ActualizarContratoSAP(ContratoSAPDto contratoSAP)
        {
            contratoSAP.Calidad = contratoSAP.Calidad ?? new List<CalidadSAP>();
            contratoSAP.DescuentoBonificaciones = contratoSAP.DescuentoBonificaciones ?? new List<DescuentoBonificacionSap>();
            contratoSAP.Apertura = contratoSAP.Apertura ?? new List<AperturaPrecioSap>();
            contratoSAP.Procedencia = contratoSAP.Procedencia != null ? contratoSAP.Procedencia.Trim() : contratoSAP.Procedencia;
            var oEntityErrors = new ResultadoSap();
            try
            {
                logger.Debug("ActualizandoContrato" + contratoSAP.ToXml());
                var contratoOriginal = repositorio.Obtener<Contrato, Contrato>(x => x.ContratoSAP == contratoSAP.ContratoSAP, x => x);

                var calidades = new List<Calidad>();
                var calEspecialList = repositorio.Listar<CalidadEspecial>();
                var standardCalidadList = repositorio.Listar<StandardDeCalidad>();


                logger.Debug("ActualizandoContrato2");

                var descuentos = new List<DescuentoBonificacion>();
                var preciosPactados = new List<PrecioPactado>();
                var tipoDescuentoList = repositorio.Listar<TipoDB>();
                var tipoPeriodoList = repositorio.Listar<TipoPeriodoDB>();

                foreach (var desc in contratoSAP.DescuentoBonificaciones ?? new List<DescuentoBonificacionSap>())
                {
                    if (desc.TipoPeriodo != "I" && desc.TipoDescBon != "B")
                    {
                        var descuento = new DescuentoBonificacion
                        {
                            ContratoId = contratoOriginal.Id,
                            FechaDesde = !string.IsNullOrEmpty(contratoSAP.FechaDesde) ? DateTime.ParseExact(desc.FechaDesde, "yyyy-MM-dd", CultureInfo.InvariantCulture) : (DateTime?)null,
                            FechaHasta = !string.IsNullOrEmpty(contratoSAP.FechaHasta) ? DateTime.ParseExact(desc.FechaHasta, "yyyy-MM-dd", CultureInfo.InvariantCulture) : (DateTime?)null,
                            Importe = desc.Importe,
                            Porcentaje = desc.PorcentajeDB,
                            MonedaId = desc.MonedaDB,
                            TipoDBId = tipoDescuentoList.FirstOrDefault(x => x.CodigoSap == desc.TipoDescBon).Id,
                            TipoPeriodoDBId = tipoPeriodoList.FirstOrDefault(x => x.CodigoSap == desc.TipoPeriodo).Id
                        };
                        descuentos.Add(descuento);
                    }
                    if (desc.TipoPeriodo != "E" && desc.TipoDescBon != "A")
                    {
                        var precio = new PrecioPactado
                        {
                            ContratoId = contratoOriginal.Id,
                            FechaDesde = !string.IsNullOrEmpty(contratoSAP.FechaDesde) ? DateTime.ParseExact(desc.FechaDesde, "yyyy-MM-dd", CultureInfo.InvariantCulture) : (DateTime?)null,
                            FechaHasta = !string.IsNullOrEmpty(contratoSAP.FechaHasta) ? DateTime.ParseExact(desc.FechaHasta, "yyyy-MM-dd", CultureInfo.InvariantCulture) : (DateTime?)null,
                            ImportePactado = desc.Importe,
                            Porcentaje = desc.PorcentajeDB,
                            MonedaPactadoId = desc.MonedaDB,
                            Precio = desc.Precio,
                            MonedaImportePactadoId = desc.Moneda

                        };
                        preciosPactados.Add(precio);
                    }
                }
                logger.Debug("ActualizandoContrato3");

                var aperturas = new List<AperturaPrecio>();
                var conceptoList = repositorio.Listar<ConceptoAperturaPrecio>();
                if (contratoSAP.Apertura != null && contratoSAP.Apertura.Count > 0)
                {
                    aperturas.Add(new AperturaPrecio { NegocioId = contratoOriginal.Id, ConceptoAperturaPrecioId = 1, Importe = 0, Porcentaje = 0 });
                    aperturas.Add(new AperturaPrecio { NegocioId = contratoOriginal.Id, ConceptoAperturaPrecioId = 2, Importe = 0, Porcentaje = 0 });
                    aperturas.Add(new AperturaPrecio { NegocioId = contratoOriginal.Id, ConceptoAperturaPrecioId = 3, Importe = 0, Porcentaje = 0 });
                    aperturas.Add(new AperturaPrecio { NegocioId = contratoOriginal.Id, ConceptoAperturaPrecioId = 4, Importe = 0, Porcentaje = 0 });
                }
                foreach (var aper in contratoSAP.Apertura ?? new List<AperturaPrecioSap>())
                {
                    var ConceptoAperturaPrecioId = conceptoList.FirstOrDefault(x => x.CodigoSap == aper.Concepto).Id;

                    aperturas.Where(a => a.ConceptoAperturaPrecioId == ConceptoAperturaPrecioId).Single().Importe = aper.Importe;
                    aperturas.Where(a => a.ConceptoAperturaPrecioId == ConceptoAperturaPrecioId).Single().MonedaId = aper.Moneda;
                    aperturas.Where(a => a.ConceptoAperturaPrecioId == ConceptoAperturaPrecioId).Single().Porcentaje = aper.Porcentaje;
                }
                logger.Debug("ActualizandoContrato4");

                var contrato = new Contrato();

                contrato.ContratoSAP = contratoSAP.ContratoSAP.PadLeft(10, '0');
                contrato.BoletoId = contratoSAP.Confirma == "X" ? 1 : contratoSAP.BolFisico == "X" ? 2 : contratoSAP.CartaOferta == "X" ? 4 : 3;
                contrato.BolsaId = repositorio.Obtener<BolsaCompraNet, int>(x => contratoSAP.Bolsa.Contains(x.CodigoSap), x => x.Id);
                contrato.CampanaId = repositorio.Obtener<Campaña, int>(x => x.Descripcion == contratoSAP.Cosecha, x => x.CampañaId);
                contrato.Cantidad = (double)contratoSAP.Cantidad;
                contrato.CantidadCamiones = contratoSAP.Camiones;
                contrato.CD = contratoSAP.AurCd == "X";
                contrato.ClasificacionId = repositorio.Obtener<ClasificacionCompraNet, int>(x => x.Descripcion == contratoSAP.Clasificacion, x => x.Id);
                contrato.Compensacion = contratoSAP.Compensacion == "X";
                contrato.DestinoId = repositorio.Obtener<Centro, int>(x => x.CodigoSap == contratoSAP.Centro, x => x.Id);
                contrato.CondicionFijacionId = !string.IsNullOrEmpty(contratoSAP.CondFijacion) ? repositorio.Obtener<CondicionFijacion, int>(x => x.CodigoSap == contratoSAP.CondFijacion, x => x.Id) : (int?)null;
                contrato.Consignatario = contratoSAP.Consignatario == "X";
                contrato.ContratoCorredor = contratoSAP.ContrCorr;
                contrato.ContratoMadre = contratoSAP.ContratoMadre;
                contrato.ContratoVendedor = contratoSAP.ContrVend;
                contrato.CorredorId = !string.IsNullOrEmpty(contratoSAP.CuitCorredor) ? repositorio.Obtener<CorredorProveedor, int>(x => x.Corredor.CUIT == contratoSAP.CuitCorredor, x => x.CorredorId) : (int?)null;
                contrato.DesdeFijacion = !string.IsNullOrEmpty(contratoSAP.FeDesdeFij) ? DateTime.ParseExact(contratoSAP.FeDesdeFij, "yyyy-MM-dd", CultureInfo.InvariantCulture) : (DateTime?)null;
                contrato.DiasPesificado = contratoSAP.DiasDiferimiento == 0 ? (int?)null : contratoSAP.DiasDiferimiento;
                contrato.PagoDiferido = contratoSAP.DiasDiferimiento > 0;
                contrato.Dolarizado = !string.IsNullOrEmpty(contratoSAP.FechaLimite);
                contrato.EstablecimientoPropio = contratoSAP.EstabPropio == "X" ? true : contratoSAP.EstabArrendado == "X" ? false : (bool?)null;
                contrato.FechaDolarizado = !string.IsNullOrEmpty(contratoSAP.FechaLimite) ? DateTime.ParseExact(contratoSAP.FechaLimite, "yyyy-MM-dd", CultureInfo.InvariantCulture) : (DateTime?)null;
                contrato.FechaDesde = DateTime.ParseExact(contratoSAP.FechaDesde, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                contrato.FechaEntrega = DateTime.ParseExact(contratoSAP.FechaEntrega, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                contrato.FechaHasta = DateTime.ParseExact(contratoSAP.FechaHasta, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                contrato.HastaFijacion = !string.IsNullOrEmpty(contratoSAP.FeHastaFij) ? DateTime.ParseExact(contratoSAP.FeHastaFij, "yyyy-MM-dd", CultureInfo.InvariantCulture) : (DateTime?)null;
                contrato.ImporteSustentable = contratoSAP.DescuentoBonificaciones.FirstOrDefault(x => x.TipoPeriodo == "I" && x.TipoDescBon == "B") != null ? contratoSAP.DescuentoBonificaciones.FirstOrDefault(x => x.TipoPeriodo == "I" && x.TipoDescBon == "B").Importe : (decimal?)null;
                contrato.LocalidadId = repositorio.Obtener<Localidad, int>(x => x.CodLocalidad == contratoSAP.Procedencia, x => x.LocalidadId);

                contrato.MaterialId = repositorio.Obtener<Material, int>(x => x.Codigo == contratoSAP.Material, x => x.MaterialId);
                contrato.MercsDeposito = contratoSAP.MercDescargada == "X";
                contrato.MonedaId = repositorio.Obtener<Moneda, string>(x => x.MonedaId == contratoSAP.Moneda, x => x.MonedaId);
                contrato.MonedaSustentableId = contratoSAP.DescuentoBonificaciones.FirstOrDefault(x => x.TipoPeriodo == "I" && x.TipoDescBon == "B") != null ? contratoSAP.DescuentoBonificaciones.FirstOrDefault(x => x.TipoPeriodo == "I" && x.TipoDescBon == "B").MonedaDB : "";
                contrato.NivelTarifaId = !string.IsNullOrEmpty(contratoSAP.FleteNivel) ? repositorio.Obtener<NivelTarifa, int>(x => x.CodigoSap == contratoSAP.FleteNivel, x => x.Id) : (int?)null;
                contrato.NoInformaSio = contratoSAP.NoInformaSio == "X";
                contrato.Observacion = contratoSAP.ObservacionCal1;
                contrato.PagoDirectoVendedor = contratoSAP.PagoDirVend == "X";
                contrato.PlanCanje = contratoSAP.IndOpCanje == "X";
                contrato.PorcentajeComision = contratoSAP.PorcComision == 0 ? (decimal?)null : contratoSAP.PorcComision;
                contrato.Precio = contratoSAP.Precio;
                contrato.PrecioNeto = contratoSAP.PrecioNeto;
                contrato.ProveedorId = repositorio.Obtener<Proveedor, int>(x => x.CUIT == contratoSAP.Proveedor && x.SegmentacionId != 5 && x.SegmentacionId != 7, x => x.ProveedorId);
                contrato.ProvinciaId = contratoSAP.Provincia;
                contrato.SelCargoMOA = contratoSAP.SelCargoMOA == "X";
                contrato.SelCargoVendedor = contratoSAP.SelCargoVend == "X";
                contrato.StandardDeCalidadId = repositorio.Obtener<StandardDeCalidad, int>(x => contratoSAP.Especial.Contains(x.CodigoSap), x => x.Id);
                if (contratoSAP.Especial == "03" && contrato.MaterialId == 3)
                {
                    contrato.StandardDeCalidadId = 3;
                }
                contrato.Sustentable = contratoSAP.Sustentable == "X";
                contrato.TarifaFlete = contratoSAP.FleteTarifa == 0 ? (decimal?)null : contratoSAP.FleteTarifa;
                contrato.Warrant = contratoSAP.AutCg == "X";
                contrato.ZonaId = !string.IsNullOrEmpty(contratoSAP.Zona) ? repositorio.Obtener<Zona, int>(x => x.CodigoSap == contratoSAP.Zona, x => x.Id) : (int?)null;
                contrato.PorcentajeDePago = contratoSAP.PorcentajeDePago ?? contratoOriginal.PorcentajeDePago ?? (decimal)97.5;
                logger.Debug("ActualizandoContrato calidades");

                foreach (var cal in contratoSAP.Calidad ?? new List<CalidadSAP>())
                {
                    var calidad = new Calidad
                    {
                        NegocioId = contratoOriginal.Id,
                        CalidadEspecialId = calEspecialList.FirstOrDefault(x => x.CodigoSap == cal.Codigo && x.MaterialId == contrato.MaterialId).Id,
                        StandardDeCalidadId = contrato.StandardDeCalidadId ?? 1,
                        Valor = cal.Valor,
                        PorcentajeDesde = cal.PorcentajeDesde,
                        PorcentajeHasta = cal.PorcentajeHasta
                    };
                    calidades.Add(calidad);
                }
                contrato.Calidad = calidades;
                contrato.AperturaPrecio = aperturas;
                contrato.Descuentos = descuentos;
                contrato.TipoNegocioId = 3;
                contrato.ComercialId = 1;
                contrato.Fecha = DateTime.Now;
                contrato.ZonaId = contrato.ZonaId ?? contratoOriginal.ZonaId;
                contrato.GrupoCompra = contratoOriginal.GrupoCompra;
                contrato.ContratoCorredor = String.IsNullOrEmpty(contratoSAP.ContratoCorredor) ? null : contratoSAP.ContratoCorredor;

                if (!string.IsNullOrEmpty(contratoSAP.PorcAPrecio) && !string.IsNullOrEmpty(contratoSAP.MonedaAPrecio))
                {
                    contrato.Descuentos.Add(new DescuentoBonificacion { TipoPeriodoDBId = 1, TipoDBId = 2, MonedaId = contratoSAP.MonedaAPrecio, Porcentaje = Convert.ToDecimal(contratoSAP.PorcAPrecio.Replace(",", "").Replace(".", ",")) });
                }
                if (!string.IsNullOrEmpty(contratoSAP.PorcSPrecio) && !string.IsNullOrEmpty(contratoSAP.MonedaSPrecio))
                {
                    contrato.Descuentos.Add(new DescuentoBonificacion { TipoPeriodoDBId = 1, TipoDBId = 1, MonedaId = contratoSAP.MonedaSPrecio, Porcentaje = Convert.ToDecimal(contratoSAP.PorcSPrecio.Replace(",", "").Replace(".", ",")) });
                }
                logger.Debug("ActualizandoContrato5");
                Validar(contrato, oEntityErrors);
                if (oEntityErrors.HayError)
                {
                    return oEntityErrors;
                }
                var resultado = contratoManager.ActualizarContratoSAP(contrato, true);
                oEntityErrors.ListaErrores.AddRange(resultado.Errores);
            }
            catch (Exception ex)
            {
                logger.Error(ex);

                oEntityErrors.ListaErrores.Add(new ErrorMessage()
                {
                    Message = ex.Message == "" ? (ex.InnerException != null ? ex.InnerException.Message : "") : ex.Message
                });
                oEntityErrors.HayError = true;
            }
            logger.Debug("ActualizandoContrato7 CONTRATOSAP:" + JsonConvert.SerializeObject(contratoSAP));
            logger.Debug("ActualizandoContrato7 RESULTADO:" + JsonConvert.SerializeObject(oEntityErrors));

            oEntityErrors.HayError = oEntityErrors.ListaErrores.Any();
            return oEntityErrors;
        }

        private void Validar(Contrato oParam, ResultadoSap oErrorMessages)
        {
            if (oParam.LocalidadId == 0)
            {
                oErrorMessages.ListaErrores.Add(new ErrorMessage() { Message = "El campo 'Procedencia' es invalido" });
            }
            oErrorMessages.HayError = oErrorMessages.ListaErrores.Any();
            if (oParam.CorredorId != null && oParam.ProveedorId != null && oParam.CorredorId != 0 && oParam.ProveedorId != 0)
            {
                if (!repositorio.Existe<CorredorProveedor>(x => x.CorredorId == oParam.CorredorId && x.ProveedorId == oParam.ProveedorId))
                {
                    repositorio.Agregar(new CorredorProveedor { CorredorId = oParam.CorredorId.Value, ProveedorId = oParam.ProveedorId.Value });

                }
            }

        }
        #endregion
    }
}
