using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
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
            var oEntityErrors = new ResultadoSap();
            try
            {
                logger.Debug("ActualizandoContrato" + contratoSAP.ToXml());
                var Id = repositorio.Obtener<Contrato, int>(x => x.ContratoSAP == contratoSAP.ContratoSAP, x => x.Id);

                var calidades = new List<Calidad>();
                var calEspecialList = repositorio.Listar<CalidadEspecial>();
                var standardCalidadList = repositorio.Listar<StandardDeCalidad>();
                logger.Debug("ActualizandoContrato1");

                foreach (var cal in contratoSAP.Calidad ?? new List<CalidadSAP>())
                {
                    var calidad = new Calidad
                    {
                        NegocioId = Id,
                        CalidadEspecialId = calEspecialList.FirstOrDefault(x => x.CodigoSap == cal.Codigo).Id,
                        StandardDeCalidadId = standardCalidadList.FirstOrDefault(x => x.CodigoSap == cal.Codigo).Id,
                        Valor = cal.Valor,
                        PorcentajeDesde = cal.PorcentajeDesde,
                        PorcentajeHasta = cal.PorcentajeHasta
                    };
                    calidades.Add(calidad);
                }

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
                            ContratoId = Id,
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
                            ContratoId = Id,
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
                foreach (var aper in contratoSAP.Apertura ?? new List<AperturaPrecioSap>())
                {
                    var apertura = new AperturaPrecio
                    {
                        NegocioId = Id,
                        ConceptoAperturaPrecioId = conceptoList.FirstOrDefault(x => x.CodigoSap == aper.Concepto).Id,
                        Importe = aper.Importe,
                        MonedaId = aper.Moneda,
                        Porcentaje = aper.Porcentaje
                    };
                    aperturas.Add(apertura);
                }
                logger.Debug("ActualizandoContrato4");

                var precioOriginal = contratoSAP.Precio;
                var porcentajeComision = contratoSAP.Apertura.Where(x => (x.Concepto == "CO") && x.Porcentaje > 0).Sum(x => x.Porcentaje);
                var precioTarifaFlete = contratoSAP.FleteTarifa;
                precioOriginal += contratoSAP.Apertura.Where(x => x.Concepto == "FI").Sum(x => x.Importe);
                precioOriginal += contratoSAP.Apertura.Where(x => x.Concepto == "RE").Sum(x => x.Importe);
                var maximoBonificacion = (contratoSAP.Precio + contratoSAP.Apertura.Where(x => x.Concepto == "FI").Sum(x => x.Importe)+ contratoSAP.Apertura.Where(x => x.Concepto == "RE").Sum(x => x.Importe) )* (decimal)0.01;
                var aperturaPrecioImporteComisiones = contratoSAP.Apertura.Where(x => x.Concepto == "CO").Sum(x => x.Importe);
                if (aperturaPrecioImporteComisiones > maximoBonificacion)
                {
                    aperturaPrecioImporteComisiones = maximoBonificacion;
                }
                precioOriginal += contratoSAP.Apertura.Where(x => x.Concepto == "BO").Sum(x => x.Importe);

                precioOriginal += contratoSAP.Apertura.Where(x => x.Concepto == "BO").Sum(x => x.Importe)* contratoSAP.Precio/100;
                porcentajeComision = porcentajeComision / 100;
                precioOriginal += (precioOriginal * porcentajeComision) - precioTarifaFlete;
                precioOriginal += aperturaPrecioImporteComisiones;

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
                contrato.ImporteSustentable = contratoSAP.DescuentoBonificaciones.FirstOrDefault(x => x.TipoPeriodo == "I" && x.TipoDescBon == "B") != null ?
                contratoSAP.DescuentoBonificaciones.FirstOrDefault(x => x.TipoPeriodo == "I" && x.TipoDescBon == "B").Importe : (decimal?)null;
                contrato.LocalidadId = repositorio.Obtener<Localidad, int>(x => x.CodLocalidad == contratoSAP.Procedencia, x => x.LocalidadId);
                contrato.MaterialId = repositorio.Obtener<Material, int>(x => x.Codigo == contratoSAP.Material, x => x.MaterialId);
                contrato.MercsDeposito = contratoSAP.MercDescargada == "X";
                contrato.MonedaId = repositorio.Obtener<Moneda, string>(x => x.MonedaId == contratoSAP.Moneda, x => x.MonedaId);
                contrato.MonedaSustentableId = contratoSAP.DescuentoBonificaciones.FirstOrDefault(x => x.TipoPeriodo == "I" && x.TipoDescBon == "B") != null ?
                contratoSAP.DescuentoBonificaciones.FirstOrDefault(x => x.TipoPeriodo == "I" && x.TipoDescBon == "B").MonedaDB : "";
                contrato.NivelTarifaId = !string.IsNullOrEmpty(contratoSAP.FleteNivel) ? repositorio.Obtener<NivelTarifa, int>(x => x.CodigoSap == contratoSAP.FleteNivel, x => x.Id) : (int?)null;
                contrato.NoInformaSio = contratoSAP.NoInformaSio == "X";
                contrato.Observacion = contratoSAP.ObservacionCal1;
                contrato.PagoDirectoVendedor = contratoSAP.PagoDirVend == "X";
                contrato.PlanCanje = contratoSAP.IndOpCanje == "X";
                contrato.PorcentajeComision = contratoSAP.PorcComision == 0 ? (decimal?)null : contratoSAP.PorcComision;
                contrato.Precio = contratoSAP.Precio;
                contrato.PrecioNeto = precioOriginal;
                contrato.ProveedorId = repositorio.Obtener<Proveedor, int>(x => x.CUIT == contratoSAP.Proveedor, x => x.ProveedorId);
                contrato.ProvinciaId = contratoSAP.Provincia;
                contrato.SelCargoMOA = contratoSAP.SelCargoMOA == "X";
                contrato.SelCargoVendedor = contratoSAP.SelCargoVend == "X";
                //contratoSAP.Especial = contratoSAP.Especial.Replace("0", "");
                contrato.StandardDeCalidadId = repositorio.Obtener<StandardDeCalidad, int>(x => contratoSAP.Especial.Contains(x.CodigoSap), x => x.Id);
                contrato.Sustentable = contratoSAP.Sustentable == "X";
                contrato.TarifaFlete = contratoSAP.FleteTarifa == 0 ? (decimal?)null : contratoSAP.FleteTarifa;
                contrato.Warrant = contratoSAP.AutCg == "X";
                contrato.ZonaId = !string.IsNullOrEmpty(contratoSAP.Zona) ? repositorio.Obtener<Zona, int>(x => x.CodigoSap == contratoSAP.Zona, x => x.Id) : (int?)null;
                contrato.Calidad = calidades;
                contrato.AperturaPrecio = aperturas;
                contrato.Descuentos = descuentos;
                contrato.TipoNegocioId = 3;
                contrato.ComercialId = 1;
                contrato.Fecha = DateTime.Now;
                logger.Debug("ActualizandoContrato5");

                var resultado = contratoManager.ActualizarContratoSAP(contrato);
                oEntityErrors.ListaErrores.AddRange(resultado.Errores);
            }
            catch (Exception ex)
            {
                logger.Error(ex);

                oEntityErrors.ListaErrores.Add(new ErrorMessage()
                {
                    Message = ex.Message
                });
            }
            logger.Debug("ActualizandoContrato7");

            oEntityErrors.HayError = oEntityErrors.ListaErrores.Any();
            return oEntityErrors;
        }
        #endregion
    }
}
