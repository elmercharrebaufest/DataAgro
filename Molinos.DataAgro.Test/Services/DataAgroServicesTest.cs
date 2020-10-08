using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebDataAgro.Controllers;
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

            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            target = new DataAgroServices(loggerMock.Object, riesgoComercialManagerMock.Object, campaniaAcutalManagerMock.Object,
                camaniaMaterialManagerMock.Object, informeComercialManagerMock.Object, contratoManagerMock.Object, repositorioMock.Object, cupoManagerMock.Object
                ,mailManagerMock.Object, fijacionManager.Object);

            HttpContext.Current.Session["perfil"] = 1;
            HttpContext.Current.Session["comercialId"] = 1;

        }

        [Test]
        public void PingOk()
        {
            var result = target.Ping() as ResultadoSap;
            Assert.NotNull(result);
            Assert.IsFalse(result.HayError);
        }
        [Test]
        public void GrabarRiesgoComercialOk()
        {
            RiesgoComercial riesgoComercial = new RiesgoComercial();
            riesgoComercialManagerMock.Setup(x => x.ActualizacionDeRiesgoComercial(It.IsAny<RiesgoComercial>()))
                .Returns(new Resultado());

            var result = target.Ping() as ResultadoSap;
            Assert.NotNull(result);
            Assert.IsFalse(result.HayError);
            Assert.AreEqual(result.ListaErrores.Count(), 0);


        }
        [Test]
        public void GrabarRiesgoComercialError()
        {
            RiesgoComercial riesgoComercial = new RiesgoComercial();
            riesgoComercialManagerMock.Setup(x => x.ActualizacionDeRiesgoComercial(It.IsAny<RiesgoComercial>()))
                .Returns(new Resultado { Errores = new List<ErrorMessage> { new ErrorMessage { Source = "", Message = "" } } });

            var result = target.GrabarRiesgoComercial(riesgoComercial) as ResultadoSap;
            Assert.NotNull(result);
            Assert.IsTrue(result.HayError);
            Assert.AreEqual(result.ListaErrores.Count, 1);

        }
        [Test]
        public void GrabarCampaniaActualTestOk()
        {
            var campania = new CampaniaActual();
            campaniaAcutalManagerMock.Setup(x => x.ActualizacionCampaniaActual(campania))
                .Returns(new Resultado());
            var result = target.GrabarCampaniaActual(campania) as ResultadoSap;
            Assert.NotNull(result);
            Assert.IsFalse(result.HayError);
            Assert.AreEqual(result.ListaErrores.Count, 0);

        }
        [Test]
        public void GrabarCampaniaActualTestError()
        {
            var campania = new CampaniaActual();
            campaniaAcutalManagerMock.Setup(x => x.ActualizacionCampaniaActual(campania))
                .Returns(new Resultado { Errores = new List<ErrorMessage> { new ErrorMessage { Source = "", Message = "" } } });
            var result = target.GrabarCampaniaActual(campania) as ResultadoSap;
            Assert.NotNull(result);
            Assert.IsTrue(result.HayError);
            Assert.AreEqual(result.ListaErrores.Count, 1);

        }
        [Test]
        public void ActualizarCampaniaMaterialTestOk()
        {

            var campania = new List<CampaniaMaterialSAPDTO>{

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
                .Returns(new Resultado ());
            var result = target.ActualizarCampaniaMaterial(campania) as ResultadoSap;
            Assert.NotNull(result);
            Assert.IsFalse(result.HayError);
            Assert.AreEqual(result.ListaErrores.Count, 0);

        }
        [Test]
        public void ActualizarCampaniaMaterialTestError()
        {
            
            var campania = new List<CampaniaMaterialSAPDTO>{

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
            var result = target.ActualizarCampaniaMaterial(campania) as ResultadoSap;
            Assert.NotNull(result);
            Assert.IsTrue(result.HayError);
            Assert.AreEqual(result.ListaErrores.Count, 1);

        }
        [Test]
        public void ActualizarEstadoComercialTestOk()
        {

            var informe = new List<InformeComercialSAPDTO>{

                new InformeComercialSAPDTO
                {
                    CUIT = "20202020",
                    Material = "Soja",
                    RptSap = "ok"
                }
            };
            informeComercialManagerMock.Setup(x => x.RespuestaDeSapCapacidadProductiva(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<String>()))
                .Returns(new Resultado ());
            var result = target.ActualizarEstadoComercial(informe) as ResultadoSap;
            Assert.NotNull(result);
            Assert.IsFalse(result.HayError);
            Assert.AreEqual(result.ListaErrores.Count, 0);

        }

        [Test]
        public void ActualizarEstadoComercialTestError()
        {

            var informe = new List<InformeComercialSAPDTO>{

                new InformeComercialSAPDTO
                {                   
                    CUIT = "20202020",
                    Material = "Soja",
                    RptSap = "ok"
                }
            };
            informeComercialManagerMock.Setup(x => x.RespuestaDeSapCapacidadProductiva(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<String>()))
                .Returns(new Resultado { Errores = new List<ErrorMessage> { new ErrorMessage { Source = "", Message = "" } } });
            var result = target.ActualizarEstadoComercial(informe) as ResultadoSap;
            Assert.NotNull(result);
            Assert.IsTrue(result.HayError);
            Assert.AreEqual(result.ListaErrores.Count, 1);

        }

        [Test]
        public void ActualizarContratoSAPTestOk()
        {

            var contratoSap = new ContratoSAPDto
            {
                ContratoSAP = "0002657570",
                Cantidad = 25000,
                Cosecha = "19-20",
                DiasDiferimiento = 0,
                FechaDesde = "2020-10-01",
                FechaEntrega = "2020-10-31",
                FechaHasta = "2020-10-31",
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
                PrecioNeto=240,
                PorcentajeDePago=(decimal)97.50,
                PrecioAjusteComision=0
            };
            var contrato = new Contrato
            {
                Id = 1,
                AperturaPrecio = new List<AperturaPrecio>()
            };
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Contrato, bool>>>())).Returns(contrato);
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CalidadEspecial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
                .Returns(new List<CalidadEspecial>() { new CalidadEspecial { Id = 1, CodigoSap = "230", Descripcion = "Especial", MaterialId = 1 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PrecioPactado, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
                .Returns(new List<PrecioPactado>() { new PrecioPactado { Id = 1 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AperturaPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
               .Returns(new List<AperturaPrecio>() { new AperturaPrecio { Id = 1 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ConceptoAperturaPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
               .Returns(new List<ConceptoAperturaPrecio>() { new ConceptoAperturaPrecio { Id = 1 } });
            informeComercialManagerMock.Setup(x => x.RespuestaDeSapCapacidadProductiva(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<String>()))
                .Returns(new Resultado());
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<BolsaCompraNet, bool>>>(), It.IsAny<Expression<Func<BolsaCompraNet, int>>>())).Returns(1);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Campaña, bool>>>(), It.IsAny<Expression<Func<Campaña, int>>>())).Returns(1);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<ClasificacionCompraNet, bool>>>(), It.IsAny<Expression<Func<ClasificacionCompraNet, int>>>())).Returns(1);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Centro, bool>>>(), It.IsAny<Expression<Func<Centro, int>>>())).Returns(1);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<CondicionFijacion, bool>>>(), It.IsAny<Expression<Func<CondicionFijacion, int>>>())).Returns(1);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<CorredorProveedor, bool>>>(), It.IsAny<Expression<Func<CorredorProveedor, int>>>())).Returns(1);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<Expression<Func<Material, int>>>())).Returns(1);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Moneda, bool>>>(), It.IsAny<Expression<Func<Moneda, string>>>())).Returns("ARP");
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<NivelTarifa, bool>>>(), It.IsAny<Expression<Func<NivelTarifa, int>>>())).Returns(1);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, int>>>())).Returns(1);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<StandardDeCalidad, bool>>>(), It.IsAny<Expression<Func<StandardDeCalidad, int>>>())).Returns(1);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Zona, bool>>>(), It.IsAny<Expression<Func<Zona, int>>>())).Returns(1);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Localidad, bool>>>(), It.IsAny<Expression<Func<Localidad, int>>>())).Returns(1);
            contratoManagerMock.Setup(y => y.ActualizarContratoSAP(It.IsAny<Contrato>(), true)).Returns(new Resultado());
            var result = target.ActualizarContratoSAP(contratoSap) as ResultadoSap;
            Assert.NotNull(result);
            Assert.IsFalse(result.HayError);
            Assert.AreEqual(result.ListaErrores.Count, 0);

        }

        [Test]
        public void ActualizarContratoSAPTestError()
        {

            var contratoSap = new ContratoSAPDto
            {
                ContratoSAP = "0002657570",
                Cantidad = 25000,
                Cosecha = "19-20",
                DiasDiferimiento = 0,
                FechaDesde = "2020-10-01",
                FechaEntrega = "2020-10-31",
                FechaHasta = "2020-10-31",
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
                PrecioAjusteComision = 0
            };
            var contrato = new Contrato
            {
                Id = 1,
                AperturaPrecio = new List<AperturaPrecio>()
            };
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Contrato, bool>>>())).Returns(contrato);
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CalidadEspecial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
                .Returns(new List<CalidadEspecial>() { new CalidadEspecial { Id = 1, CodigoSap = "230", Descripcion = "Especial", MaterialId = 1 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PrecioPactado, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
                .Returns(new List<PrecioPactado>() { new PrecioPactado { Id = 1 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AperturaPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
               .Returns(new List<AperturaPrecio>() { new AperturaPrecio { Id = 1 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ConceptoAperturaPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
               .Returns(new List<ConceptoAperturaPrecio>() { new ConceptoAperturaPrecio { Id = 1 } });
            informeComercialManagerMock.Setup(x => x.RespuestaDeSapCapacidadProductiva(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<String>()))
                .Returns(new Resultado());
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<BolsaCompraNet, bool>>>(), It.IsAny<Expression<Func<BolsaCompraNet, int>>>())).Returns(1);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Campaña, bool>>>(), It.IsAny<Expression<Func<Campaña, int>>>())).Returns(1);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<ClasificacionCompraNet, bool>>>(), It.IsAny<Expression<Func<ClasificacionCompraNet, int>>>())).Returns(1);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Centro, bool>>>(), It.IsAny<Expression<Func<Centro, int>>>())).Returns(1);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<CondicionFijacion, bool>>>(), It.IsAny<Expression<Func<CondicionFijacion, int>>>())).Returns(1);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<CorredorProveedor, bool>>>(), It.IsAny<Expression<Func<CorredorProveedor, int>>>())).Returns(1);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<Expression<Func<Material, int>>>())).Returns(1);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Moneda, bool>>>(), It.IsAny<Expression<Func<Moneda, string>>>())).Returns("ARP");
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<NivelTarifa, bool>>>(), It.IsAny<Expression<Func<NivelTarifa, int>>>())).Returns(1);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, int>>>())).Returns(1);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<StandardDeCalidad, bool>>>(), It.IsAny<Expression<Func<StandardDeCalidad, int>>>())).Returns(1);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Zona, bool>>>(), It.IsAny<Expression<Func<Zona, int>>>())).Returns(1);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Localidad, bool>>>(), It.IsAny<Expression<Func<Localidad, int>>>())).Returns(1);
            contratoManagerMock.Setup(y => y.ActualizarContratoSAP(It.IsAny<Contrato>(), true)).Returns(new Resultado { Errores = new List<ErrorMessage> { new ErrorMessage { Source = "", Message = "" } } });
            var result = target.ActualizarContratoSAP(contratoSap) as ResultadoSap;
            Assert.NotNull(result);
            Assert.IsTrue(result.HayError);
            Assert.AreEqual(result.ListaErrores.Count, 1);

        }

        [Test]
        public void ActualizarCupoSAPTestOk()
        {

            var cupoSap =
                new CupoSapDto
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
                    Comercial = "LASOH",
                };
            var cupo = new Cupo
            {
                Id = 1,
                EstadoCupoId = 1,
            };
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Cupo, bool>>>())).Returns(cupo);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<Expression<Func<Material, int>>>())).Returns(1);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, int>>>())).Returns(1);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Centro, bool>>>(), It.IsAny<Expression<Func<Centro, int>>>())).Returns(1);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<ZonaCupo, bool>>>(), It.IsAny<Expression<Func<ZonaCupo, int>>>())).Returns(1);

            cupoManagerMock.Setup(x => x.ActualizarCupoSAP(It.IsAny<Cupo>()))
                .Returns(new Resultado ());
            var result = target.ActualizarCupoSAP(cupoSap) as ResultadoSap;
            Assert.NotNull(result);
            Assert.IsFalse(result.HayError);
            Assert.AreEqual(result.ListaErrores.Count, 0);

        }

        [Test]
        public void ActualizarCupoSAPTestError()
        {

            var cupoSap =
                new CupoSapDto
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
                    Comercial = "LASOH",
                };
            var cupo = new Cupo
            {
                Id = 1,
                EstadoCupoId = 1,
            };
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Cupo, bool>>>())).Returns(cupo);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<Expression<Func<Material, int>>>())).Returns(1);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, int>>>())).Returns(1);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Centro, bool>>>(), It.IsAny<Expression<Func<Centro, int>>>())).Returns(1);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Zona, bool>>>(), It.IsAny<Expression<Func<Zona, int>>>())).Returns(1);

            cupoManagerMock.Setup(x => x.ActualizarCupoSAP(It.IsAny<Cupo>()))
                .Returns(new Resultado { Errores = new List<ErrorMessage> { new ErrorMessage { Source = "", Message = "" } } });
            var result = target.ActualizarCupoSAP(cupoSap) as ResultadoSap;
            Assert.NotNull(result);
            Assert.IsTrue(result.HayError);
            Assert.AreEqual(result.ListaErrores.Count, 1);

        }


    }
}
