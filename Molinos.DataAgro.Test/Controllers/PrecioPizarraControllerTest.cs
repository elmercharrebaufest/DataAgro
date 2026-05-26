using NLog;
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
    class PrecioPizarraControllerTest
    {
        private PrecioPizarraController target;
        private Mock<IPrecioPizarraManager> precioPizarraManagerMock;
        private Mock<IMaterialManager> materialManagerMock;
        private Mock<IPizarraManager> pizarraManagerMock;
        private Mock<ILogger> loggerMock;
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            precioPizarraManagerMock = new Mock<IPrecioPizarraManager>();
            materialManagerMock = new Mock<IMaterialManager>();
            pizarraManagerMock = new Mock<IPizarraManager>();
            loggerMock = new Mock<ILogger>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            HttpContext.Current.Session["comercialId"] = 1;
            target = new PrecioPizarraController(precioPizarraManagerMock.Object, materialManagerMock.Object, pizarraManagerMock.Object, loggerMock.Object);
        }

        [Test]
        public void IndexOk()
        {
            materialManagerMock.Setup(x => x.TraerTodoMaterial()).Returns(new ResultIniMaterial
            {
                Material = new List<MaterialIni>
                 {
                     new MaterialIni { Codigo = " ", Descripcion = " ", MaterialId = 1, CampaniaIdActual = 1}
                 }
            });
            pizarraManagerMock.Setup(x => x.TraerTodoPizarra()).Returns(new List<PizarraDto> {
                new PizarraDto {Id = 1, Codigo = "", Descripcion = "" }
            });

            precioPizarraManagerMock.Setup(x => x.TraerTodoMoneda()).Returns(new List<MonedaDto>
            {
                new MonedaDto {MonedaId = "USD", Descripcion = ""}
            });

            precioPizarraManagerMock.Setup(x => x.TraerTodoPrecioPizarra()).Returns(new List<PrecioPizarraDto> {
            new PrecioPizarraDto{ Id = 1, FechaDesde = new DateTime(2018, 10, 26).ToString(), FechaHasta = new DateTime(2018, 10, 26).ToString(), MaterialId = 1, MonedaId = "USD", PizarraId = 1, Precio = 100, UnidadMedida = "TON" } }
            );

            var result = target.Index() as ViewResult;

            precioPizarraManagerMock.Verify(x => x.TraerTodoPrecioPizarra(), Times.Once);
            materialManagerMock.Verify(x => x.TraerTodoMaterial(), Times.Once);
            precioPizarraManagerMock.Verify(x => x.TraerTodoMoneda(), Times.Once);
            pizarraManagerMock.Verify(x => x.TraerTodoPizarra(), Times.Once);

            Assert.NotNull(result);
            Assert.IsInstanceOf<PrecioPizarraModel>(result.Model);
            Assert.AreEqual("Index", (result.ViewName));
        }

        [Test]
        public void GrabarPrecioPizarraOk()
        {
            var precioPizarraModel = new PrecioPizarraModel
            { MaterialId = 1, Id = 1, FechaDesde = "06-08-2019", FechaHasta = "06-08-2019", MonedaId = "USD", PizarraId = 1, Precio = 100, UnidadMedida = "TON", Resultado = new Resultado(), HistorialPrecioPizarra = new List<PrecioPizarraDto>(), Precios = new List<PrecioPizarraModel>() };

            precioPizarraManagerMock.Setup(x => x.GrabarPrecioPizarra(It.IsAny<PrecioPizarra>(), false)).Returns(new Resultado { Errores = new List<ErrorMessage>() });
            precioPizarraManagerMock.Setup(x => x.TraerTodoPrecioPizarra()).Returns(new List<PrecioPizarraDto> {
            new PrecioPizarraDto{ Id = 1, FechaDesde = "06-08-2019", FechaHasta = "06-08-2019", MaterialId = 1, MonedaId = "USD", PizarraId = 1, Precio = 100, UnidadMedida = "TON"} });

            precioPizarraManagerMock.Setup(x => x.TraerPrecioPizarraPorMaterialYPizarra(1, 1)).Returns(new List<PrecioPizarraDto> {
            new PrecioPizarraDto{ Id = 1, FechaDesde = "06-08-2019", FechaHasta = "06-08-2019", MaterialId = 1, MonedaId = "USD", PizarraId = 1, Precio = 100, UnidadMedida = "TON"} });

            var result = target.GrabarPrecioPizarra(precioPizarraModel) as PartialViewResult;

            precioPizarraManagerMock.Verify(x => x.TraerPrecioPizarraPorMaterialYPizarra(1, 1), Times.Once);
            precioPizarraManagerMock.Verify(x => x.TraerTodoPrecioPizarra(), Times.Once);
            precioPizarraManagerMock.Verify(x => x.GrabarPrecioPizarra(It.IsAny<PrecioPizarra>(), false), Times.Once);

            Assert.IsInstanceOf<PrecioPizarraModel>(result.Model);
            Assert.AreEqual("_ListaPrecioPizarra", (result.ViewName));
        }

        [Test]
        public void BuscarPorPizarraYMaterial()
        {
            precioPizarraManagerMock.Setup(x => x.TraerPrecioPizarraPorMaterialYPizarra(1, 1)).Returns(new List<PrecioPizarraDto> {
            new PrecioPizarraDto{ Id = 1, FechaDesde = "06-08-2019", FechaHasta = "06-08-2019", MaterialId = 1, MonedaId = "USD", PizarraId = 1, Precio = 100, UnidadMedida = "TON", Fecha = new DateTime(2019,8,6)} });

            var result = target.BuscarPorPizarraYMaterial(1, 1);

            precioPizarraManagerMock.Verify(x => x.TraerPrecioPizarraPorMaterialYPizarra(1, 1), Times.Once);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            Assert.AreEqual("{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"Id\":1,\"Precio\":100,\"MaterialId\":1,\"PizarraId\":1,\"FechaDesde\":\"06-08-2019\",\"FechaHasta\":\"06-08-2019\",\"MonedaId\":\"USD\",\"Moneda\":null,\"UnidadMedida\":\"TON\",\"Material\":null,\"Pizarra\":null,\"Fecha\":\"\\/Date(1565067600000)\\/\"}],\"JsonRequestBehavior\":1,\"MaxJsonLength\":null,\"RecursionLimit\":null}", a);
        }
    }
}
