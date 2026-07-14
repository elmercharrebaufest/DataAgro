using NLog;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Dto.ControlDeBoletos;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Interfaces.Managers;
using Molinos.DataAgro.Repository;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Script.Serialization;
using WebDataAgro.Services;

namespace Molinos.DataAgro.Test.Services
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class DataAgroServicesTest
    {
        private DataAgroServices target;
        private Mock<ILogger> loggerMock;
        private Mock<IRiesgoComercialManager> riesgoComercialManagerMock;
        private Mock<ICampaniaActualManager> campaniaAcutalManagerMock;
        private Mock<ICampaniaMaterialManager> camaniaMaterialManagerMock;
        private Mock<IInformeComercialManager> informeComercialManagerMock;
        private Mock<IContratoManager> contratoManagerMock;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ICupoManager> cupoManagerMock;
        private Mock<IMailManager> mailManagerMock;
        private Mock<IFijacionDePrecioContratoManager> fijacionManager;
        private Mock<ITipoDeCambioAgent> tipoDeCambioAgent;
        private Mock<IProveedorManager> proveedorManager;
        private Mock<IHomeManager> homeManager;
        private Mock<IConfiguracionInternaManager> configuracionInternaManager;
        private Mock<IMaterialManager> materialManager;
        private Mock<ICentroManager> centroManager;
        private Mock<ICampañaManager> campañaManager;
        private Mock<IConfiguracionBolsaManager> configuracionBolsaManager;
        private Mock<ILocalidadManager> localidadManager;
        private Mock<IFechaFeriadoManager> fechaFeriadoManager;
        private Mock<IReportesManager> reportesManager;
        private Mock<ICartaDePresentacionManager> cartaDePresentacionManager;
        private Mock<IControlDeBoletosSAPManager> controlDeBoletosSapManager;
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            loggerMock = new Mock<ILogger>();
            riesgoComercialManagerMock = new Mock<IRiesgoComercialManager>();
            campaniaAcutalManagerMock = new Mock<ICampaniaActualManager>();
            camaniaMaterialManagerMock = new Mock<ICampaniaMaterialManager>();
            informeComercialManagerMock = new Mock<IInformeComercialManager>();
            contratoManagerMock = new Mock<IContratoManager>();
            repositorioMock = new Mock<IRepositorio>();
            cupoManagerMock = new Mock<ICupoManager>();
            mailManagerMock = new Mock<IMailManager>();
            fijacionManager = new Mock<IFijacionDePrecioContratoManager>();
            tipoDeCambioAgent = new Mock<ITipoDeCambioAgent>();
            proveedorManager = new Mock<IProveedorManager>();
            homeManager = new Mock<IHomeManager>();
            configuracionInternaManager = new Mock<IConfiguracionInternaManager>();
            materialManager = new Mock<IMaterialManager>();
            centroManager = new Mock<ICentroManager>();
            campañaManager = new Mock<ICampañaManager>();
            configuracionBolsaManager = new Mock<IConfiguracionBolsaManager>();
            localidadManager = new Mock<ILocalidadManager>();
            fechaFeriadoManager = new Mock<IFechaFeriadoManager>();
            reportesManager = new Mock<IReportesManager>();
            cartaDePresentacionManager = new Mock<ICartaDePresentacionManager>();
            controlDeBoletosSapManager = new Mock<IControlDeBoletosSAPManager>();

            HttpContext.Current = Mock.FakeContext.FakeHttpContext();

            target = new DataAgroServices(loggerMock.Object, riesgoComercialManagerMock.Object, campaniaAcutalManagerMock.Object,
                camaniaMaterialManagerMock.Object, informeComercialManagerMock.Object, contratoManagerMock.Object, repositorioMock.Object, 
                cupoManagerMock.Object, mailManagerMock.Object, fijacionManager.Object, tipoDeCambioAgent.Object, proveedorManager.Object, 
                homeManager.Object, configuracionInternaManager.Object, materialManager.Object, centroManager.Object, campañaManager.Object,
                configuracionBolsaManager.Object, localidadManager.Object, fechaFeriadoManager.Object, reportesManager.Object, cartaDePresentacionManager.Object,
                controlDeBoletosSapManager.Object);

            HttpContext.Current.Session["perfil"] = 1;
            HttpContext.Current.Session["comercialId"] = 1;
        }

        #region Ping Tests
        [Test]
        public void Ping_NoParameters_ReturnsResultadoSap()
        {
            var result = target.Ping();
            Assert.NotNull(result);
            Assert.IsFalse(result.HayError);
        }
        #endregion

        #region Riesgo Comercial Tests
        [Test]
        public void GrabarRiesgoComercial_ValidObject_ReturnsOk()
        {
            RiesgoComercial riesgoComercial = new RiesgoComercial();
            riesgoComercialManagerMock.Setup(x => x.ActualizacionDeRiesgoComercial(It.IsAny<RiesgoComercial>()))
                .Returns(new Resultado());

            var result = target.GrabarRiesgoComercial(riesgoComercial);
            Assert.NotNull(result);
            Assert.IsFalse(result.HayError);
            Assert.AreEqual(result.ListaErrores.Count, 0);
        }

        [Test]
        public void GrabarRiesgoComercial_ValidObject_ReturnsError()
        {
            RiesgoComercial riesgoComercial = new RiesgoComercial();
            riesgoComercialManagerMock.Setup(x => x.ActualizacionDeRiesgoComercial(It.IsAny<RiesgoComercial>()))
                .Returns(new Resultado { Errores = new List<ErrorMessage> { new ErrorMessage { Source = "", Message = "" } } });

            var result = target.GrabarRiesgoComercial(riesgoComercial);
            Assert.NotNull(result);
            Assert.IsTrue(result.HayError);
            Assert.AreEqual(result.ListaErrores.Count, 1);
        }

        [Test]
        public void GrabarRiesgoComercial_ThrowsException_CatchesAndReturnsError()
        {
            RiesgoComercial riesgoComercial = new RiesgoComercial();
            riesgoComercialManagerMock.Setup(x => x.ActualizacionDeRiesgoComercial(It.IsAny<RiesgoComercial>()))
                .Throws(new Exception("Test exception"));

            var result = target.GrabarRiesgoComercial(riesgoComercial);
            Assert.NotNull(result);
            Assert.IsTrue(result.HayError);
        }
        #endregion

        #region Campania Actual Tests
        [Test]
        public void GrabarCampaniaActual_ValidObject_ReturnsOk()
        {
            var campania = new CampaniaActual();
            campaniaAcutalManagerMock.Setup(x => x.ActualizacionCampaniaActual(campania))
                .Returns(new Resultado());
            var result = target.GrabarCampaniaActual(campania);
            Assert.NotNull(result);
            Assert.IsFalse(result.HayError);
            Assert.AreEqual(result.ListaErrores.Count, 0);
        }

        [Test]
        public void GrabarCampaniaActual_ValidObject_ReturnsError()
        {
            var campania = new CampaniaActual();
            campaniaAcutalManagerMock.Setup(x => x.ActualizacionCampaniaActual(campania))
                .Returns(new Resultado { Errores = new List<ErrorMessage> { new ErrorMessage { Source = "", Message = "" } } });
            var result = target.GrabarCampaniaActual(campania);
            Assert.NotNull(result);
            Assert.IsTrue(result.HayError);
            Assert.AreEqual(result.ListaErrores.Count, 1);
        }

        [Test]
        public void GrabarCampaniaActual_ThrowsException_CatchesAndReturnsError()
        {
            var campania = new CampaniaActual();
            campaniaAcutalManagerMock.Setup(x => x.ActualizacionCampaniaActual(campania))
                .Throws(new Exception("Test exception"));
            var result = target.GrabarCampaniaActual(campania);
            Assert.NotNull(result);
            Assert.IsTrue(result.HayError);
        }
        #endregion

        #region Campania Material Tests
        [Test]
        public void ActualizarCampaniaMaterial_ValidList_ReturnsOk()
        {
            var campania = new List<CampaniaMaterialSAPDTO>
            {
                new CampaniaMaterialSAPDTO
                {
                    Anio = DateTime.Now.Year,
                    Campania = "20-21",
                    Comercial = "bmelgarejo",
                    CUIT = "20202020",
                    Material = "Soja",
                    Mes = "Mayo",
                    Toneladas = 10
                }
            };
            camaniaMaterialManagerMock.Setup(x => x.TraerCampañasPorGrano(campania))
                .Returns(new Resultado());
            var result = target.ActualizarCampaniaMaterial(campania);
            Assert.NotNull(result);
            Assert.IsFalse(result.HayError);
            Assert.AreEqual(result.ListaErrores.Count, 0);
        }

        [Test]
        public void ActualizarCampaniaMaterial_ValidList_ReturnsError()
        {
            var campania = new List<CampaniaMaterialSAPDTO>
            {
                new CampaniaMaterialSAPDTO
                {
                    Anio = DateTime.Now.Year,
                    Campania = "20-21",
                    Comercial = "bmelgarejo",
                    CUIT = "20202020",
                    Material = "Soja",
                    Mes = "Mayo",
                    Toneladas = 10
                }
            };
            camaniaMaterialManagerMock.Setup(x => x.TraerCampañasPorGrano(campania))
                .Returns(new Resultado { Errores = new List<ErrorMessage> { new ErrorMessage { Source = "", Message = "" } } });
            var result = target.ActualizarCampaniaMaterial(campania);
            Assert.NotNull(result);
            Assert.IsTrue(result.HayError);
            Assert.AreEqual(result.ListaErrores.Count, 1);
        }

        [Test]
        public void ActualizarCampaniaMaterial_ThrowsException_CatchesAndReturnsError()
        {
            var campania = new List<CampaniaMaterialSAPDTO>
            {
                new CampaniaMaterialSAPDTO
                {
                    Anio = DateTime.Now.Year,
                    Campania = "20-21",
                    Comercial = "bmelgarejo",
                    CUIT = "20202020",
                    Material = "Soja",
                    Mes = "Mayo",
                    Toneladas = 10
                }
            };
            camaniaMaterialManagerMock.Setup(x => x.TraerCampañasPorGrano(campania))
                .Throws(new Exception("Test exception"));
            var result = target.ActualizarCampaniaMaterial(campania);
            Assert.NotNull(result);
            Assert.IsTrue(result.HayError);
        }
        #endregion

        #region Actualizar Estado Comercial Tests
        [Test]
        public void ActualizarEstadoComercial_ValidList_ReturnsOk()
        {
            var informe = new List<InformeComercialSAPDTO>
            {
                new InformeComercialSAPDTO
                {
                    CUIT = "20202020",
                    Material = "Soja",
                    RptSap = "ok"
                }
            };
            informeComercialManagerMock.Setup(x => x.RespuestaDeSapCapacidadProductiva(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<String>()))
                .Returns(new Resultado());
            var result = target.ActualizarEstadoComercial(informe);
            Assert.NotNull(result);
            Assert.IsFalse(result.HayError);
            Assert.AreEqual(result.ListaErrores.Count, 0);
        }

        [Test]
        public void ActualizarEstadoComercial_ValidList_ReturnsError()
        {
            var informe = new List<InformeComercialSAPDTO>
            {
                new InformeComercialSAPDTO
                {
                    CUIT = "20202020",
                    Material = "Soja",
                    RptSap = "ok"
                }
            };
            informeComercialManagerMock.Setup(x => x.RespuestaDeSapCapacidadProductiva(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<String>()))
                .Returns(new Resultado { Errores = new List<ErrorMessage> { new ErrorMessage { Source = "", Message = "" } } });
            var result = target.ActualizarEstadoComercial(informe);
            Assert.NotNull(result);
            Assert.IsTrue(result.HayError);
            Assert.AreEqual(result.ListaErrores.Count, 1);
        }

        [Test]
        public void ActualizarEstadoComercial_ThrowsException_CatchesAndReturnsError()
        {
            var informe = new List<InformeComercialSAPDTO>
            {
                new InformeComercialSAPDTO
                {
                    CUIT = "20202020",
                    Material = "Soja",
                    RptSap = "ok"
                }
            };
            informeComercialManagerMock.Setup(x => x.RespuestaDeSapCapacidadProductiva(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<String>()))
                .Throws(new Exception("Test exception"));
            var result = target.ActualizarEstadoComercial(informe);
            Assert.NotNull(result);
            Assert.IsTrue(result.HayError);
        }
        #endregion

        #region Actualizar Contrato SAP Tests
        [Test]
        public void ActualizarContratoSAP_ValidDTO_ReturnsOk()
        {
            var contratoSap = CreateContratoSAPDto();
            SetupRepositorioMocks();
            contratoManagerMock.Setup(y => y.ActualizarContratoSAP(It.IsAny<Contrato>())).Returns(new Resultado());

            var result = target.ActualizarContratoSAP(contratoSap);
            Assert.NotNull(result);
        }

        [Test]
        public void ActualizarContratoSAP_ValidDTO_ReturnsError()
        {
            var contratoSap = CreateContratoSAPDto();
            SetupRepositorioMocks();
            contratoManagerMock.Setup(y => y.ActualizarContratoSAP(It.IsAny<Contrato>()))
                .Returns(new Resultado { Errores = new List<ErrorMessage> { new ErrorMessage { Source = "", Message = "" } } });

            var result = target.ActualizarContratoSAP(contratoSap);
            Assert.NotNull(result);
            Assert.IsTrue(result.HayError);
        }

        [Test]
        public void ActualizarContratoSAP_ThrowsException_CatchesAndReturnsError()
        {
            var contratoSap = CreateContratoSAPDto();
            SetupRepositorioMocks();
            contratoManagerMock.Setup(y => y.ActualizarContratoSAP(It.IsAny<Contrato>()))
                .Throws(new Exception("Test exception"));

            var result = target.ActualizarContratoSAP(contratoSap);
            Assert.NotNull(result);
            Assert.IsTrue(result.HayError);
        }
        #endregion

        #region Alta Contrato SAP Tests
        [Test]
        public void AltaContratoSAP_ValidDTO_ReturnsOk()
        {
            var contratoSap = CreateContratoSAPDto();
            SetupRepositorioMocks();
            contratoManagerMock.Setup(y => y.AltaContratoSAP(It.IsAny<Contrato>())).Returns(new Resultado());

            var result = target.AltaContratoSAP(contratoSap);
            Assert.NotNull(result);
        }

        [Test]
        public void AltaContratoSAP_ValidDTO_ReturnsError()
        {
            var contratoSap = CreateContratoSAPDto();
            SetupRepositorioMocks();
            contratoManagerMock.Setup(y => y.AltaContratoSAP(It.IsAny<Contrato>()))
                .Returns(new Resultado { Errores = new List<ErrorMessage> { new ErrorMessage { Source = "", Message = "" } } });

            var result = target.AltaContratoSAP(contratoSap);
            Assert.NotNull(result);
            Assert.IsTrue(result.HayError);
        }

        [Test]
        public void AltaContratoSAP_ThrowsException_CatchesAndReturnsError()
        {
            var contratoSap = CreateContratoSAPDto();
            SetupRepositorioMocks();
            contratoManagerMock.Setup(y => y.AltaContratoSAP(It.IsAny<Contrato>()))
                .Throws(new Exception("Test exception"));

            var result = target.AltaContratoSAP(contratoSap);
            Assert.NotNull(result);
            Assert.IsTrue(result.HayError);
        }
        #endregion

        #region Actualizar Cupo SAP Tests
        [Test]
        public void ActualizarCupoSAP_ValidDTO_ReturnsOk()
        {
            var cupoSap = CreateCupoSapDto();
            SetupCupoMocks();
            cupoManagerMock.Setup(x => x.ActualizarCupoSAP(It.IsAny<Cupo>()))
                .Returns(new Resultado());

            var result = target.ActualizarCupoSAP(cupoSap);
            Assert.NotNull(result);
            Assert.IsFalse(result.HayError);
        }

        [Test]
        public void ActualizarCupoSAP_ValidDTO_ReturnsError()
        {
            var cupoSap = CreateCupoSapDto();
            SetupCupoMocks();
            cupoManagerMock.Setup(x => x.ActualizarCupoSAP(It.IsAny<Cupo>()))
                .Returns(new Resultado { Errores = new List<ErrorMessage> { new ErrorMessage { Source = "", Message = "" } } });

            var result = target.ActualizarCupoSAP(cupoSap);
            Assert.NotNull(result);
            Assert.IsTrue(result.HayError);
        }

        [Test]
        public void ActualizarCupoSAP_ThrowsException_CatchesAndReturnsError()
        {
            var cupoSap = CreateCupoSapDto();
            SetupCupoMocks();
            cupoManagerMock.Setup(x => x.ActualizarCupoSAP(It.IsAny<Cupo>()))
                .Throws(new Exception("Test exception"));

            var result = target.ActualizarCupoSAP(cupoSap);
            Assert.NotNull(result);
            Assert.IsTrue(result.HayError);
        }
        #endregion

        #region Alta Cupo SAP Tests
        [Test]
        public void AltaCupoSAP_ValidDTO_ReturnsOk()
        {
            var cupoSap = CreateCupoSapDto();
            SetupCupoMocks();
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<Expression<Func<Comercial, int>>>())).Returns(1);
            cupoManagerMock.Setup(x => x.AltaCupoSAP(It.IsAny<Cupo>()))
                .Returns(new Resultado());

            var result = target.AltaCupoSAP(cupoSap);
            Assert.NotNull(result);
            Assert.IsFalse(result.HayError);
        }

        [Test]
        public void AltaCupoSAP_ValidDTO_ReturnsError()
        {
            var cupoSap = CreateCupoSapDto();
            SetupCupoMocks();
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<Expression<Func<Comercial, int>>>())).Returns(1);
            cupoManagerMock.Setup(x => x.AltaCupoSAP(It.IsAny<Cupo>()))
                .Returns(new Resultado { Errores = new List<ErrorMessage> { new ErrorMessage { Source = "", Message = "" } } });

            var result = target.AltaCupoSAP(cupoSap);
            Assert.NotNull(result);
            Assert.IsTrue(result.HayError);
        }

        [Test]
        public void AltaCupoSAP_ThrowsException_CatchesAndReturnsError()
        {
            var cupoSap = CreateCupoSapDto();
            SetupCupoMocks();
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<Expression<Func<Comercial, int>>>())).Returns(1);
            cupoManagerMock.Setup(x => x.AltaCupoSAP(It.IsAny<Cupo>()))
                .Throws(new Exception("Test exception"));

            var result = target.AltaCupoSAP(cupoSap);
            Assert.NotNull(result);
            Assert.IsTrue(result.HayError);
        }
        #endregion

        #region Anular Contrato SAP Tests
        [Test]
        public void AnularContratoSAP_ValidObject_ReturnsOk()
        {
            var contratoSap = new ContratoSAP { Cantidad = "25000" };
            contratoManagerMock.Setup(y => y.AnularContratoSAP(It.IsAny<ContratoSAP>()))
                .Returns(new Resultado());

            var result = target.AnularContratoSAP(contratoSap);
            Assert.NotNull(result);
            Assert.IsFalse(result.HayError);
        }

        [Test]
        public void AnularContratoSAP_ValidObject_ReturnsError()
        {
            var contratoSap = new ContratoSAP { Cantidad = "25000" };
            contratoManagerMock.Setup(y => y.AnularContratoSAP(It.IsAny<ContratoSAP>()))
                .Returns(new Resultado { Errores = new List<ErrorMessage> { new ErrorMessage { Source = "", Message = "" } } });

            var result = target.AnularContratoSAP(contratoSap);
            Assert.NotNull(result);
            Assert.IsTrue(result.HayError);
        }

        [Test]
        public void AnularContratoSAP_ThrowsException_CatchesAndReturnsError()
        {
            var contratoSap = new ContratoSAP { Cantidad = "25000" };
            contratoManagerMock.Setup(y => y.AnularContratoSAP(It.IsAny<ContratoSAP>()))
                .Throws(new Exception("Test exception"));

            var result = target.AnularContratoSAP(contratoSap);
            Assert.NotNull(result);
            Assert.IsTrue(result.HayError);
        }
        #endregion

        #region Helper Methods
        private ContratoSAPDto CreateContratoSAPDto()
        {
            return new ContratoSAPDto
            {
                ContratoSAP = "0002657570",
                Cantidad = 25000,
                Cosecha = "19-20",
                DiasDiferimiento = 0,
                FechaDesde = "2020-10-01",
                FechaEntrega = "2020-10-31",
                FechaHasta = "2020-10-31",
                FechaOperacion = "2020-10-31",
                FechaCreacion = "2020-10-31",
                HORAACT = "16:00:00",
                Moneda = "USDM",
                Material = "000000000019908017",
                Precio = 240,
                Proveedor = "30685141694",
                Provincia = 21,
                Especial = "03",
                Procedencia = "915",
                Centro = "1029",
                Clasificacion = "ACOPIADOR",
                Camiones = 0,
                Ninguno = "X",
                ImporteSPrecio = "0.00",
                PorcSPrecio = "0.00",
                ImporteAPrecio = "0.00",
                MonedaAPrecio = "ARP",
                PorcAPrecio = "0.00",
                MercDescargada = "X",
                PorcComision = 0,
                FleteNivel = "Z01",
                FleteTarifa = 0,
                Calidad = new List<CalidadSAP>(),
                DescuentoBonificaciones = new List<DescuentoBonificacionSap>(),
                Apertura = new List<AperturaPrecioSap>(),
                PrecioNeto = 240,
                PorcentajeDePago = (decimal)97.50,
                PrecioAjusteComision = 0,
                Comercial = "bmelgarejo",
                ComercialCreador = "bmelgarejo",
                TipoNegocio = "MADRE"
            };
        }

        private CupoSapDto CreateCupoSapDto()
        {
            return new CupoSapDto
            {
                Codigo = "MOL5581/10102020",
                FechaIngreso = "2020-10-10",
                Material = "000000000019908017",
                Proveedor = "0030555495494",
                Planta = "1029",
                Zona = "OIS",
                Destinatario = "30715118773",
                FleteProcedencia = "N",
                Calidad = "03",
                Comercial = "LASOH"
            };
        }

        private void SetupRepositorioMocks()
        {
            var contrato = new Contrato { Id = 1, AperturaPrecio = new List<AperturaPrecio>(), Comercial = new Comercial { GrupoDeComprasId = 1 } };
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Contrato, bool>>>())).Returns(contrato);
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CalidadEspecial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
                .Returns(new List<CalidadEspecial>() { new CalidadEspecial { Id = 1, CodigoSap = "230", Descripcion = "Especial", MaterialId = 1 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PrecioPactado, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
                .Returns(new List<PrecioPactado>() { new PrecioPactado { Id = 1 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AperturaPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
               .Returns(new List<AperturaPrecio>() { new AperturaPrecio { Id = 1 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ConceptoAperturaPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
               .Returns(new List<ConceptoAperturaPrecio>() { new ConceptoAperturaPrecio { Id = 1 } });
        }

        private void SetupCupoMocks()
        {
            var cupo = new Cupo { Id = 1, EstadoCupoId = 1, ComercialId = 1 };
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Cupo, bool>>>())).Returns(cupo);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<Expression<Func<Material, int>>>())).Returns(1);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, int>>>())).Returns(1);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Centro, bool>>>(), It.IsAny<Expression<Func<Centro, int>>>())).Returns(1);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<ZonaCupo, bool>>>(), It.IsAny<Expression<Func<ZonaCupo, int>>>())).Returns(1);
        }
        #endregion
    }
}
