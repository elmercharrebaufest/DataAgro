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
        private readonly ICupoManager cupoManager;
        private readonly IMailManager mailManager;
        private readonly IFijacionDePrecioContratoManager fijacionDePrecioContratoManager;

        public DataAgroServices(ILogger logger,
            IRiesgoComercialManager riesgoComercial,
            ICampaniaActualManager campanaActual,
            ICampaniaMaterialManager campaniaMaterial,
            IInformeComercialManager informeComercial,
            IContratoManager contratoManager,
            IRepositorio repositorio,
            ICupoManager cupoManager,
            IMailManager mailManager, IFijacionDePrecioContratoManager fijacionDePrecioContratoManager)
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
                var contratoOriginal = repositorio.Obtener<Contrato>(x => x.ContratoSAP == contratoSAP.ContratoSAP);
                var contrato = new Contrato();
                CrearProyeccionContrato(contratoSAP, contrato, null, true);
                logger.Debug("ActualizandoContrato5");
                ValidarContrato(contrato, oEntityErrors);
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

        public ResultadoSap AltaContratoSAP(ContratoSAPDto contratoSAP)
        {
            contratoSAP.Calidad = contratoSAP.Calidad ?? new List<CalidadSAP>();
            contratoSAP.DescuentoBonificaciones = contratoSAP.DescuentoBonificaciones ?? new List<DescuentoBonificacionSap>();
            contratoSAP.Apertura = contratoSAP.Apertura ?? new List<AperturaPrecioSap>();
            contratoSAP.Procedencia = contratoSAP.Procedencia != null ? contratoSAP.Procedencia.Trim() : contratoSAP.Procedencia;
            var oEntityErrors = new ResultadoSap();
            try
            {
                logger.Debug("Alta ContratoSap" + contratoSAP.ToXml());
                var c = contratoSAP.ContratoSAP.PadLeft(10, '0');
                if (repositorio.Existe<Contrato>(x => x.ContratoSAP == c))
                {
                    oEntityErrors.ListaErrores.Add(new ErrorMessage("Contrato", "El contrato ya existe en DataAgro"));
                    return oEntityErrors;
                }
                var contrato = new Contrato();

                CrearProyeccionContrato(contratoSAP, contrato, null, false);

                logger.Debug("Validacion alta contrato");
                ValidarContrato(contrato, oEntityErrors);
                if (oEntityErrors.HayError)
                {
                    return oEntityErrors;
                }

                var resultado = contratoManager.AltaContratoSAP(contrato, true);
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
            logger.Debug("Alta CONTRATOSAP:" + JsonConvert.SerializeObject(contratoSAP));
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
            logger.Debug("Alta de descuentos");

            var descuentos = new List<DescuentoBonificacion>();
            var preciosPactados = new List<PrecioPactado>();
            var tipoDescuentoList = repositorio.Listar<TipoDB>();
            var tipoPeriodoList = repositorio.Listar<TipoPeriodoDB>();

            foreach (var desc in contratoSAP.DescuentoBonificaciones ?? new List<DescuentoBonificacionSap>())
            {
                if (!(desc.TipoPeriodo == "I" && desc.TipoDescBon == "B") && !(desc.TipoPeriodo == "E" && desc.TipoDescBon == "A"))
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
                if (desc.TipoPeriodo == "E" && desc.TipoDescBon == "A")
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

            logger.Debug("Alta contrato Apertura");

            var aperturas = new List<AperturaPrecio>();
            var conceptoList = repositorio.Listar<ConceptoAperturaPrecio>();
            if (contratoSAP.Apertura != null && contratoSAP.Apertura.Count > 0)
            {
                aperturas.Add(new AperturaPrecio { NegocioId = esActualizar ? contratoOriginal.Id : 0, ConceptoAperturaPrecioId = 1, Importe = 0, Porcentaje = 0 });
                aperturas.Add(new AperturaPrecio { NegocioId = esActualizar ? contratoOriginal.Id : 0, ConceptoAperturaPrecioId = 2, Importe = 0, Porcentaje = 0 });
                aperturas.Add(new AperturaPrecio { NegocioId = esActualizar ? contratoOriginal.Id : 0, ConceptoAperturaPrecioId = 3, Importe = 0, Porcentaje = 0 });
                aperturas.Add(new AperturaPrecio { NegocioId = esActualizar ? contratoOriginal.Id : 0, ConceptoAperturaPrecioId = 4, Importe = 0, Porcentaje = 0 });
            }
            foreach (var aper in contratoSAP.Apertura ?? new List<AperturaPrecioSap>())
            {
                var ConceptoAperturaPrecioId = conceptoList.FirstOrDefault(x => x.CodigoSap == aper.Concepto).Id;

                aperturas.Where(a => a.ConceptoAperturaPrecioId == ConceptoAperturaPrecioId).Single().Importe = aper.Importe;
                aperturas.Where(a => a.ConceptoAperturaPrecioId == ConceptoAperturaPrecioId).Single().MonedaId = aper.Moneda;
                aperturas.Where(a => a.ConceptoAperturaPrecioId == ConceptoAperturaPrecioId).Single().Porcentaje = aper.Porcentaje;
            }
            if (esActualizar)
            {
                if (contratoOriginal.AperturaPrecio.Count() == 0 && !aperturas.Any(a => a.Importe != 0 || a.Porcentaje != 0))
                {
                    aperturas = new List<AperturaPrecio>();
                }
            }
            logger.Debug("Alta de datos contrato");


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
            //contrato.ContratoCorredor = String.IsNullOrEmpty(contratoSAP.ContratoCorredor) ? null : contratoSAP.ContratoCorredor;
            contrato.ContratoMadre = contratoSAP.ContratoMadre;
            contrato.ContratoVendedor = contratoSAP.ContrVend;
            contrato.CorredorId = !string.IsNullOrEmpty(contratoSAP.CuitCorredor) ? repositorio.Obtener<CorredorProveedor, int>(x => x.Corredor.CUIT == contratoSAP.CuitCorredor, x => x.CorredorId) : (int?)null;
            contrato.DesdeFijacion = !string.IsNullOrEmpty(contratoSAP.FeDesdeFij) ? DateTime.ParseExact(contratoSAP.FeDesdeFij, "yyyy-MM-dd", CultureInfo.InvariantCulture) : (DateTime?)null;
            contrato.DiasPesificado = contratoSAP.DiasDiferimiento == 0 ? (int?)null : contratoSAP.DiasDiferimiento;
            contrato.PagoDiferido = contratoSAP.DiasDiferimiento > 0;
            contrato.Dolarizado = contratoSAP.DolarizadoExpress == "X" ? false : !string.IsNullOrEmpty(contratoSAP.FechaLimite);
            contrato.EstablecimientoPropio = contratoSAP.EstabPropio == "X" ? true : contratoSAP.EstabArrendado == "X" ? false : (bool?)null;
            contrato.FechaDolarizado = !string.IsNullOrEmpty(contratoSAP.FechaLimite) ? DateTime.ParseExact(contratoSAP.FechaLimite, "yyyy-MM-dd", CultureInfo.InvariantCulture) : (DateTime?)null;
            contrato.FechaDesde = DateTime.ParseExact(contratoSAP.FechaDesde, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            contrato.FechaEntrega = DateTime.ParseExact(contratoSAP.FechaEntrega, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            contrato.FechaHasta = DateTime.ParseExact(contratoSAP.FechaHasta, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            contrato.HastaFijacion = !string.IsNullOrEmpty(contratoSAP.FeHastaFij) ? DateTime.ParseExact(contratoSAP.FeHastaFij, "yyyy-MM-dd", CultureInfo.InvariantCulture) : (DateTime?)null;
            if (!esActualizar)
            {
                contrato.FechaOperacion = DateTime.ParseExact(contratoSAP.FechaOperacion, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            contrato.FechaCierta = !string.IsNullOrEmpty(contratoSAP.FechaCierta) ? DateTime.ParseExact(contratoSAP.FechaCierta, "yyyy-MM-dd", CultureInfo.InvariantCulture) : (DateTime?)null;
            if (!esActualizar)
            {
                var hora = DateTime.ParseExact(contratoSAP.HORAACT, "HH:mm:ss", CultureInfo.InvariantCulture);
                TimeSpan time = new TimeSpan(hora.Hour, hora.Minute, hora.Second);
                contrato.Fecha = DateTime.ParseExact(contratoSAP.FechaCreacion, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                contrato.Fecha = contrato.Fecha.Add(time);
            }
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
                contrato.EsFason = contratoSAP.TipoNegocio == "FASON" ? true : (bool?)null;
                contrato.Madre = contratoSAP.TipoNegocio == "MADRE" ? true : contratoSAP.TipoNegocio == "HIJO" ? false : (bool?)null;
                contrato.TipoNegocioId = contratoSAP.TipoNegocio == "HIJO" ? 2 :
                    contratoSAP.TipoNegocio == "MADRE" ? 1 : contratoSAP.TipoNegocio == "FASON" ? 1 :
                    repositorio.Obtener<TipoNegocio, int>(x => x.Descripcion == contratoSAP.TipoNegocio, x => x.TipoNegocioId);
            }
            if (contratoSAP.Especial == "03" && contrato.MaterialId == 3)
            {
                contrato.StandardDeCalidadId = 3;
            }
            if (contratoSAP.Especial == "04" && contrato.MaterialId == 1)
            {
                contrato.StandardDeCalidadId = 7;
            }
            contrato.Sustentable = contratoSAP.Sustentable == "X";
            contrato.TarifaFlete = contratoSAP.FleteTarifa == 0 ? (decimal?)null : contratoSAP.FleteTarifa;
            contrato.Warrant = contratoSAP.AutCg == "X";

            contrato.ZonaId = !string.IsNullOrEmpty(contratoSAP.Zona) ? repositorio.Obtener<Zona, int>(x => x.CodigoSap == contratoSAP.Zona, x => x.Id) : contratoOriginal.ZonaId;

            contrato.PorcentajeDePago = contratoSAP.PorcentajeDePago ?? contratoOriginal.PorcentajeDePago ?? (decimal)97.5;


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
                            if (contratoOriginal.Calidad.Where(a => a.CalidadEspecialId == calidad.CalidadEspecialId).FirstOrDefault() != null)
                            {
                                calidad.PorcentajeDesde = contratoOriginal.Calidad.Where(a => a.CalidadEspecialId == calidad.CalidadEspecialId).FirstOrDefault().PorcentajeDesde;
                                calidad.PorcentajeHasta = contratoOriginal.Calidad.Where(a => a.CalidadEspecialId == calidad.CalidadEspecialId).FirstOrDefault().PorcentajeHasta;
                                calidad.Valor = contratoOriginal.Calidad.Where(a => a.CalidadEspecialId == calidad.CalidadEspecialId).FirstOrDefault().Valor;
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
                        if (contratoOriginal.Calidad.Where(a => a.CalidadEspecialId == calidad.CalidadEspecialId).FirstOrDefault() != null)
                        {
                            calidad.PorcentajeDesde = contratoOriginal.Calidad.Where(a => a.StandardDeCalidadId == calidad.StandardDeCalidadId).FirstOrDefault().PorcentajeDesde;
                            calidad.PorcentajeHasta = contratoOriginal.Calidad.Where(a => a.StandardDeCalidadId == calidad.StandardDeCalidadId).FirstOrDefault().PorcentajeHasta;
                            calidad.Valor = contratoOriginal.Calidad.Where(a => a.StandardDeCalidadId == calidad.StandardDeCalidadId).FirstOrDefault().Valor;
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
                var comercial = repositorio.Obtener<Comercial>(x => x.IdActiveDirectory == contratoSAP.Comercial);
                contrato.GrupoCompra = comercial.GrupoDeComprasId;
                contrato.ComercialId = comercial.ComercialId;
                contrato.UsuarioId = contratoSAP.Comercial;
                contrato.ComercialCreadorId = repositorio.Obtener<Comercial, int>(x => x.IdActiveDirectory == contratoSAP.ComercialCreador, x => x.ComercialId);
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
        }

        public ResultadoSap ActualizarFijacionSAP(FijacionSAPDto fijacionSAP)
        {
            var oEntityErrors = new ResultadoSap();
            try
            {
                logger.Debug("ActualizandoFijacion" + fijacionSAP.ToXml());
                var fijacion = new FijacionDePrecioContrato();
                fijacion.FijacionSAP = fijacionSAP.FijacionSAP;
                fijacion.ChequeElectronico = fijacionSAP.ZLSCH == "=";
                fijacion.PagoCBU = fijacionSAP.CUENTA_MRP;
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
                var f = fijacionSAP.FijacionSAP.PadLeft(10, '0');
                //if (repositorio.Existe<FijacionDePrecioContrato>(x => x.FijacionSAP == f)) {
                //    oEntityErrors.ListaErrores.Add( new ErrorMessage("Fijacion", "la fijacion ya existe en DataAgro"));
                //    return oEntityErrors;
                //}
                var fijarId = repositorio.Obtener<Contrato, int>(x => x.ContratoSAP == f, x => x.Id);

                var fijacion = new FijacionDePrecioContrato();
                fijacion.FijacionSAP = fijacionSAP.FijacionSAP;
                fijacion.ChequeElectronico = fijacionSAP.ZLSCH == "=";
                fijacion.TrigoEspecial = fijacionSAP.TrigoEspecial == "X";
                fijacion.PagoCBU = fijacionSAP.CUENTA_MRP;
                fijacion.ContratoSAP = fijacionSAP.ContratoSAP.PadLeft(10, '0');
                fijacion.ContratoId = fijarId != 0 ? fijarId : (int?)null;
                fijacion.Precio = fijacionSAP.Precio;
                fijacion.Cantidad = (double)fijacionSAP.Cantidad;
                fijacion.ComercialId = repositorio.Obtener<Comercial, int>(x => x.IdActiveDirectory == fijacionSAP.Comercial, x => x.ComercialId);
                fijacion.DestinoId = repositorio.Obtener<Centro, int>(x => x.CodigoSap == fijacionSAP.Centro, x => x.Id);
                logger.Debug("Validacion Precio " + (fijacionSAP.Precio == 0));
                fijacion.Pizarra = fijacionSAP.Pizarra == "X";
                fijacion.Posicion = fijacionSAP.Posicion;
                fijacion.CorredorId = !string.IsNullOrEmpty(fijacionSAP.CuitCorredor) ? repositorio.Obtener<CorredorProveedor, int>(x => x.Corredor.CUIT == fijacionSAP.CuitCorredor, x => x.CorredorId) : (int?)null;
                fijacion.DiasPesificado = fijacionSAP.DiasDiferimiento == 0 ? (int?)null : fijacionSAP.DiasDiferimiento;
                fijacion.PagoDiferido = fijacionSAP.DiasDiferimiento > 0;
                fijacion.FechaOperacion = DateTime.ParseExact(fijacionSAP.FechaOperacion, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                fijacion.FechaHasta = DateTime.ParseExact(fijacionSAP.FechaHasta, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                fijacion.FechaDesde = DateTime.ParseExact(fijacionSAP.FechaDesde, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                fijacion.MaterialId = repositorio.Obtener<Material, int>(x => x.Codigo == fijacionSAP.Material, x => x.MaterialId);
                fijacion.MonedaId = repositorio.Obtener<Moneda, string>(x => x.MonedaId == fijacionSAP.Moneda, x => x.MonedaId);
                fijacion.CampanaId = repositorio.Obtener<Campaña, int>(x => x.Descripcion == fijacionSAP.Cosecha, x => x.CampañaId);
                fijacion.PrecioNeto = fijacionSAP.PrecioNeto;
                fijacion.ProveedorId = repositorio.Obtener<Proveedor, int>(x => x.CUIT == fijacionSAP.Proveedor && x.SegmentacionId != 5 && x.SegmentacionId != 7, x => x.ProveedorId);
                fijacion.EstadoId = 5;
                fijacion.TipoNegocioId = 3;
                var comercial = repositorio.Obtener<Comercial>(x => x.IdActiveDirectory == fijacionSAP.Comercial);
                fijacion.ComercialId = comercial.ComercialId;
                fijacion.ComercialCreadorId = repositorio.Obtener<Comercial, int>(x => x.IdActiveDirectory == fijacionSAP.Comercial, x => x.ComercialId);
                fijacion.GrupoCompra = comercial.GrupoDeComprasId;
                var hora = DateTime.ParseExact(fijacionSAP.HORAACT, "HH:mm:ss", CultureInfo.InvariantCulture);
                TimeSpan time = new TimeSpan(hora.Hour, hora.Minute, hora.Second);
                fijacion.Fecha = DateTime.ParseExact(fijacionSAP.FechaCreacion, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                fijacion.Fecha = fijacion.Fecha.Add(time);
                fijacion.Canje = fijacionSAP.Canje == "X" ? true : false;
                logger.Debug("Alta fijacion Apertura");

                var aperturas = new List<AperturaPrecio>();
                var conceptoList = repositorio.Listar<ConceptoAperturaPrecio>();
                if (fijacionSAP.Apertura != null && fijacionSAP.Apertura.Count > 0)
                {
                    aperturas.Add(new AperturaPrecio { ConceptoAperturaPrecioId = 1, Importe = 0, Porcentaje = 0 });
                    aperturas.Add(new AperturaPrecio { ConceptoAperturaPrecioId = 2, Importe = 0, Porcentaje = 0 });
                    aperturas.Add(new AperturaPrecio { ConceptoAperturaPrecioId = 3, Importe = 0, Porcentaje = 0 });
                    aperturas.Add(new AperturaPrecio { ConceptoAperturaPrecioId = 4, Importe = 0, Porcentaje = 0 });
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
                var resultado = fijacionDePrecioContratoManager.AltaFijacionSap(fijacion);
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
                logger.Debug("AnularFijacion");
                var resultado = fijacionDePrecioContratoManager.AnularFijacionSAP(fijacionSAP);
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

                var cupo = new Cupo();
                cupo.Id = cupoOriginal.Id;
                cupo.FechaIngreso = DateTime.ParseExact(cupoSAP.FechaIngreso, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                cupo.CupoSap = cupoSAP.Codigo;
                cupo.MaterialId = repositorio.Obtener<Material, int>(x => x.Codigo == cupoSAP.Material, x => x.MaterialId);
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

                var cupo = new Cupo();
                cupo.FechaIngreso = DateTime.ParseExact(cupoSAP.FechaIngreso, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                cupo.CupoSap = cupoSAP.Codigo;
                cupo.MaterialId = repositorio.Obtener<Material, int>(x => x.Codigo == cupoSAP.Material, x => x.MaterialId);
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
                cupo.ComercialId = repositorio.Obtener<Comercial, int>(x => x.IdActiveDirectory == cupoSAP.Comercial, x => x.ComercialId);
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
                oEntityErrors.ListaErrores.Add(new ErrorMessage() { Message = "El campo 'Material' es invalido" });
            }
            if (cupo.ProveedorId == 0)
            {
                oEntityErrors.ListaErrores.Add(new ErrorMessage() { Message = "El campo 'Proveedor' es invalido" });
            }
            if (cupo.CentroId == 0)
            {
                oEntityErrors.ListaErrores.Add(new ErrorMessage() { Message = "El campo 'Planta' es invalido" });
            }
            if (cupo.ZonaCupoId == 0)
            {
                oEntityErrors.ListaErrores.Add(new ErrorMessage() { Message = "El campo 'Zona' es invalido" });
            }
            //if (cupo.ComercialId == null || cupo.ComercialId == 0)
            //{
            //    oEntityErrors.ListaErrores.Add(new ErrorMessage() { Message = "El campo 'Comercial' es invalido" });
            //}

            oEntityErrors.HayError = oEntityErrors.ListaErrores.Any();
        }

        private void ValidarContrato(Contrato oParam, ResultadoSap oErrorMessages)
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

        public ResultadoValidarProveedorComercial ValidarProveedorComercial(string cuit)
        {
            ResultadoValidarProveedorComercial resultado = new ResultadoValidarProveedorComercial();
            var proveedor = repositorio.Obtener<Proveedor>(x => x.CUIT == cuit);
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
                Negocio negocio = repositorio.ObtenerMayor<Negocio, DateTime>(x => x.ProveedorId == proveedor.ProveedorId, x => x.Fecha);
                if (negocio != null)
                {
                    resultado.ProveedorOperando = true;
                    resultado.ProveedorUltimaOperacion = negocio.Fecha;
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
                    resultado.ListaErrores.Add(new ErrorMessage("El cuit no tiene ninguno comercial asociado"));
                }
            }
            else
            {
                resultado.ListaErrores.Add(new ErrorMessage("No se encontro el cuit"));
            }


            resultado.HayError = resultado.ListaErrores.Count() > 0;
            return resultado;
        }

        #endregion
    }


}
