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
            var oEntityErrors = new ResultadoSap();
            try
            {
                logger.Debug("ActualizandoContrato" + contratoSAP.ToXml());
                var Id = repositorio.Obtener<Contrato,int>(x=> x.ContratoSAP== contratoSAP .ContratoSAP, x=>x.ContratoId);

                var calidades = new List<Calidad>();
                var calEspecialList = repositorio.Listar<CalidadEspecial>();
                var standardCalidadList = repositorio.Listar<StandardDeCalidad>();

                foreach (var cal in contratoSAP.Calidad)
                {
                    var calidad = new Calidad
                    {
                        ContratoId = Id,
                        CalidadEspecialId = calEspecialList.FirstOrDefault(x => x.CodigoSap == cal.Codigo).Id,
                        StandardDeCalidadId = standardCalidadList.FirstOrDefault(x => x.CodigoSap == cal.Codigo).Id,
                        Valor = cal.Valor,
                        PorcentajeDesde = cal.PorcentajeDesde,
                        PorcentajeHasta = cal.PorcentajeHasta
                    };
                    calidades.Add(calidad);
                }

                var descuentos = new List<DescuentoBonificacion>();
                var tipoDescuentoList = repositorio.Listar<TipoDB>();
                var tipoPeriodoList = repositorio.Listar<TipoPeriodoDB>();
                foreach (var desc in contratoSAP.DescuentoBonificaciones)
                {
                    if (desc.TipoPeriodo != "I"&& desc.TipoDescBon != "B")
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
                }

                var aperturas = new List<AperturaPrecio>();
                var conceptoList = repositorio.Listar<ConceptoAperturaPrecio>();
                foreach (var aper in contratoSAP.Apertura)
                {
                    var apertura = new AperturaPrecio
                    {
                        ContratoId = Id,
                        ConceptoAperturaPrecioId= conceptoList.FirstOrDefault(x => x.CodigoSap == aper.Concepto).Id,
                        Importe =aper.Importe,
                        MonedaId = aper.Moneda,
                        Porcentaje = aper.Porcentaje
                    };
                    aperturas.Add(apertura);
                }
                var porcentaje = contratoSAP.Apertura.Where(x => x.Concepto == "CO" || x.Concepto == "BO" && x.Porcentaje > 0).ToList().Count >0?
                    contratoSAP.Apertura.Where(x => x.Concepto == "CO" || x.Concepto == "BO" && x.Porcentaje > 0).Sum(x => x.Porcentaje * contratoSAP.Precio):0;
                var contrato = new Contrato
                {
                    ContratoSAP = contratoSAP.ContratoSAP.PadLeft(10,'0'),
                    BoletoId = contratoSAP.Confirma == "X" ? 1 : contratoSAP.BolFisico == "X" ? 2 : contratoSAP.CartaOferta == "X" ? 4 : 3,
                    BolsaId = repositorio.Obtener<BolsaCompraNet, int>(x => x.CodigoSap == contratoSAP.Bolsa, x => x.Id),
                    CampanaId = repositorio.Obtener<Campaña, int>(x => x.Descripcion == contratoSAP.Cosecha, x => x.CampañaId),
                    Cantidad = (double)contratoSAP.Cantidad,
                    CantidadCamiones = contratoSAP.Camiones,
                    CD = contratoSAP.AurCd == "X",
                    ClasificacionId = repositorio.Obtener<ClasificacionCompraNet, int>(x => x.Descripcion == contratoSAP.Clasificacion, x => x.Id),
                    ComercialCreadorId = repositorio.Obtener<Comercial, int>(x => x.IdActiveDirectory == contratoSAP.Creador, x => x.ComercialId),
                    ComercialId = repositorio.Obtener<Comercial, int>(x => x.IdActiveDirectory == contratoSAP.Usuario, x => x.ComercialId),
                    Compensacion = contratoSAP.Compensacion == "X",
                    DestinoId = repositorio.Obtener<Centro, int>(x => x.CodigoSap == contratoSAP.Centro, x => x.Id),
                    CondicionFijacionId = !string.IsNullOrEmpty(contratoSAP.CondFijacion) ? repositorio.Obtener<CondicionFijacion, int>(x => x.CodigoSap == contratoSAP.CondFijacion, x => x.Id) : (int?)null,
                    Consignatario = contratoSAP.Consignatario == "X",
                    ContratoCorredor = contratoSAP.ContrCorr,
                    ContratoMadre = contratoSAP.ContratoMadre,
                    ContratoVendedor = contratoSAP.ContrVend,
                    CorredorId = !string.IsNullOrEmpty(contratoSAP.CuitCorredor) ? repositorio.Obtener<CorredorProveedor, int>(x => x.Corredor.CUIT == contratoSAP.CuitCorredor, x => x.CorredorId) : (int?)null,
                    DesdeFijacion = !string.IsNullOrEmpty(contratoSAP.FeDesdeFij) ? DateTime.ParseExact(contratoSAP.FeDesdeFij, "yyyy-MM-dd", CultureInfo.InvariantCulture) : (DateTime?)null,
                    DiasPesificado = contratoSAP.DiasDiferimiento == 0 ? (int?)null : contratoSAP.DiasDiferimiento,
                    Dolarizado = contratoSAP.PagoDiferido == "X",
                    EstablecimientoPropio = contratoSAP.EstabPropio == "X" ? true : contratoSAP.EstabArrendado == "X" ? false : (bool?)null,
                    FechaDolarizado = !string.IsNullOrEmpty(contratoSAP.FechaLimite) ? DateTime.ParseExact(contratoSAP.FechaLimite, "yyyy-MM-dd", CultureInfo.InvariantCulture) : (DateTime?)null,
                    Fecha = DateTime.ParseExact(contratoSAP.Fecha, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    FechaDesde = DateTime.ParseExact(contratoSAP.FechaDesde, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    FechaEntrega = DateTime.ParseExact(contratoSAP.FechaEntrega, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    FechaHasta = DateTime.ParseExact(contratoSAP.FechaHasta, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    HastaFijacion = !string.IsNullOrEmpty(contratoSAP.FeHastaFij) ? DateTime.ParseExact(contratoSAP.FeHastaFij, "yyyy-MM-dd", CultureInfo.InvariantCulture) : (DateTime?)null,
                    ImporteSustentable = contratoSAP.DescuentoBonificaciones.FirstOrDefault(x => x.TipoPeriodo == "I" && x.TipoDescBon == "B") != null ?
                    contratoSAP.DescuentoBonificaciones.FirstOrDefault(x => x.TipoPeriodo == "I" && x.TipoDescBon == "B").Importe : (decimal?)null,
                    LocalidadId = repositorio.Obtener<Localidad, int>(x => x.CodLocalidad == contratoSAP.Procedencia, x => x.LocalidadId),
                    Madre = contratoSAP.TipoNegocio == "MADRE" ? true : contratoSAP.TipoNegocio == "HIJO" ? false : (bool?)null,
                    MaterialId = repositorio.Obtener<Material, int>(x => x.Codigo == contratoSAP.Material, x => x.MaterialId),
                    MercsDeposito = contratoSAP.MercDescargada == "X",
                    MonedaId = repositorio.Obtener<Moneda, string>(x => x.MonedaId == contratoSAP.Moneda, x => x.MonedaId),
                    MonedaSustentableId = contratoSAP.DescuentoBonificaciones.FirstOrDefault(x => x.TipoPeriodo == "I" && x.TipoDescBon == "B") != null ?
                    contratoSAP.DescuentoBonificaciones.FirstOrDefault(x => x.TipoPeriodo == "I" && x.TipoDescBon == "B").MonedaDB : "",
                    NivelTarifaId = !string.IsNullOrEmpty(contratoSAP.FleteNivel) ? repositorio.Obtener<NivelTarifa, int>(x => x.CodigoSap == contratoSAP.FleteNivel, x => x.Id) : (int?)null,
                    NoInformaSio = contratoSAP.NoInformaSio == "X",
                    Observacion = contratoSAP.ObservacionCal1,
                    PagoDiferido = contratoSAP.PagoDiferido == "X",
                    PagoDirectoVendedor = contratoSAP.PagoDirVend == "X",
                    PlanCanje = contratoSAP.IndOpCanje == "X",
                    PorcentajeComision = contratoSAP.PorcComision == 0 ? (decimal?)null : contratoSAP.PorcComision,
                    Precio = contratoSAP.Precio,
                    PrecioNeto = contratoSAP.Precio + contratoSAP.Apertura.Where(x => x.Concepto == "RE" || x.Concepto == "FI" || x.Concepto == "CO" || x.Concepto == "BO").Sum(x => x.Importe)
                    + porcentaje + contratoSAP.FleteTarifa,
                    ProveedorId = repositorio.Obtener<CorredorProveedor, int>(x => x.Proveedor.CUIT == contratoSAP.Proveedor, x => x.ProveedorId),
                    ProvinciaId = contratoSAP.Provincia,
                    SelCargoMOA = contratoSAP.SelCargoMOA == "X",
                    SelCargoVendedor = contratoSAP.SelCargoVend == "X",
                    StandardDeCalidadId = repositorio.Obtener<StandardDeCalidad, int>(x => x.CodigoSap == contratoSAP.Especial, x => x.Id),
                    Sustentable = contratoSAP.Sustentable == "X",
                    TarifaFlete = contratoSAP.FleteTarifa == 0 ? (decimal?)null : contratoSAP.FleteTarifa,
                    TipoNegocioId = contratoSAP.TipoNegocio == "MADRE" ? 1 : contratoSAP.TipoNegocio == "HIJO" ? 2 :
                    repositorio.Obtener<TipoNegocio, int>(x => x.Descripcion == contratoSAP.TipoNegocio, x => x.TipoNegocioId),
                    UsuarioId = contratoSAP.Usuario,
                    Warrant = contratoSAP.AutCg == "X",
                    ZonaId = !string.IsNullOrEmpty(contratoSAP.Zona) ? repositorio.Obtener<Zona, int>(x => x.CodigoSap == contratoSAP.Zona, x => x.Id) : (int?)null,
                    Calidad = calidades,
                    AperturaPrecio = aperturas,
                    Descuentos = descuentos
                };
                var resultado = contratoManager.ActualizarContratoSAP(contrato);
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
        #endregion
    }
}
