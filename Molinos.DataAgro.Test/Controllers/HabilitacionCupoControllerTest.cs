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
    class HabilitacionCupoControllerTest
    {
        private HabilitacionCupoController target;
        private Mock<IZonaCupoManager> zonaCupoManagerMock;
        private Mock<IMaterialManager> materialManagerMock;
        private Mock<IHabilitacionCupoManager> habilitacionCupoManagerMock;

        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            zonaCupoManagerMock = new Mock<IZonaCupoManager>();
            habilitacionCupoManagerMock = new Mock<IHabilitacionCupoManager>();
            materialManagerMock = new Mock<IMaterialManager>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            target = new HabilitacionCupoController(habilitacionCupoManagerMock.Object, zonaCupoManagerMock.Object, materialManagerMock.Object);

            HttpContext.Current.Session["perfil"] = 1;
            HttpContext.Current.Session["EsCupera"] = true;
            HttpContext.Current.Session["comercialId"] = 1;
            zonaCupoManagerMock.Setup(x => x.TraerTodoZonaCupo()).Returns(new ResultIniZonaCupo { ZonaCupo = new List<ZonaCupoIni>()});
            materialManagerMock.Setup(y => y.TraerTodoMaterial())
              .Returns(new ResultIniMaterial { Material = new List<MaterialIni>() { new MaterialIni { MaterialId = 1, Descripcion = "a" } } });

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
            Assert.IsInstanceOf<HabilitacionCupoModel>(result.Model);
        }
        [Test]
        public void GrabarCuposTest()
        {
            habilitacionCupoManagerMock.Setup(x => x.GrabarHabilitacionCupo(It.IsAny<HabilitacionCupo>()))
                .Returns(new Resultado());
            var result = target.GrabarHabilitacion(new HabilitacionCupoModel { Id = 1, ZonaCupoId = 1 , MaterialId = 1}) as RedirectToRouteResult;

            Assert.NotNull(result);
            zonaCupoManagerMock.Verify(x => x.TraerTodoZonaCupo(), Times.Once);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }
        [Test]
        public void DatosHabilitacionTest()
        {
            habilitacionCupoManagerMock.Setup(x => x.TraerTodaHabilitacionCupo(It.IsAny<KendoGridMvcRequest>()))
                .Returns(new KendoGrid<HabilitacionCupoDto>(new List<HabilitacionCupoDto>() { new HabilitacionCupoDto { Id = 1, ZonaCupoId = 1, MaterialId = 1 } }, 20));
            var result = target.DatosConfiguracion(new KendoGridMvcRequest());

            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            habilitacionCupoManagerMock.Verify(x => x.TraerTodaHabilitacionCupo(It.IsAny<KendoGridMvcRequest>()), Times.Once);
            Assert.AreEqual(
               "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Groups\":null,\"Data\":[{\"Id\":1,\"ZonaCupoId\":1,\"MaterialId\":1,\"FechaDesde\":\"\\/Date(-62135586000000)\\/\",\"FechaHasta\":\"\\/Date(-62135586000000)\\/\",\"Zona\":null,\"Material\":null}],\"Aggregates\":null,\"Total\":20},\"JsonRequestBehavior\":0,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
               a);
        }
       
        [Test]
        public void EditarConfiguracionCupoTest()
        {
            habilitacionCupoManagerMock.Setup(x => x.TraerHabilitacionCupo(It.IsAny<int>()))
                .Returns(new HabilitacionCupoDto { Id=1, ZonaCupoId = 1, MaterialId = 1});
            var result = target.EditarHabilitacionCupo(1);

            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            habilitacionCupoManagerMock.Verify(x => x.TraerHabilitacionCupo(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(
               "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Id\":1,\"ZonaCupoId\":1,\"MaterialId\":1,\"FechaDesde\":\"\\/Date(-62135586000000)\\/\",\"FechaHasta\":\"\\/Date(-62135586000000)\\/\",\"Zona\":null,\"Material\":null},\"JsonRequestBehavior\":0,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
               a);
        }

        [Test]
        public void EliminarHabilitacionCupoOk()
        {
            habilitacionCupoManagerMock.Setup(x => x.EliminarHabilitacionCupo(It.IsAny<int>()))
                 .Returns(new Resultado());
            var result = target.EliminarHabilitacionCupo(1);

            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            habilitacionCupoManagerMock.Verify(x => x.EliminarHabilitacionCupo(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(
             "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":0,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
             a);
        }
    }
}
