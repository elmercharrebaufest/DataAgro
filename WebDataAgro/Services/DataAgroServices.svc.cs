using Kendo.DynamicLinq;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using WebDataAgro.Helpers.Excel;
using static WebDataAgro.MvcApplication;
using Filter = Kendo.DynamicLinq.Filter;
using Sort = Kendo.DynamicLinq.Sort;

namespace WebDataAgro.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "DataAgroServices" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select DataAgroServices.svc or DataAgroServices.svc.cs at the Solution Explorer and start debugging.
    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.PerCall,
        IncludeExceptionDetailInFaults = true)]
    public class DataAgroServices : IDataAgroServices
    {
        private readonly ILogger logger;
        private readonly IRiesgoComercialManager riesgoComercial;
        private readonly ICampaniaActualManager campanaActual;
        private readonly ICampaniaMaterialManager campaniaMaterial;
        private readonly IInformeComercialManager informeComercial;
        private readonly IContratoManager contratoManager;
        private readonly IRepositorio repositorio;
        private readonly ICupoManager cupoManager;
        private readonly IMailManager mailManager;
        private readonly IFijacionDePrecioContratoManager fijacionDePrecioContratoManager;
        private readonly ITipoDeCambioAgent tipoDeCambioAgent;
        private readonly IProveedorManager proveedorManager;
        private readonly IHomeManager homeManager;
        private readonly IConfiguracionInternaManager configuracionInternaManager;
        private readonly IMaterialManager materialManager;
        private readonly ICentroManager centroManager;
        private readonly ICampañaManager campañaManager;
        private readonly IConfiguracionBolsaManager configuracionBolsaManager;
        private readonly ILocalidadManager localidadManager;

        public DataAgroServices(ILogger logger,
            IRiesgoComercialManager riesgoComercial,
            ICampaniaActualManager campanaActual,
            ICampaniaMaterialManager campaniaMaterial,
            IInformeComercialManager informeComercial,
            IContratoManager contratoManager,
            IRepositorio repositorio,
            ICupoManager cupoManager,
            IMailManager mailManager,
            IFijacionDePrecioContratoManager fijacionDePrecioContratoManager,
            ITipoDeCambioAgent tipoDeCambioAgent,
            IProveedorManager proveedorManager,
            IHomeManager homeManager,
            IConfiguracionInternaManager configuracionInternaManager,
            IMaterialManager materialManager,
            ICentroManager centroManager,
            ICampañaManager campañaManager,
            IConfiguracionBolsaManager configuracionBolsaManager,
            ILocalidadManager localidadManager
            )
        {
            this.logger = logger;
            this.riesgoComercial = riesgoComercial;
            this.campanaActual = campanaActual;
            this.campaniaMaterial = campaniaMaterial;
            this.informeComercial = informeComercial;
            this.contratoManager = contratoManager;
            this.repositorio = repositorio;
            this.cupoManager = cupoManager;
            this.mailManager = mailManager;
            this.fijacionDePrecioContratoManager = fijacionDePrecioContratoManager;
            this.tipoDeCambioAgent = tipoDeCambioAgent;
            this.proveedorManager = proveedorManager;
            this.homeManager = homeManager;
            this.configuracionInternaManager = configuracionInternaManager;
            this.materialManager = materialManager;
            this.centroManager = centroManager;
            this.campañaManager = campañaManager;
            this.configuracionBolsaManager = configuracionBolsaManager;
            this.localidadManager = localidadManager;
        }

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
            var contrato = new Contrato();
            try
            {
                logger.Debug("ActualizandoContrato" + contratoSAP.ToXml());
                var contratoOriginal = repositorio.Obtener<Contrato>(x => x.ContratoSAP == contratoSAP.ContratoSAP && x.EstadoId != 8);
                if (contratoOriginal != null)
                    contrato.TipoNegocioId = contratoOriginal.TipoNegocioId;
                CrearProyeccionContrato(contratoSAP, contrato, null, true);
                logger.Debug("ActualizandoContrato5");
                ValidarContrato(contrato, oEntityErrors, contratoSAP);
                if (oEntityErrors.HayError)
                {
                    return oEntityErrors;
                }
                var resultado = contratoManager.ActualizarContratoSAP(contrato);
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
            //logger.Debug("ActualizandoContrato7 CONTRATO:" + JsonConvert.SerializeObject(contrato));
            logger.Debug("ActualizandoContrato7 RESULTADO:" + JsonConvert.SerializeObject(oEntityErrors));

            oEntityErrors.HayError = oEntityErrors.ListaErrores.Any();
            return oEntityErrors;
        }

        public ResultadoSap AltaContratoSAP(ContratoSAPDto contratoSAP)
        {
            contratoSAP.Calidad = contratoSAP.Calidad ?? new List<CalidadSAP>();
            contratoSAP.DescuentoBonificaciones = contratoSAP.DescuentoBonificaciones ?? new List<DescuentoBonificacionSap>();
            contratoSAP.Apertura = contratoSAP.Apertura ?? new List<AperturaPrecioSap>();
            contratoSAP.Procedencia = contratoSAP.Procedencia != null ? contratoSAP.Procedencia.Trim() : contratoSAP.Procedencia;
            var oEntityErrors = new ResultadoSap();
            var contrato = new Contrato();
            try
            {
                logger.Debug("Alta ContratoSap" + contratoSAP.ToXml());
                var c = contratoSAP.ContratoSAP.PadLeft(10, '0');
                if (repositorio.Existe<Contrato>(x => x.ContratoSAP == c))
                {
                    oEntityErrors.ListaErrores.Add(new ErrorMessage("Contrato", "El contrato ya existe en Data Agro."));
                    return oEntityErrors;
                }

                CrearProyeccionContrato(contratoSAP, contrato, null, false);

                logger.Debug("Validacion alta contrato");
                ValidarContrato(contrato, oEntityErrors, contratoSAP);
                if (oEntityErrors.HayError)
                {
                    return oEntityErrors;
                }

                var resultado = contratoManager.AltaContratoSAP(contrato);
                oEntityErrors.ListaErrores.AddRange(resultado.Errores);
                var idNuevo = repositorio.Obtener<Contrato, int>(x => x.ContratoSAP.Contains(contratoSAP.ContratoSAP), x => x.Id);
                oEntityErrors.ContratoId = idNuevo != 0 ? idNuevo.ToString() : "";
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
            //logger.Debug("Alta CONTRATO:" + JsonConvert.SerializeObject(contrato));
            logger.Debug("Alta RESULTADO:" + JsonConvert.SerializeObject(oEntityErrors));

            oEntityErrors.HayError = oEntityErrors.ListaErrores.Any();
            return oEntityErrors;
        }

        private void CrearProyeccionContrato(ContratoSAPDto contratoSAP, Contrato contrato, Contrato contratoOriginal, bool esActualizar)
        {
            var calidades = new List<Calidad>();
            var calEspecialList = repositorio.Listar<CalidadEspecial>();
            var standardCalidadList = repositorio.Listar<StandardDeCalidad>();

            contratoOriginal = contratoOriginal ?? new Contrato();
            logger.Debug("CrearProyeccionContrato - Alta de descuentos");

            var descuentos = new List<DescuentoBonificacion>();
            var preciosPactados = new List<PrecioPactado>();
            var tipoDescuentoList = repositorio.Listar<TipoDB>();
            var tipoPeriodoList = repositorio.Listar<TipoPeriodoDB>();

            foreach (var desc in contratoSAP.DescuentoBonificaciones ?? new List<DescuentoBonificacionSap>())
            {
                if (!(desc.TipoPeriodo == "I" && desc.TipoDescBon == "B") && !(desc.TipoPeriodo == "E" && string.IsNullOrEmpty(desc.TipoDescBon)))
                {
                    var descuento = new DescuentoBonificacion
                    {
                        ContratoId = esActualizar ? contratoOriginal.Id : 0,
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
                if (desc.TipoPeriodo == "E" && string.IsNullOrEmpty(desc.TipoDescBon))
                {
                    var precio = new PrecioPactado
                    {
                        ContratoId = esActualizar ? contratoOriginal.Id : 0,
                        FechaDesde = !string.IsNullOrEmpty(contratoSAP.FechaDesde) ? DateTime.ParseExact(desc.FechaDesde, "yyyy-MM-dd", CultureInfo.InvariantCulture) : (DateTime?)null,
                        FechaHasta = !string.IsNullOrEmpty(contratoSAP.FechaHasta) ? DateTime.ParseExact(desc.FechaHasta, "yyyy-MM-dd", CultureInfo.InvariantCulture) : (DateTime?)null,
                        ImportePactado = desc.Importe,
                        Porcentaje = desc.PorcentajeDB,
                        MonedaPactadoId = desc.Moneda,
                        Precio = desc.Precio,
                        MonedaImportePactadoId = desc.MonedaDB
                    };
                    preciosPactados.Add(precio);
                }
            }

            logger.Debug("CrearProyeccionContrato - Alta de apertura");

            var aperturas = new List<AperturaPrecio>();
            var conceptoList = repositorio.Listar<ConceptoAperturaPrecio>();
            if (contratoSAP.Apertura != null && contratoSAP.Apertura.Count > 0)
            {
                foreach (var item in conceptoList)
                {
                    aperturas.Add(new AperturaPrecio { NegocioId = esActualizar ? contratoOriginal.Id : 0, ConceptoAperturaPrecioId = item.Id, Importe = 0, Porcentaje = 0 });
                }
            }
            foreach (var aper in contratoSAP.Apertura ?? new List<AperturaPrecioSap>())
            {
                var ConceptoAperturaPrecioId = conceptoList.FirstOrDefault(x => x.CodigoSap == aper.Concepto).Id;
                if (!((contratoSAP.EPA == "X" || contratoSAP.EUDR == "X" || contratoSAP.Sustentable == "X") && ConceptoAperturaPrecioId == 4))
                {
                    aperturas.Find(a => a.ConceptoAperturaPrecioId == ConceptoAperturaPrecioId).Importe = aper.Importe;
                    aperturas.Find(a => a.ConceptoAperturaPrecioId == ConceptoAperturaPrecioId).MonedaId = aper.Moneda;
                    aperturas.Find(a => a.ConceptoAperturaPrecioId == ConceptoAperturaPrecioId).Porcentaje = aper.Porcentaje;
                }
            }
            if (esActualizar)
            {
                if (contratoOriginal.AperturaPrecio != null && contratoOriginal.AperturaPrecio.Count() == 0 && !aperturas.Any(a => a.Importe != 0 || a.Porcentaje != 0))
                {
                    aperturas = new List<AperturaPrecio>();
                }
            }

            logger.Debug("CrearProyeccionContrato - Alta del contrato");

            contrato.ContratoSAP = contratoSAP.ContratoSAP.PadLeft(10, '0');
            contrato.BoletoId = contratoSAP.Confirma == "X" ? 1 : contratoSAP.BolFisico == "X" ? 2 : contratoSAP.CartaOferta == "X" ? 4 : contratoSAP.SinBoleto == "X" ? 5 : 3;
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
            //contrato.ContratoCorredor = String.IsNullOrEmpty(contratoSAP.ContratoCorredor) ? null : contratoSAP.ContratoCorredor;
            contrato.ContratoMadre = contratoSAP.ContratoMadre;
            contrato.ContratoVendedor = contratoSAP.ContrVend;
            contrato.CorredorId = !string.IsNullOrEmpty(contratoSAP.CuitCorredor) ? repositorio.Obtener<CorredorProveedor, int>(x => x.Corredor.CUIT == contratoSAP.CuitCorredor, x => x.CorredorId) : (int?)null;
            contrato.DesdeFijacion = !string.IsNullOrEmpty(contratoSAP.FeDesdeFij) ? DateTime.ParseExact(contratoSAP.FeDesdeFij, "yyyy-MM-dd", CultureInfo.InvariantCulture) : (DateTime?)null;
            contrato.DiasPesificado = contratoSAP.DiasDiferimiento == 0 ? (int?)null : contratoSAP.DiasDiferimiento;
            contrato.PagoDiferido = contratoSAP.DiasDiferimiento > 0;
            contrato.Dolarizado = contratoSAP.DolarizadoExpress != "X" && !string.IsNullOrEmpty(contratoSAP.FechaLimite);
            contrato.DolarizadoCorredor = false;
            if (contrato.CorredorId.HasValue && contrato.Dolarizado == true)
            {
                contrato.Dolarizado = false;
                contrato.DolarizadoCorredor = true;
            }
            contrato.EstablecimientoPropio = contratoSAP.EstabPropio == "X" ? true : contratoSAP.EstabArrendado == "X" ? false : (bool?)null;
            contrato.FechaDolarizado = !string.IsNullOrEmpty(contratoSAP.FechaLimite) ? DateTime.ParseExact(contratoSAP.FechaLimite, "yyyy-MM-dd", CultureInfo.InvariantCulture) : (DateTime?)null;
            contrato.FechaDesde = DateTime.ParseExact(contratoSAP.FechaDesde, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            contrato.FechaEntrega = DateTime.ParseExact(contratoSAP.FechaEntrega, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            contrato.FechaHasta = DateTime.ParseExact(contratoSAP.FechaHasta, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            contrato.HastaFijacion = !string.IsNullOrEmpty(contratoSAP.FeHastaFij) ? DateTime.ParseExact(contratoSAP.FeHastaFij, "yyyy-MM-dd", CultureInfo.InvariantCulture) : (DateTime?)null;
            if (!esActualizar)
            {
                var fechaCreacion = DateTime.ParseExact(contratoSAP.FechaCreacion, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                var fechaOperacion = DateTime.ParseExact(contratoSAP.FechaOperacion, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                contrato.FechaOperacion = fechaOperacion;
                contrato.MotivoOperacionAnterior = fechaOperacion.Date < fechaCreacion.Date ? "Cargado desde SAP" : "";
                var hora = DateTime.ParseExact(contratoSAP.HORAACT, "HH:mm:ss", CultureInfo.InvariantCulture);
                TimeSpan time = new TimeSpan(hora.Hour, hora.Minute, hora.Second);
                contrato.Fecha = DateTime.ParseExact(contratoSAP.FechaCreacion, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                contrato.Fecha = contrato.Fecha.Add(time);
            }
            contrato.FechaCierta = !string.IsNullOrEmpty(contratoSAP.FechaCierta) ? DateTime.ParseExact(contratoSAP.FechaCierta, "yyyy-MM-dd", CultureInfo.InvariantCulture) : (DateTime?)null;
            contrato.ImporteSustentable = contratoSAP.DescuentoBonificaciones.FirstOrDefault(x => x.TipoPeriodo == "I" && x.TipoDescBon == "B") != null ? contratoSAP.DescuentoBonificaciones.FirstOrDefault(x => x.TipoPeriodo == "I" && x.TipoDescBon == "B").Importe : (decimal?)null;
            contrato.MonedaSustentableId = contratoSAP.DescuentoBonificaciones.FirstOrDefault(x => x.TipoPeriodo == "I" && x.TipoDescBon == "B") != null ? contratoSAP.DescuentoBonificaciones.FirstOrDefault(x => x.TipoPeriodo == "I" && x.TipoDescBon == "B").MonedaDB : "";

            //sap tiene problemas con la moneda USD y llega corrido los decimales  por eso el *10 -- Lo arreglaron en SAP y sacamos el parche 29/09
            if (contrato.ImporteSustentable.HasValue && !string.IsNullOrEmpty(contrato.MonedaSustentableId) && contrato.MonedaSustentableId.Contains("USDM"))
            {
                contrato.ImporteSustentable = contrato.ImporteSustentable; /** 10;*/
            }

            if (contrato.ImporteSustentable == -1)
            {
                contrato.ImporteSustentable = null;
                contrato.TarifaAConvenir = true;
            }

            contrato.FechaDesdeSustentable = contratoSAP.DescuentoBonificaciones.FirstOrDefault(x => x.TipoPeriodo == "I" && x.TipoDescBon == "B") != null
            && !string.IsNullOrEmpty(contratoSAP.DescuentoBonificaciones.FirstOrDefault(x => x.TipoPeriodo == "I" && x.TipoDescBon == "B").FechaDesde) ?
            DateTime.ParseExact(contratoSAP.DescuentoBonificaciones.FirstOrDefault(x => x.TipoPeriodo == "I" && x.TipoDescBon == "B").FechaDesde, "yyyy-MM-dd", CultureInfo.InvariantCulture) : (DateTime?)null;

            contrato.FechaHastaSustentable = contratoSAP.DescuentoBonificaciones.FirstOrDefault(x => x.TipoPeriodo == "I" && x.TipoDescBon == "B") != null
            && !string.IsNullOrEmpty(contratoSAP.DescuentoBonificaciones.FirstOrDefault(x => x.TipoPeriodo == "I" && x.TipoDescBon == "B").FechaHasta) ?
            DateTime.ParseExact(contratoSAP.DescuentoBonificaciones.FirstOrDefault(x => x.TipoPeriodo == "I" && x.TipoDescBon == "B").FechaHasta, "yyyy-MM-dd", CultureInfo.InvariantCulture) : (DateTime?)null;

            contrato.LocalidadId = repositorio.Obtener<Localidad, int>(x => x.CodLocalidad == contratoSAP.Procedencia, x => x.LocalidadId);

            contrato.MaterialId = repositorio.Obtener<Material, int>(x => x.Codigo == contratoSAP.Material, x => x.MaterialId);
            contrato.MercsDeposito = contratoSAP.MercDescargada == "X";
            contrato.MonedaId = repositorio.Obtener<Moneda, string>(x => x.MonedaId == contratoSAP.Moneda, x => x.MonedaId);
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
            contrato.StandardDeCalidadId = string.IsNullOrEmpty(contratoSAP.Especial) ? (int?)null : repositorio.Obtener<StandardDeCalidad, int>(x => contratoSAP.Especial.Contains(x.CodigoSap), x => x.Id);
            contrato.EstadoId = 5;
            contrato.TipoAgenteCompraId = contratoSAP.TipoAgenteCompraId == "9952569841" ? (int?)1 : null;
            contrato.CaratulaMAT = contratoSAP.CaratulaMAT;
            contrato.CaratulaExtension = contratoSAP.CaratulaExtension;
            contrato.PrecioAjusteComision = contratoSAP.PrecioAjusteComision;
            contrato.MonedaAjusteComisionId = contratoSAP.MonedaAjusteComisionId;
            contrato.ChequeElectronico = contratoSAP.ZLSCH == "=";
            contrato.PagoCBU = contratoSAP.CUENTA_MRP;
            contrato.DolarizadoExpress = contratoSAP.DolarizadoExpress == "X";
            if (!esActualizar)
            {
                contrato.Canje = contratoSAP.Canje == "X";
                contrato.PrestamoDevolucion = contratoSAP.TipoNegocio == "PRESTAMO_DEVOLUCION";
                contrato.EsFason = contratoSAP.TipoNegocio == "FASON" ? true : (bool?)null;
                contrato.Madre = contratoSAP.TipoNegocio == "MADRE" ? true : contratoSAP.TipoNegocio == "HIJO" ? false : (bool?)null;
                contrato.TipoNegocioId = contratoSAP.TipoNegocio == "HIJO" ? 2 :
                    contratoSAP.TipoNegocio == "MADRE" ? 1 : contratoSAP.TipoNegocio == "FASON" ? 1 : contratoSAP.TipoNegocio == "PRESTAMO_DEVOLUCION" ? 1 :
                    contratoSAP.TipoNegocio == "VENTA" ? 1 : repositorio.Obtener<TipoNegocio, int>(x => x.Descripcion == contratoSAP.TipoNegocio, x => x.TipoNegocioId);
            }
            contrato.Monto = contratoSAP.Monto == 0 ? (decimal?)null : contratoSAP.Monto;
            contrato.Insumo = contratoSAP.Insumo;
            contrato.MonedaCanjeId = contratoSAP.MonedaCanjeId;
            contrato.Venta = contratoSAP.TipoNegocio == "VENTA" || contratoSAP.TipoNegocio == "VENTAS";
            contrato.PlantaDestinoId = !String.IsNullOrEmpty(contratoSAP.PlantaDestino) ? repositorio.Obtener<Centro, int>(x => x.CodigoSap == contratoSAP.PlantaDestino, x => x.Id) : (int?)null;

            if (contrato.Venta == true && contrato.TipoNegocioId == 2)
            {
                contrato.Cantidad = Math.Abs(contrato.Cantidad) * -1;
            }
            if (contratoSAP.Especial == "03" && contrato.MaterialId == 3)
            {
                contrato.StandardDeCalidadId = 3;
            }
            if (contratoSAP.Especial == "04" && contrato.MaterialId == 1)
            {
                contrato.StandardDeCalidadId = 7;
            }
            contrato.Sustentable = contratoSAP.Sustentable == "X" && contratoSAP.EPA != "X" && contratoSAP.EUDR != "X";
            contrato.EPA = contratoSAP.EPA == "X";
            contrato.EUDR = contratoSAP.EUDR == "X";
            if (contrato.EPA || contrato.EUDR || contrato.Sustentable)
            {
                if (contratoSAP.Apertura != null && contratoSAP.Apertura.Any(x => x.Concepto == "BO" && x.Importe > 0)) //es Sobre Precio
                {
                    contrato.SustentableTipoDBId = 1;
                    contrato.ImporteSustentable = contratoSAP.Apertura.Find(x => x.Concepto == "BO")?.Importe;
                    contrato.MonedaSustentableId = contratoSAP.Apertura.Find(x => x.Concepto == "BO")?.Moneda;
                }
                else if (contratoSAP.DescuentoBonificaciones != null && contratoSAP.DescuentoBonificaciones.Any()) //es Fuera de Precio
                {
                    contrato.SustentableTipoDBId = 2;
                    contrato.ImporteSustentable = contratoSAP.DescuentoBonificaciones.Find(x => x.TipoPeriodo == "I" && x.TipoDescBon == "B")?.Importe;
                    contrato.MonedaSustentableId = contratoSAP.DescuentoBonificaciones.Find(x => x.TipoPeriodo == "I" && x.TipoDescBon == "B")?.MonedaDB;
                }
            }
            else
            {
                contrato.SustentableTipoDBId = null;
                contrato.ImporteSustentable = null;
                contrato.MonedaSustentableId = null;
            }
            contrato.TarifaFlete = contratoSAP.FleteTarifa == 0 ? (decimal?)null : contratoSAP.FleteTarifa;
            contrato.Warrant = contratoSAP.AutCg == "X";

            contrato.ZonaId = !string.IsNullOrEmpty(contratoSAP.Zona) ? repositorio.Obtener<Zona, int>(x => x.CodigoSap == contratoSAP.Zona, x => x.Id) : contratoOriginal.ZonaId;

            contrato.PorcentajeDePago = contratoSAP.PorcentajeDePago ?? contratoOriginal.PorcentajeDePago ?? (contrato.MaterialId == (int)EnumMateriales.TRIGO ? (decimal)95.0 : (decimal)97.5);


            logger.Debug("Alta Contrato calidades");

            foreach (var cal in contratoSAP.Calidad ?? new List<CalidadSAP>())
            {
                var calidad = new Calidad
                {
                    NegocioId = esActualizar ? contratoOriginal.Id : 0,
                    CalidadEspecialId = calEspecialList.FirstOrDefault(x => x.CodigoSap == cal.Codigo && x.MaterialId == contrato.MaterialId).Id,
                    StandardDeCalidadId = contrato.StandardDeCalidadId ?? 1,
                    Valor = cal.Valor,
                    PorcentajeDesde = cal.PorcentajeDesde,
                    PorcentajeHasta = cal.PorcentajeHasta
                };
                if (calidad.StandardDeCalidadId == 2)
                {
                    if ((calidad.Valor == 0 && calidad.PorcentajeDesde == 1 && calidad.PorcentajeHasta == 1 && (calidad.CalidadEspecialId == 4 || calidad.CalidadEspecialId == 5)) || calidad.CalidadEspecialId == 10)
                    {
                        if (esActualizar)
                        {
                            Calidad quality = contratoOriginal.Calidad.FirstOrDefault(a => a.CalidadEspecialId == calidad.CalidadEspecialId);
                            if (contratoOriginal.Calidad != null && quality != null)
                            {
                                calidad.PorcentajeDesde = quality.PorcentajeDesde;
                                calidad.PorcentajeHasta = quality.PorcentajeHasta;
                                calidad.Valor = quality.Valor;
                            }
                        }
                        else
                        {
                            calidad.PorcentajeDesde = null;
                            calidad.PorcentajeHasta = null;
                            calidad.Valor = 2;
                        }
                    }
                    else
                    {
                        if (calidad.CalidadEspecialId == 1 && calidad.PorcentajeHasta == 51)
                        {
                            calidad.PorcentajeHasta = 40;
                        }
                    }
                }
                else if (calidad.StandardDeCalidadId == 7)
                {
                    if (esActualizar)
                    {
                        if (contratoOriginal.Calidad != null && contratoOriginal.Calidad.FirstOrDefault(a => a.CalidadEspecialId == calidad.CalidadEspecialId) != null)
                        {
                            calidad.Valor = contratoOriginal.Calidad.FirstOrDefault(a => a.StandardDeCalidadId == calidad.StandardDeCalidadId).Valor;
                            calidad.PorcentajeDesde = contratoOriginal.Calidad.FirstOrDefault(a => a.StandardDeCalidadId == calidad.StandardDeCalidadId).PorcentajeDesde;
                            calidad.PorcentajeHasta = contratoOriginal.Calidad.FirstOrDefault(a => a.StandardDeCalidadId == calidad.StandardDeCalidadId).PorcentajeHasta;
                        }
                    }
                }

                calidades.Add(calidad);
            }
            contrato.Descuentos = descuentos;
            contrato.Calidad = calidades;
            contrato.AperturaPrecio = aperturas;
            contrato.PrecioPactado = preciosPactados;
            if (!esActualizar)
            {
                var comercial = repositorio.Obtener<Comercial>(x => x.IdActiveDirectory == contratoSAP.Comercial || x.IdUsuarioSAP == contratoSAP.Comercial);
                contrato.GrupoCompra = comercial.GrupoDeComprasId;
                contrato.ComercialId = comercial.ComercialId;
                contrato.UsuarioId = contratoSAP.Comercial;
                contrato.ComercialCreadorId = repositorio.Obtener<Comercial, int>(x => x.IdActiveDirectory == contratoSAP.ComercialCreador || x.IdUsuarioSAP == contratoSAP.ComercialCreador, x => x.ComercialId);
            }


            if ((!string.IsNullOrEmpty(contratoSAP.PorcAPrecio) || !string.IsNullOrEmpty(contratoSAP.ImporteAPrecio))
                && (Convert.ToDecimal(contratoSAP.PorcAPrecio.Replace(",", "").Replace(".", ",")) != 0 || Convert.ToDecimal(contratoSAP.ImporteAPrecio.Replace(",", "").Replace(".", ",")) != 0))
            {
                contrato.Descuentos.Add(new DescuentoBonificacion { TipoPeriodoDBId = 1, TipoDBId = 2, MonedaId = contratoSAP.MonedaAPrecio, Porcentaje = Convert.ToDecimal(contratoSAP.PorcAPrecio.Replace(",", "").Replace(".", ",")), Importe = Convert.ToDecimal(contratoSAP.ImporteAPrecio.Replace(",", "").Replace(".", ",")) });
            }
            if ((!string.IsNullOrEmpty(contratoSAP.PorcSPrecio) || !string.IsNullOrEmpty(contratoSAP.ImporteSPrecio))
                && (Convert.ToDecimal(contratoSAP.PorcSPrecio.Replace(",", "").Replace(".", ",")) != 0 || Convert.ToDecimal(contratoSAP.ImporteSPrecio.Replace(",", "").Replace(".", ",")) != 0))
            {
                contrato.Descuentos.Add(new DescuentoBonificacion { TipoPeriodoDBId = 1, TipoDBId = 1, MonedaId = contratoSAP.MonedaSPrecio, Porcentaje = Convert.ToDecimal(contratoSAP.PorcSPrecio.Replace(",", "").Replace(".", ",")), Importe = Convert.ToDecimal(contratoSAP.ImporteSPrecio.Replace(",", "").Replace(".", ",")) });
            }

            contrato.PosicionCBOT = contratoSAP.PosicionCBOT;
            contrato.TipoPosicionCBOTId = contratoSAP.TipoPosicionCBOTId;
            contrato.Cesion = contratoSAP.Cesion == "X";

            contrato.Condicional = contratoSAP.Condicional == "X";
            contrato.CondicionalPrecio = contratoSAP.CondicionalPrecio;
            contrato.CondicionalMonedaId = repositorio.Obtener<Moneda, string>(x => x.MonedaId == contratoSAP.CondicionalMonedaId, x => x.MonedaId);
            contrato.CondicionalFecha = !string.IsNullOrEmpty(contratoSAP.CondicionalFecha) ? DateTime.ParseExact(contratoSAP.CondicionalFecha, "yyyy-MM-dd", CultureInfo.InvariantCulture) : (DateTime?)null;
            contrato.CondicionalPosicion = contratoSAP.CondicionalPosicion;
            //contrato.KgMaximo = contratoSAP.KGMaximo;
            //contrato.KgMinimo = contratoSAP.KGMinimo;
            if (!string.IsNullOrEmpty(contratoSAP.CondicionalContratoSAP))
            {
                string num = contratoSAP.CondicionalContratoSAP.PadLeft(10, '0');
                var condicional = repositorio.Obtener<Contrato>(x => x.ContratoSAP == num);
                if (contrato != null)
                {
                    contrato.CondicionalContratoId = condicional.Id;
                }
            }
            contrato.Pizarra = contratoSAP.Pizarra == "X";

            if (ConfigurationManager.AppSettings["SacarCeroSAP"] == "Si") //se borra este bloque si se confirma que la división ya no es necesaria. May24
            {
                foreach (var desc in contrato.Descuentos)
                {
                    // los importes en USDM que llegan de sap tienen un 0 demás
                    if (desc.Importe != 0 && desc.MonedaId.Trim() == "USDM")
                    {
                        desc.Importe /= 10;
                    }
                }
            }
        }

        public ResultadoSap ActualizarFijacionSAP(FijacionSAPDto fijacionSAP)
        {
            var oEntityErrors = new ResultadoSap();
            try
            {
                logger.Debug("ActualizandoFijacion" + fijacionSAP.ToXml());
                var fijacion = new FijacionDePrecioContrato
                {
                    FijacionSAP = fijacionSAP.FijacionSAP,
                    ChequeElectronico = fijacionSAP.ZLSCH == "=",
                    PagoCBU = fijacionSAP.CUENTA_MRP
                };
                if (oEntityErrors.HayError)
                {
                    return oEntityErrors;
                }
                var resultado = fijacionDePrecioContratoManager.ActualizarFijacionSap(fijacion);
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
            logger.Debug("ActualizandoFijacion CONTRATOSAP:" + JsonConvert.SerializeObject(fijacionSAP));
            logger.Debug("ActualizandoFijacion RESULTADO:" + JsonConvert.SerializeObject(oEntityErrors));

            oEntityErrors.HayError = oEntityErrors.ListaErrores.Any();
            return oEntityErrors;


        }

        public ResultadoSap AltaFijacionSAP(FijacionSAPDto fijacionSAP)
        {
            //contratoSAP.Calidad = contratoSAP.Calidad ?? new List<CalidadSAP>();
            //contratoSAP.DescuentoBonificaciones = contratoSAP.DescuentoBonificaciones ?? new List<DescuentoBonificacionSap>();
            fijacionSAP.Apertura = fijacionSAP.Apertura ?? new List<AperturaPrecioSap>();
            //fijacionSAP.Procedencia = fijacionSAP.Procedencia != null ? fijacionSAP.Procedencia.Trim() : fijacionSAP.Procedencia;           
            var oEntityErrors = new ResultadoSap();
            try
            {
                logger.Debug("AltaFijacion" + fijacionSAP.ToXml());
                //if (repositorio.Existe<FijacionDePrecioContrato>(x => x.FijacionSAP == f)) {
                //    oEntityErrors.ListaErrores.Add( new ErrorMessage("Fijacion", "la fijacion ya existe en DataAgro"));
                //    return oEntityErrors;
                //}
                var NroContratoSAP = fijacionSAP.ContratoSAP.PadLeft(10, '0');
                var aFijar = repositorio.Obtener<Contrato>(x => x.ContratoSAP == NroContratoSAP);

                var fijacion = new FijacionDePrecioContrato();
                if (aFijar != null)
                {
                    fijacion.ClasificacionContrato = aFijar.Clasificacion.Descripcion.ToUpper();
                    if (aFijar.TipoPosicionCBOTId == 3)
                    {
                        fijacion.TipoPosicionCBOTId = 3;
                    }
                }
                fijacion.FijacionSAP = fijacionSAP.FijacionSAP;
                fijacion.ChequeElectronico = fijacionSAP.ZLSCH == "=";
                fijacion.TrigoEspecial = fijacionSAP.TrigoEspecial == "X";
                fijacion.PagoCBU = fijacionSAP.CUENTA_MRP;
                fijacion.ContratoSAP = fijacionSAP.ContratoSAP.PadLeft(10, '0');
                fijacion.ContratoId = aFijar != null ? aFijar.Id : (int?)null;
                fijacion.Precio = fijacionSAP.Precio;
                fijacion.Cantidad = (double)fijacionSAP.Cantidad;
                //fijacion.ComercialId = repositorio.Obtener<Comercial, int>(x => x.IdActiveDirectory == fijacionSAP.Comercial, x => x.ComercialId);
                fijacion.DestinoId = repositorio.Obtener<Centro, int>(x => x.CodigoSap == fijacionSAP.Centro, x => x.Id);
                logger.Debug("Validacion Precio " + (fijacionSAP.Precio == 0));
                fijacion.Pizarra = fijacionSAP.Pizarra == "X";
                fijacion.Posicion = fijacionSAP.Posicion;
                fijacion.CorredorId = !string.IsNullOrEmpty(fijacionSAP.CuitCorredor) ? repositorio.Obtener<CorredorProveedor, int>(x => x.Corredor.CUIT == fijacionSAP.CuitCorredor, x => x.CorredorId) : (int?)null;
                fijacion.DiasPesificado = fijacionSAP.DiasDiferimiento == 0 ? (int?)null : fijacionSAP.DiasDiferimiento;
                fijacion.PagoDiferido = fijacionSAP.DiasDiferimiento > 0;
                var hora = DateTime.ParseExact(fijacionSAP.HORAACT, "HH:mm:ss", CultureInfo.InvariantCulture);
                TimeSpan time = new TimeSpan(hora.Hour, hora.Minute, hora.Second);
                fijacion.Fecha = DateTime.ParseExact(fijacionSAP.FechaCreacion, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                fijacion.Fecha = fijacion.Fecha.Add(time);
                fijacion.FechaOperacion = DateTime.ParseExact(fijacionSAP.FechaOperacion, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                fijacion.MotivoOperacionAnterior = fijacion.FechaOperacion.Date < fijacion.Fecha.Date ? "Cargado desde SAP" : "";
                fijacion.FechaHasta = DateTime.ParseExact(fijacionSAP.FechaHasta, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                fijacion.FechaDesde = DateTime.ParseExact(fijacionSAP.FechaDesde, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                fijacion.MaterialId = repositorio.Obtener<Material, int>(x => x.Codigo == fijacionSAP.Material, x => x.MaterialId);
                fijacion.MonedaId = repositorio.Obtener<Moneda, string>(x => x.MonedaId == fijacionSAP.Moneda, x => x.MonedaId);
                if (fijacion.Pizarra == true)
                {
                    fijacion.MonedaId = "ARP  ";
                }
                fijacion.CampanaId = repositorio.Obtener<Campaña, int>(x => x.Descripcion == fijacionSAP.Cosecha, x => x.CampañaId);
                fijacion.PrecioNeto = fijacionSAP.PrecioNeto;
                fijacion.ProveedorId = repositorio.Obtener<Proveedor, int>(x => x.CUIT == fijacionSAP.Proveedor && x.SegmentacionId != 5 && x.SegmentacionId != 7, x => x.ProveedorId);
                fijacion.EstadoId = 5;
                fijacion.TipoNegocioId = 3;
                var comercial = repositorio.Obtener<Comercial>(x => x.IdActiveDirectory == fijacionSAP.Comercial || x.IdUsuarioSAP == fijacionSAP.Comercial);
                var comercialCreador = repositorio.Obtener<Comercial>(x => x.IdActiveDirectory == fijacionSAP.ComercialCreador || x.IdUsuarioSAP == fijacionSAP.ComercialCreador);
                fijacion.ComercialId = comercial.ComercialId;
                fijacion.ComercialCreadorId = comercialCreador.ComercialId;
                fijacion.GrupoCompra = comercial.GrupoDeComprasId;

                fijacion.Canje = fijacionSAP.Canje == "X";
                fijacion.Virtual = fijacionSAP.Virtual == "X";
                logger.Debug("Alta fijacion Apertura");

                var aperturas = new List<AperturaPrecio>();
                var conceptoList = repositorio.Listar<ConceptoAperturaPrecio>();

                if (fijacionSAP.Apertura != null && fijacionSAP.Apertura.Count > 0)
                {
                    foreach (var item in conceptoList)
                    {
                        aperturas.Add(new AperturaPrecio { ConceptoAperturaPrecioId = item.Id, Importe = 0, Porcentaje = 0 });
                    }
                }
                foreach (var aper in fijacionSAP.Apertura ?? new List<AperturaPrecioSap>())
                {
                    var ConceptoAperturaPrecioId = conceptoList.FirstOrDefault(x => x.CodigoSap == aper.Concepto).Id;

                    aperturas.Where(a => a.ConceptoAperturaPrecioId == ConceptoAperturaPrecioId).Single().Importe = aper.Importe;
                    aperturas.Where(a => a.ConceptoAperturaPrecioId == ConceptoAperturaPrecioId).Single().MonedaId = aper.Moneda;
                    aperturas.Where(a => a.ConceptoAperturaPrecioId == ConceptoAperturaPrecioId).Single().Porcentaje = aper.Porcentaje;
                }

                if (oEntityErrors.HayError)
                {
                    return oEntityErrors;
                }
                fijacion.AperturaPrecio = aperturas;
                var resultado = fijacionDePrecioContratoManager.AltaFijacionSap(fijacion, fijacionSAP.FijacionVirtuales);

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
            logger.Debug("AltaFijacion negocioSAP:" + JsonConvert.SerializeObject(fijacionSAP));
            logger.Debug("AltaFijacion RESULTADO:" + JsonConvert.SerializeObject(oEntityErrors));

            oEntityErrors.HayError = oEntityErrors.ListaErrores.Any();
            return oEntityErrors;


        }

        public ResultadoSap AnularFijacionSAP(FijacionSAP fijacionSAP)
        {
            var oEntityErrors = new ResultadoSap();
            try
            {
                logger.Debug("AnularFijacion " + fijacionSAP.Fijacion);
                var resultado = fijacionDePrecioContratoManager.AnularFijacionSAP(fijacionSAP, null);
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

        public ResultadoSap AnulaFijacionVirtualSAP(FijacionVirtualSAP fijacionSAP)
        {
            var oEntityErrors = new ResultadoSap();
            try
            {
                logger.Debug("AnularFijacionVirtual " + fijacionSAP.Contrato);
                var resultado = fijacionDePrecioContratoManager.AnularFijacionSAP(null, fijacionSAP);
                oEntityErrors.ListaErrores.AddRange(resultado.Errores);
            }
            catch (Exception ex)
            {
                oEntityErrors.ListaErrores.Add(new ErrorMessage()
                {
                    Message = ex.Message
                });
                logger.Error("AnularFijacionVirtual Error");
                logger.Error(ex);
            }
            oEntityErrors.HayError = oEntityErrors.ListaErrores.Any();
            return oEntityErrors;
        }

        public ResultadoSap ActualizarCupoSAP(CupoSapDto cupoSAP)
        {
            var oEntityErrors = new ResultadoSap();
            try
            {
                logger.Debug("Actualizando Cupo" + cupoSAP.ToXml());
                var cupoOriginal = repositorio.Obtener<Cupo>(x => x.CupoSap == cupoSAP.Codigo);
                if (cupoOriginal == null || cupoOriginal.Id == 0)
                {
                    oEntityErrors.ListaErrores.Add(new ErrorMessage { Source = "Codigo", Message = "No existe cupo " + (cupoSAP.Codigo ?? "") + " en DataAgro" });
                    return oEntityErrors;
                }

                var cupo = new Cupo
                {
                    Id = cupoOriginal.Id,
                    FechaIngreso = DateTime.ParseExact(cupoSAP.FechaIngreso, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    CupoSap = cupoSAP.Codigo,
                    MaterialId = repositorio.Obtener<Material, int>(x => x.Codigo == cupoSAP.Material, x => x.MaterialId)
                };
                string cuit = cupoSAP.Proveedor;
                int ProveedorId = 0;
                if (cuit.StartsWith("C"))
                {
                    cuit = cuit.Replace("C", "");
                    ProveedorId = repositorio.Obtener<Proveedor, int>(x => x.CUIT.Contains(cuit) && (x.SegmentacionId == 5 || x.SegmentacionId == 7), x => x.ProveedorId);
                }
                if (cuit.StartsWith("00"))
                {
                    cuit = cuit.Remove(0, 2);
                    ProveedorId = repositorio.Obtener<Proveedor, int>(x => x.CUIT.Contains(cuit) && x.SegmentacionId != 5 && x.SegmentacionId != 7, x => x.ProveedorId);
                }

                cupo.ProveedorId = ProveedorId;
                cupo.CentroId = repositorio.Obtener<Centro, int>(x => x.CodigoSap == cupoSAP.Planta, x => x.Id);
                cupo.ZonaCupoId = repositorio.Obtener<ZonaCupo, int>(x => x.CodigoSap == cupoSAP.Zona, x => x.Id);
                cupo.Observaciones = cupoSAP.Observaciones;
                cupo.Destinatario = cupoSAP.Destinatario ?? "";
                cupo.FleteProcedencia = cupoSAP.FleteProcedencia == "S";
                cupo.Calidad = cupoSAP.Calidad == "01" ? "Camara" : cupoSAP.Calidad == "03" ? "Fabrica" : "";

                cupo.EstadoCupoId = cupoSAP.Borrado == "X" ? 4 : cupoOriginal.EstadoCupoId;
                if (!string.IsNullOrEmpty(cupoSAP.Comercial))
                {
                    cupo.ComercialId = repositorio.Obtener<Comercial, int>(x => x.IdActiveDirectory == cupoSAP.Comercial || x.IdUsuarioSAP == cupoSAP.Comercial, x => x.ComercialId);
                }
                if (cupo.ComercialId == null || cupo.ComercialId == 0)
                {
                    cupo.ComercialId = cupoOriginal.ComercialId;
                }
                logger.Debug("Actualizando CUPOSAP VALIDAR");
                ValidarCupo(cupo, oEntityErrors);
                if (oEntityErrors.HayError)
                {
                    return oEntityErrors;
                }
                Resultado resultado = cupoManager.ActualizarCupoSAP(cupo);
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
            logger.Debug("Actualizand CUPOSAP:" + JsonConvert.SerializeObject(cupoSAP));
            logger.Debug("Actualizando CUPOSAP RESULTADO:" + JsonConvert.SerializeObject(oEntityErrors));

            oEntityErrors.HayError = oEntityErrors.ListaErrores.Any();
            return oEntityErrors;
        }

        public ResultadoSap AltaCupoSAP(CupoSapDto cupoSAP)
        {
            var oEntityErrors = new ResultadoSap();
            try
            {
                logger.Debug("Alta Cupo" + cupoSAP.ToXml());
                //var cupoOriginal = repositorio.Obtener<Cupo, Cupo>(x => x.CupoSap == cupoSAP.Codigo, x => x);
                //if (cupoOriginal == null || cupoOriginal.Id == 0)
                //{
                //    oEntityErrors.ListaErrores.Add(new ErrorMessage { Source = "Codigo", Message = "No existe cupo " + (cupoSAP.Codigo ?? "") + " en DataAgro" });
                //    return oEntityErrors;
                //}

                var cupo = new Cupo
                {
                    FechaIngreso = DateTime.ParseExact(cupoSAP.FechaIngreso, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    CupoSap = cupoSAP.Codigo,
                    MaterialId = repositorio.Obtener<Material, int>(x => x.Codigo == cupoSAP.Material, x => x.MaterialId)
                };
                string cuit = cupoSAP.Proveedor;
                int ProveedorId = 0;

                if (cuit.StartsWith("C"))
                {
                    cuit = cuit.Replace("C", "");
                    ProveedorId = repositorio.Obtener<Proveedor, int>(x => x.CUIT.Contains(cuit) && (x.SegmentacionId == 5 || x.SegmentacionId == 7), x => x.ProveedorId);
                }
                if (cuit.StartsWith("00"))
                {
                    cuit = cuit.Remove(0, 2);
                    ProveedorId = repositorio.Obtener<Proveedor, int>(x => x.CUIT.Contains(cuit) && x.SegmentacionId != 5 && x.SegmentacionId != 7, x => x.ProveedorId);
                }

                cupo.ProveedorId = ProveedorId;
                cupo.CentroId = repositorio.Obtener<Centro, int>(x => x.CodigoSap == cupoSAP.Planta, x => x.Id);
                cupo.ZonaCupoId = repositorio.Obtener<ZonaCupo, int>(x => x.CodigoSap == cupoSAP.Zona, x => x.Id);
                cupo.Observaciones = cupoSAP.Observaciones;
                cupo.Destinatario = cupoSAP.Destinatario ?? "";
                cupo.FleteProcedencia = cupoSAP.FleteProcedencia == "S";
                cupo.Calidad = cupoSAP.Calidad == "01" ? "Camara" : cupoSAP.Calidad == "03" ? "Fabrica" : "";
                cupo.ComercialId = repositorio.Obtener<Comercial, int>(x => x.IdActiveDirectory == cupoSAP.Comercial || x.IdUsuarioSAP == cupoSAP.Comercial, x => x.ComercialId);
                cupo.EstadoCupoId = 1;

                logger.Debug("Alta CUPOSAP VALIDAR");
                ValidarCupo(cupo, oEntityErrors);
                if (oEntityErrors.HayError)
                {
                    return oEntityErrors;
                }
                Resultado resultado = cupoManager.AltaCupoSAP(cupo);
                oEntityErrors.ListaErrores.AddRange(resultado.Errores);
                oEntityErrors.CupoSapId = cupo != null ? cupo.Id.ToString() : "";

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
            logger.Debug("Alta CUPOSAP:" + JsonConvert.SerializeObject(cupoSAP));
            logger.Debug("Alta CUPOSAP RESULTADO:" + JsonConvert.SerializeObject(oEntityErrors));

            oEntityErrors.HayError = oEntityErrors.ListaErrores.Any();
            return oEntityErrors;
        }

        private void ValidarCupo(Cupo cupo, ResultadoSap oEntityErrors)
        {
            if (cupo.MaterialId == 0)
            {
                oEntityErrors.ListaErrores.Add(new ErrorMessage() { Message = "El campo 'Material' es inválido." });
            }
            if (cupo.ProveedorId == 0)
            {
                oEntityErrors.ListaErrores.Add(new ErrorMessage() { Message = "El campo 'Proveedor' es inválido." });
            }
            if (cupo.CentroId == 0)
            {
                oEntityErrors.ListaErrores.Add(new ErrorMessage() { Message = "El campo 'Planta' es inválido." });
            }
            if (cupo.ZonaCupoId == 0)
            {
                oEntityErrors.ListaErrores.Add(new ErrorMessage() { Message = "El campo 'Zona' es inválido." });
            }
            if (cupo.ComercialId == null || cupo.ComercialId == 0)
            {
                oEntityErrors.ListaErrores.Add(new ErrorMessage() { Message = "El campo 'Comercial' es inválido." });
            }

            oEntityErrors.HayError = oEntityErrors.ListaErrores.Any();
        }

        private void ValidarContrato(Contrato oParam, ResultadoSap oErrorMessages, ContratoSAPDto contratoSAP)
        {
            if (oParam.LocalidadId == 0)
            {
                oErrorMessages.ListaErrores.Add(new ErrorMessage() { Message = "El campo 'Procedencia' es inválido." });
            }
            oErrorMessages.HayError = oErrorMessages.ListaErrores.Any();
            if (oParam.CorredorId != null && oParam.ProveedorId != null && oParam.CorredorId != 0 && oParam.ProveedorId != 0)
            {
                if (!repositorio.Existe<CorredorProveedor>(x => x.CorredorId == oParam.CorredorId && x.ProveedorId == oParam.ProveedorId))
                {
                    repositorio.Agregar(new CorredorProveedor { CorredorId = oParam.CorredorId.Value, ProveedorId = oParam.ProveedorId.Value });

                }
            }

            if (oParam.CondicionalContratoId == null && !string.IsNullOrEmpty(contratoSAP.CondicionalContratoSAP))
            {
                oErrorMessages.ListaErrores.Add(new ErrorMessage() { Message = "El campo 'CondicionalContratoSAP' no es válido. No existe en Data Agro el contrato nro. " + contratoSAP.CondicionalContratoSAP });
            }
        }

        public ResultadoSap AnularContratoSAP(ContratoSAP contratoSAP)
        {
            var oEntityErrors = new ResultadoSap();
            try
            {
                logger.Debug("AnularContrato");
                var resultado = contratoManager.AnularContratoSAP(contratoSAP);
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

        public ResultadoValidarProveedorComercial ValidarProveedorComercial(string cuit, bool? corredor)
        {
            try
            {
                //logger.Debug("ValidarProveedorComercial " + cuit);
                ResultadoValidarProveedorComercial resultado = new ResultadoValidarProveedorComercial();
                Proveedor proveedor = null;
                if (corredor == true)
                {
                    proveedor = repositorio.Listar<Proveedor>(x => x.CUIT == cuit && (x.SegmentacionId == 5 || x.SegmentacionId == 7), 0, "ProveedorId", DirOrden.Asc).FirstOrDefault();
                }
                else
                {
                    proveedor = repositorio.Listar<Proveedor>(x => x.CUIT == cuit && x.SegmentacionId != 5 && x.SegmentacionId != 7, 0, "ProveedorId", DirOrden.Asc).FirstOrDefault();
                }
                if (corredor == null)
                {
                    proveedor = repositorio.Listar<Proveedor>(x => x.CUIT == cuit, 0, "ProveedorId", DirOrden.Asc).FirstOrDefault();
                }

                if (proveedor != null)
                {
                    var existeEnSISA = repositorio.Obtener<SISA>(x => x.CUIT == cuit);
                    resultado.ProveedorOperable = existeEnSISA != null;
                    resultado.ProveedorCBU = existeEnSISA != null ? (existeEnSISA.CBU ?? "") : "";
                    resultado.ProveedorSISAEstadoCuit = existeEnSISA != null ? existeEnSISA.EstadoCuit.ToString() : "";
                    resultado.ProveedorSISASituacionCategoria = existeEnSISA != null ? (existeEnSISA.SituacionCategoria ?? "") : "";
                    resultado.ProveedorSISACodCategoria = existeEnSISA != null ? existeEnSISA.CodCategoria.ToString() : "";

                    resultado.ProveedorMails = new List<string>();
                    if (!string.IsNullOrEmpty(proveedor.Email1))
                        resultado.ProveedorMails.Add(proveedor.Email1);
                    if (!string.IsNullOrEmpty(proveedor.Email2))
                        resultado.ProveedorMails.Add(proveedor.Email2);
                    if (!string.IsNullOrEmpty(proveedor.Email3))
                        resultado.ProveedorMails.Add(proveedor.Email3);
                    if (!string.IsNullOrEmpty(proveedor.Email4))
                        resultado.ProveedorMails.Add(proveedor.Email4);
                    var contactos = repositorio.Listar<ContactoComercial>(x => x.ProveedorId == proveedor.ProveedorId);
                    foreach (var contacto in contactos)
                    {
                        if (!string.IsNullOrEmpty(contacto.Email1))
                            resultado.ProveedorMails.Add(contacto.Email1);
                        if (!string.IsNullOrEmpty(contacto.Email2))
                            resultado.ProveedorMails.Add(contacto.Email2);
                        if (!string.IsNullOrEmpty(contacto.Email3))
                            resultado.ProveedorMails.Add(contacto.Email3);
                    }
                    resultado.ProveedorMails = resultado.ProveedorMails.Distinct().ToList();
                    resultado.ProveedorId = proveedor.ProveedorId;
                    resultado.ProveedorRazonSocial = proveedor.RazonSocial;
                    resultado.ProveedorClasificacion = proveedor.ClasificacionCompraNet == null ? "" : proveedor.ClasificacionCompraNet.Descripcion;
                    Negocio negocio = repositorio.ObtenerMayor<Negocio, DateTime>(x => x.ProveedorId == proveedor.ProveedorId || x.CorredorId == proveedor.ProveedorId, x => x.Fecha);
                    bool compras = repositorio.Existe<CampañaMaterial>(x => x.ProveedorId == proveedor.ProveedorId);
                    if (negocio != null || compras)
                    {
                        resultado.ProveedorOperando = true;
                        resultado.ProveedorUltimaOperacion = negocio == null ? (DateTime?)null : negocio.Fecha;
                    }


                    var provCom = proveedor.ProveedorComercialAsociados.FirstOrDefault();
                    if (provCom != null)
                    {
                        resultado.ComercialApellido = provCom.Comercial.Apellido;
                        resultado.ComercialId = provCom.Comercial.ComercialId;
                        resultado.ComercialNombres = provCom.Comercial.Nombres;
                        try
                        {
                            resultado.ComercialMail = mailManager.GetEmailUserActiveDirectory(provCom.Comercial.IdActiveDirectory);

                        }
                        catch (Exception)
                        {
                        }
                    }
                    else
                    {
                        resultado.ListaErrores.Add(new ErrorMessage("El CUIT no tiene ningún comercial asociado."));
                    }
                }
                else
                {
                    resultado.ListaErrores.Add(new ErrorMessage("No se encontró el CUIT."));
                }


                resultado.HayError = resultado.ListaErrores.Count() > 0;
                //logger.Debug("ValidarProveedorComercial resultado" + resultado.ToJson());

                return resultado;
            }
            catch (Exception e)
            {
                logger.Debug(e);
                throw;
            }

        }

        public bool ProveedorApocrifo(string cuit)
        {
            var oFacacop = repositorio.Existe<FACACOP>(x => x.CUIT == cuit);
            return oFacacop;
        }

        // Quienes usan este servicio? En que casos lo usan? Habría que avisarles que la RFC se modificó y devuelve de acuerdo al tipo de cambio (BLED u otro)
        public decimal TraerTipoDeCambio(DateTime? fecha, string moneda, string typeOfRate)
        {
            if (string.IsNullOrEmpty(moneda))
            {
                return tipoDeCambioAgent.TraerTipoDeCambio(fecha, typeOfRate);
            }
            else
            {
                moneda = moneda.Trim().ToUpper().PadRight(5, ' ');
                return tipoDeCambioAgent.TraerTipoDeCambioMoneda(fecha, moneda, typeOfRate);
            }
        }

        public ResultadoSap ActualizarCesionContratoSAP(string contratoSAP, bool cesion)
        {
            var oEntityErrors = new ResultadoSap();
            try
            {
                logger.Debug("ActualizarCesionContratoSAP" + contratoSAP + " " + cesion.ToString());
                Resultado resultado = contratoManager.ActualizarCesionContratoSAP(contratoSAP, cesion);
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
            logger.Debug("ActualizarCesionContratoSAP RESULTADO:" + JsonConvert.SerializeObject(oEntityErrors));

            oEntityErrors.HayError = oEntityErrors.ListaErrores.Any();
            return oEntityErrors;
        }

        public ResultadoAltaCampoSustentable AltaCampoSustentable(CampoDetalleTerceroDto campo)
        {
            logger.Debug("AltaCampoSustentable: " + campo.ToJson());
            ResultadoAltaCampoSustentable resultado = new ResultadoAltaCampoSustentable();
            try
            {
                var proveedor = repositorio.Obtener<Proveedor>(x => x.CUIT == campo.ProveedorCUIT);
                if (proveedor == null)
                {
                    resultado.Error("ProveedorCUIT", "El proveedor no existe");
                }
                if (!repositorio.Existe<Localidad>(x => x.LocalidadId == campo.LocalidadId))
                {
                    resultado.Error("LocalidadId", "La localidad no existe");
                }

                var campaña = repositorio.Obtener<Campaña>(x => x.Descripcion == campo.Campania);
                if (campaña == null)
                {
                    resultado.Error("Campania", "La campaña no existe");
                }
                if (resultado.HayError)
                {
                    return resultado;
                }

                CampoDetalleTercero campoSave = new CampoDetalleTercero
                {
                    HectareasCultivables = campo.HectareasCultivables,
                    HectareasTotales = campo.HectareasTotales,
                    Rinde = campo.ToneladasAprobadas,
                    KMZfile = "data:application/octet-stream;base64," + (campo.KMZfileBase64 ?? ""), // ej: data:image/jpeg;base64,/9j/4AAQSkZJRgABAQAAA...
                    KMZnombre = @"C:\fakepath\" + (campo.KMZnombre ?? ""),
                    Latitud = campo.Latitud,
                    Longitud = campo.Longitud,
                    LocalidadId = campo.LocalidadId,
                    MaterialId = 3,
                    Nombre = campo.Nombre,
                    ProveedorId = proveedor.ProveedorId,
                    CampañaId = campaña.CampañaId,
                    IdMoa = campo.Id,
                    Estado = campo.Estado,

                };

                resultado = proveedorManager.AltaCampoSustentable(campoSave);


            }
            catch (Exception e)
            {
                logger.Error("Error en AltaCampoSustentable");
                logger.Error(e);
                resultado.Error("Error", e.Message);
            }
            return resultado;
        }

        public List<SISA> BuscarProveedorEnSisa(string cuit)
        {
            try
            {
                logger.Debug("BuscarProveedorEnSisa " + cuit);

                var sisa = repositorio.Listar<SISA>(x => x.CUIT == cuit);


                return sisa;
            }
            catch (Exception e)
            {
                logger.Debug(e);
                throw;
            }

        }

        public ResultadoSap ConfirmarFijacionSAP(string fijacionSAP)
        {
            var oEntityErrors = new ResultadoSap();
            try
            {
                logger.Debug("ConfirmarFijacion" + (fijacionSAP ?? ""));
                Resultado resultado = fijacionDePrecioContratoManager.ConfirmarFijacionSAP(fijacionSAP);
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
            logger.Debug("ConfirmarFijacion RESULTADO:" + JsonConvert.SerializeObject(oEntityErrors));

            oEntityErrors.HayError = oEntityErrors.ListaErrores.Any();
            return oEntityErrors;
        }

        public List<ApoderadoSapDto> ListarApoderadosPorProveedor(string cuit)
        {
            try
            {
                logger.Debug("ListarApoderadosPorProveedor " + cuit);
                var proveedor = repositorio.Obtener<Proveedor>(x => x.CUIT == cuit);
                //var apoderados = repositorio.Listar<ContactoComercial>(x => x.ProveedorId == proveedor.ProveedorId && x.EsApoderado == true);

                List<ApoderadoSapDto> listaApoderadosDto2 = repositorio.Listar<ContactoComercial, ApoderadoSapDto>(x => new ApoderadoSapDto
                {
                    Nombres = x.Nombres,
                    Apellido = x.Apellido,
                    CuitApoderado = x.CuitApoderado,
                    Puesto = x.PuestoApoderado == null ? null : x.PuestoApoderado.Descripcion,
                    FechaDesde = x.FechaDesde.HasValue ? (x.FechaDesde.Value.Year + "-" + ((x.FechaDesde.Value.Month < 10 ? "0" : "") + x.FechaDesde.Value.Month) + "-" + ((x.FechaDesde.Value.Day < 10 ? "0" : "") + x.FechaDesde.Value.Day)) : "",
                    FechaHasta = x.FechaHasta.HasValue ? (x.FechaHasta.Value.Year + "-" + ((x.FechaHasta.Value.Month < 10 ? "0" : "") + x.FechaHasta.Value.Month) + "-" + ((x.FechaHasta.Value.Day < 10 ? "0" : "") + x.FechaHasta.Value.Day)) : "",
                }, x => x.ProveedorId == proveedor.ProveedorId && x.EsApoderado == true).ToList();

                return listaApoderadosDto2;
            }
            catch (Exception e)
            {
                logger.Debug(e);
                throw;
            }
        }

        public CupoSapTerceroDto DatosCupoSap(string cupoSap)
        {
            try
            {
                CupoSapTerceroDto response = cupoManager.DatosCupoSap(cupoSap);

                return response;
            }
            catch (Exception ex)
            {
                logger.Debug("Error en DatosCupoSap: ", ex);
                throw;
            }
        }

        public ResultEstadoProveedores ObtenerEstadoProveedores(List<string> listaCuits)
        {
            var resultado = homeManager.ObtenerEstadoProveedores(string.Join(",", GlobalVariables.EquipoReal), listaCuits);
            return resultado;
        }

        #region MOA_Operaciones
        public InicializarContratoDto InicializarContrato(int? tipoNegocioId)
        {
            var resultado = new InicializarContratoDto
            {
                Datos = contratoManager.TraerDatosCombo(tipoNegocioId)
            };

            return resultado;
        }

        public DatosCompraNetDto ObtenerDatosCompraNet(int id)
        {
            var resultado = contratoManager.TraerDatosCompraNet(id);
            return resultado;
        }

        public List<DatosFijacionDeContratoDto> ObtenerFijacionesAutomaticas(string cuitProveedor, string cuitCorredor, int materialId, string filtro, int fijacionId, bool esVirtual = false)
        {
            var resultado = esVirtual ? fijacionDePrecioContratoManager.TraerDatosFijacionVirtual(cuitProveedor, cuitCorredor, materialId, filtro, fijacionId) :
                fijacionDePrecioContratoManager.TraerDatosFijacion(cuitProveedor, cuitCorredor, materialId, filtro, fijacionId);
            return resultado;
        }

        public AltaTempranaNRCODto ValidarProveedor(int proveedorId)
        {
            var resultado = contratoManager.ValidarProveedor(proveedorId);
            return resultado;
        }

        public List<List<PrecioMoaCompraNetDto>> TraerPrecioMoa(int? tipoNegocioId)
        {
            var grupos = configuracionInternaManager.TraerPrecioCompraNet(tipoNegocioId);

            var resultado = grupos.Select(g => g.ToList()).ToList();

            return resultado;
        }

        public BasicoContrato TraerContratoCompleto(int id, string tipo)
        {
            var copia = (tipo != "acuerdo") ? contratoManager.TraerContrato(id) : contratoManager.TraerContratoAcuerdoACopiar(id);

            if (copia.ContratoId == 0)
            {
                var resultado = new ResultadoDto
                {
                    Ok = false
                };
                resultado.Errores.Add("Solamente se puede utilizar acuerdos con fecha de hoy o del último día hábil anterior.");

                throw new FaultException<ResultadoDto>(resultado, new FaultReason("Error de validación"));
            }

            if (copia.Venta == true && copia.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO)
                copia.Cantidad = Math.Abs(copia.Cantidad);

            return copia;
        }

        public BasicoContrato TraerFijacionCompleto(int id)
        {
            var resultado = fijacionDePrecioContratoManager.TraerFijacion(id);
            return resultado;
        }

        public GrabarContratoResultDto GrabarContratoAPrecio(Contrato contrato)
        {
            var resultado = contratoManager.GrabarContratoAPrecioTercero(contrato);
            GrabarContratoResultDto result2 = new GrabarContratoResultDto();
            result2.ContratoId = resultado.ContratoId;
            resultado.Errores.ForEach(a => result2.Errores.Add(a.Message));
            return result2;
        }

        public GrabarContratoResultDto GrabarContratoAFijar(Contrato contrato)
        {
            var resultado = contratoManager.GrabarContratoAFijarTercero(contrato);
            GrabarContratoResultDto result2 = new GrabarContratoResultDto();
            result2.ContratoId = resultado.ContratoId;
            resultado.Errores.ForEach(a => result2.Errores.Add(a.Message));
            return result2;
        }

        public bool ValidarDirecto(string cuit)
        {
            var resultado = proveedorManager.ValidarDirecto(cuit);
            return resultado;
        }

        public HabilitacionPizarraDto HabilitarPizarra(int material, int tiponegocio)
        {
            var resultado = configuracionInternaManager.HabilitarPizarraExterno(material, tiponegocio);
            return resultado;
        }

        public List<HabilitacionPagoDiferidoDto> TraerPagosDiferido()
        {
            var resultado = configuracionInternaManager.TraerPagosDiferido();
            return resultado;
        }

        public List<HabilitacionCampañaDto> HabilitarCampaña(int material)
        {
            var resultado = configuracionInternaManager.HabilitarCampañaExterno(material);
            return resultado;
        }

        public List<PrecioMoaCompraNetDto> TraerPrecioMoaV2(int material, int tiponegocio)
        {
            var resultado = configuracionInternaManager.TraerPrecioCompraNet(material, tiponegocio);
            return resultado;
        }

        public GrabarContratoResultDto GrabarFijacion(FijacionDePrecioContrato contrato)
        {
            var resultado = fijacionDePrecioContratoManager.GrabarFijacionDePrecioTercero(contrato);
            GrabarContratoResultDto result2 = new GrabarContratoResultDto();
            result2.FijacionDePrecioContratoId = resultado.FijacionDePrecioContratoId;
            resultado.Errores.ForEach(a => result2.Errores.Add(a.Message));
            return result2;
        }

        public List<ContratoCopiar> TraerContratosAcuerdoPorCorredor(int corredorId)
        {
            var resultado = contratoManager.TraerContratosAcuerdoPorCorredor(corredorId);
            return resultado;
        }

        public List<GrabarContratoResultDto> GrabarContratoMasivo(List<BasicoContrato> contratos)
        {
            var resultados = contratoManager.GrabarContratoMasivo(contratos);
            List<GrabarContratoResultDto> resultado2 = new List<GrabarContratoResultDto>();
            foreach (var resultado in resultados)
            {
                GrabarContratoResultDto result2 = new GrabarContratoResultDto();
                result2.ContratoId = resultado.ContratoId;
                resultado.Errores.ForEach(a => result2.Errores.Add(a.Message));
                resultado2.Add(result2);
            }
            return resultado2;
        }

        public ResultadoSap AnularContrato(int negocioId, string MotivoRechazo)
        {
            var resultado = new ResultadoSap();
            try
            {
                var result = contratoManager.AnularContratoCarga(negocioId, MotivoRechazo);
                resultado.ListaErrores.AddRange(result.Errores);
            }
            catch (Exception ex)
            {
                resultado.ListaErrores.Add(new ErrorMessage()
                {
                    Message = ex.Message
                });
            }
            resultado.HayError = resultado.ListaErrores.Any();
            return resultado;
        }

        public ResultadoSap AnularFijacion(int negocioId, string MotivoRechazo)
        {
            var resultado = new ResultadoSap();
            try
            {
                var result = fijacionDePrecioContratoManager.AnularFijacionCarga(negocioId, MotivoRechazo);
                resultado.ListaErrores.AddRange(result.Errores);
            }
            catch (Exception ex)
            {
                resultado.ListaErrores.Add(new ErrorMessage()
                {
                    Message = ex.Message
                });
            }
            resultado.HayError = resultado.ListaErrores.Any();
            return resultado;
        }

        public List<HabilitacionSustentableDto> HabilitarSustentable()
        {
            var resultado = configuracionInternaManager.TraerSustentables();
            return resultado;
        }

        public List<BusquedaHome> BuscarProveedoresConCorredor(string filtroProveedor, string filtro, int? agenteCompraId)
        {
            if (filtro == "")
            {
                return proveedorManager.DevolverProveedores(filtroProveedor, 0, GlobalVariables.Equipo, agenteCompraId);
            }
            else
            {
                return proveedorManager.DevolverProveedoresConCorredor(filtroProveedor, filtro);
            }
        }

        public KendoDataSourceResultDto BuscaDatosTablaContrato(KendoDataSourceRequestDto filtro)
        {
            // Convertir el DTO SOAP → DataSourceRequest de Kendo
            var filtroKendo = MapperToKendoRequest(filtro);

            var equipo = GlobalVariables.EquipoReal;
            var resultado = contratoManager.TraerContratosFiltrados(filtroKendo, equipo);

            return MapperToKendoResult(resultado);
        }

        private DataSourceRequest MapperToKendoRequest(KendoDataSourceRequestDto dto)
        {
            if (dto.Sort == null)
            {
                dto.Sort = new List<KendoSortDto> {
                    new KendoSortDto {
                        Field = "Material",
                        Dir = "desc"
                    }
                };
            }

            return new DataSourceRequest
            {
                Take = dto.Take,
                Skip = dto.Skip,
                Sort = dto.Sort?.Select(s => new Sort
                {
                    Field = s.Field,
                    Dir = s.Dir
                }).ToList(),
                Filter = MapperFilter(dto.Filter)
            };
        }

        private Filter MapperFilter(KendoFilterDto dto)
        {
            if (dto == null) return null;

            return new Filter
            {
                Field = dto.Field,
                Operator = dto.Operator,
                Value = dto.Value, // se parseará después según necesidad
                Logic = dto.Logic,
                Filters = dto.Filters?.Select(MapperFilter).ToList()
            };
        }

        private KendoDataSourceResultDto MapperToKendoResult(DataSourceResult result)
        {
            return new KendoDataSourceResultDto
            {
                Data = result.Data.Cast<BasicoContrato>()
                                  //.Select(MapperBasicoContrato)
                                  .ToList(),

                Total = result.Total,

                AggregatesJson = JsonConvert.SerializeObject(result.Aggregates)
            };
        }

        public BuscarMaterialesDto BuscarMateriales()
        {
            var model = new BuscarMaterialesDto();

            var resultado = materialManager.TraerTodoMaterial();

            if (resultado != null)
            {
                model.Datos = resultado.Material;
            }

            return model;
        }

        public BuscarCentroDto BuscarCentro()
        {
            var model = new BuscarCentroDto();

            var resultado = centroManager.TraerTodoCentro();

            if (resultado != null)
            {
                model.Datos = resultado.Centro;
            }

            return model;
        }

        public List<CampañaDto> BuscarCampana()
        {
            var resultado = campañaManager.TraerTodoCampania();
            return resultado;
        }

        public KendoGridResponseDto<ConfiguracionBolsaDto> ObtenerConfiguracionBolsa()
        {
            var request = new KendoGridMvcRequest
            {
                Take = 1000,
                Skip = 0,
                Page = 1,
                PageSize = 1000
            };

            var resultado = configuracionBolsaManager.TraerTodaConfiguracionBolsa(request);

            return new KendoGridResponseDto<ConfiguracionBolsaDto>
            {
                Data = resultado.Data.ToList(),
                Total = resultado.Total
            };
        }

        public byte[] ExcelModeloAltaMasiva()
        {
            var materiales = materialManager.TraerTodoMaterial().Material;
            var centros = centroManager.TraerTodoCentro().Centro;
            var excelBytes = ExcelReporteCompleto.ExcelModeloAltaMasiva(materiales, centros);
            return excelBytes;
        }

        public List<LocalidadDto> ListarLocalidades()
        {
            var resultado = localidadManager.ListarLocalidadTodas();
            return resultado;
        }

        public List<PartidoDto> ListarPartidos()
        {
            var resultado = localidadManager.ListarPartidos();
            return resultado;
        }


        #endregion MOA_Operaciones
    }
}