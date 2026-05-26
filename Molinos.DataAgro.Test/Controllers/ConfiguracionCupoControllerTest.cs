using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Interfaces.Managers;
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
using WebDataAgro.Models;

namespace Molinos.DataAgro.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    class ConfiguracionCupoControllerTest
    {
        private ConfiguracionCupoController target;
        private Mock<ICentroManager> centroManagerMock;
        private Mock<IMaterialManager> materialManagerMock;
        private Mock<IZonaCupoManager> zonaCupoManagerMock;
        private Mock<IProveedorManager> proveedorManagerMock;
        private Mock<IConfiguracionCupoManager> configuracionCupoManagerMock;

        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            centroManagerMock = new Mock<ICentroManager>();
            materialManagerMock = new Mock<IMaterialManager>();
            zonaCupoManagerMock = new Mock<IZonaCupoManager>();
            proveedorManagerMock = new Mock<IProveedorManager>();
            configuracionCupoManagerMock = new Mock<IConfiguracionCupoManager>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            target = new ConfiguracionCupoController(centroManagerMock.Object, materialManagerMock.Object, zonaCupoManagerMock.Object, proveedorManagerMock.Object, configuracionCupoManagerMock.Object);

            HttpContext.Current.Session["perfil"] = 1;
            HttpContext.Current.Session["EsCupera"] = true;
            HttpContext.Current.Session["comercialId"] = 1;
            materialManagerMock.Setup(x => x.TraerTodoMaterial()).Returns(new ResultIniMaterial { Material = new List<MaterialIni>() });
            centroManagerMock.Setup(x => x.TraerTodoCentro()).Returns(new ResultIniCentro { Centro = new List<CentroIni>() });

        }

        [Test]
        public void IndexOk()
        {
            var result = target.Index() as ViewResult;

            Assert.NotNull(result);
            Assert.IsEmpty(result.ViewName);
        }
        [Test]
        public void TablaCuposPartialTest()
        {

            var result = target.TablaCuposPartial() as PartialViewResult;

            Assert.NotNull(result);

            Assert.AreEqual("_ListaCupo", result.ViewName);
            Assert.IsInstanceOf<ConfiguracionCupoModel>(result.Model);
        }
        [Test]
        public void GrabarCuposTest()
        {
            configuracionCupoManagerMock.Setup(x => x.GrabarConfiguracionCupo(It.IsAny<ConfiguracionCupo>(), It.IsAny<List<DiaCupo>>()))
                .Returns(new Resultado());
            var result = target.GrabarCupos(new ConfiguracionCupoModel { Id = 1, CantidadCupo = 1, CentroId = 1, MaterialId = 1 }) as RedirectToRouteResult;

            Assert.NotNull(result);
            materialManagerMock.Verify(x => x.TraerTodoMaterial(), Times.Once);
            centroManagerMock.Verify(x => x.TraerTodoCentro(), Times.Once);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }
        //[Test]
        //public void DatosConfiguracionTest()
        //{
        //    configuracionCupoManagerMock.Setup(x => x.TraerTodaConfiguracionCupo(It.IsAny<KendoGridMvcRequest>()))
        //        .Returns(new KendoGrid<ConfiguracionCupoDto>(new List<ConfiguracionCupoDto>() { new ConfiguracionCupoDto { Id = 1, LimiteCupo = 1, CentroId = 1, MaterialId = 1 } }, 20));
        //    var result = target.DatosConfiguracion(new KendoGridMvcRequest());

        //    Assert.NotNull(result);
        //    var a = serializer.Serialize(result);
        //    configuracionCupoManagerMock.Verify(x => x.TraerTodaConfiguracionCupo(It.IsAny<KendoGridMvcRequest>()), Times.Once);
        //    Assert.AreEqual(
        //       "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Groups\":null,\"Data\":[{\"Id\":1,\"CentroId\":1,\"Centro\":null,\"MaterialId\":1,\"Material\":null,\"Fecha\":\"\\/Date(-62135578800000)\\/\",\"LimiteCupo\":1,\"LimiteAlgoritmo\":0,\"CierreCupera\":null,\"CantidadCupo\":null,\"BloquearCupera\":null,\"LimiteCupoAnterior\":0,\"ZonaCupo\":null,\"LiberarCupera\":false,\"LiberarCuperaDesc\":null,\"CuposConsumidos\":0,\"CentroCodigoSap\":null,\"MaterialCodigoSap\":null,\"Color\":null,\"Bloquear\":false,\"CuposDisponibles\":0,\"NoPropio\":false,\"Sustentable\":null,\"LimiteDescarga\":0,\"CuposDisponiblesConDescarga\":0,\"CuposConsumidosConDescarga\":0}],\"Aggregates\":null,\"Total\":20},\"JsonRequestBehavior\":0,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
        //       a);
        //}
        [Test]
        public void TraerZonaCupoTest()
        {
            zonaCupoManagerMock.Setup(x => x.TraerTodoZonaCupo())
                .Returns(new ResultIniZonaCupo { ZonaCupo = new List<ZonaCupoIni>() { new ZonaCupoIni { Id = 1, Descripcion = "a", CodigoSap = "a" } } });
            var result = target.TraerZonaCupo();

            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            zonaCupoManagerMock.Verify(x => x.TraerTodoZonaCupo(), Times.Once);
            Assert.AreEqual(
               "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"ZonaCupo\":[{\"Id\":1,\"Descripcion\":\"a\",\"CodigoSap\":\"a\"}]},\"JsonRequestBehavior\":0,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
               a);
        }
        [Test]
        public void GrabarLimitesCupoTest()
        {
            configuracionCupoManagerMock.Setup(x => x.GrabarLimites(It.IsAny<List<LimiteCupo>>()))
                .Returns(new Resultado());
            var result = target.GrabarLimitesCupo(new List<LimiteCupo>());

            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            configuracionCupoManagerMock.Verify(x => x.GrabarLimites(It.IsAny<List<LimiteCupo>>()), Times.Once);
            Assert.AreEqual(
               "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":0,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
               a);
        }
        //[Test]
        //public void EditarConfiguracionCupoTest()
        //{
        //    configuracionCupoManagerMock.Setup(x => x.TraerConfiguracionCupo(It.IsAny<int>()))
        //        .Returns(new ConfiguracionCupoDto { Id = 1, CentroId = 1, MaterialId = 1, LimiteCupo = 1 });
        //    var result = target.EditarConfiguracionCupo(1);

        //    Assert.NotNull(result);
        //    var a = serializer.Serialize(result);
        //    configuracionCupoManagerMock.Verify(x => x.TraerConfiguracionCupo(It.IsAny<int>()), Times.Once);
        //    Assert.AreEqual(
        //       "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Id\":1,\"CentroId\":1,\"Centro\":null,\"MaterialId\":1,\"Material\":null,\"Fecha\":\"\\/Date(-62135578800000)\\/\",\"LimiteCupo\":1,\"LimiteAlgoritmo\":0,\"CierreCupera\":null,\"CantidadCupo\":null,\"BloquearCupera\":null,\"LimiteCupoAnterior\":0,\"ZonaCupo\":null,\"LiberarCupera\":false,\"LiberarCuperaDesc\":null,\"CuposConsumidos\":0,\"CentroCodigoSap\":null,\"MaterialCodigoSap\":null,\"Color\":null,\"Bloquear\":false,\"CuposDisponibles\":0,\"NoPropio\":false,\"Sustentable\":null,\"LimiteDescarga\":0,\"CuposDisponiblesConDescarga\":0,\"CuposConsumidosConDescarga\":0},\"JsonRequestBehavior\":0,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
        //       a);
        //}

        [Test]
        public void CambioMasivoTest()
        {
            configuracionCupoManagerMock.Setup(x => x.CambioMasivo(It.IsAny<bool>()))
                .Returns(new Resultado());
            var result = target.EditarConfiguracionCupo(1);

            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            configuracionCupoManagerMock.Verify(x => x.TraerConfiguracionCupo(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(
               "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":null,\"JsonRequestBehavior\":0,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
               a);
        }

        [Test]
        public void GrabarLimitesCupoMasivoTest()
        {
            configuracionCupoManagerMock.Setup(x => x.GrabarLimitesMasivo(It.IsAny<List<LimiteCupo>>(), It.IsAny<List<int>>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new Resultado());
            var result = target.GrabarLimitesCupoMasivo(new List<LimiteCupo>(), new List<int>(), It.IsAny<int>(), It.IsAny<int>());

            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            configuracionCupoManagerMock.Verify(x => x.GrabarLimitesMasivo(It.IsAny<List<LimiteCupo>>(), It.IsAny<List<int>>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            Assert.AreEqual(
               "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":0,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
               a);
        }
    }
}
