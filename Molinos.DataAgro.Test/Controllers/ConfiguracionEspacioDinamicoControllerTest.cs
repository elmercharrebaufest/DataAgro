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
    class ConfiguracionEspacioDinamicoControllerTest
    {
        private ConfiguracionEspacioDinamicoController target;
        private Mock<ICentroManager> centroManagerMock;
        private Mock<IMaterialManager> materialManagerMock;
        private Mock<IComercialManager> comercialManagerMock;
        private Mock<IProveedorManager> proveedorManagerMock;
        private Mock<IConfiguracionEspacioDinamicoManager> configuracionEspacioDinamicoManagerMock;

        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            centroManagerMock = new Mock<ICentroManager>();
            materialManagerMock = new Mock<IMaterialManager>();
            comercialManagerMock = new Mock<IComercialManager>();
            proveedorManagerMock = new Mock<IProveedorManager>();
            configuracionEspacioDinamicoManagerMock = new Mock<IConfiguracionEspacioDinamicoManager>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            target = new ConfiguracionEspacioDinamicoController(comercialManagerMock.Object, centroManagerMock.Object, materialManagerMock.Object, proveedorManagerMock.Object, configuracionEspacioDinamicoManagerMock.Object);

            HttpContext.Current.Session["perfil"] = 1;
            HttpContext.Current.Session["EsCupera"] = true;
            HttpContext.Current.Session["comercialId"] = 1;
            materialManagerMock.Setup(x => x.TraerTodoMaterial()).Returns(new ResultIniMaterial { Material = new List<MaterialIni>() });
            centroManagerMock.Setup(x => x.TraerTodoCentro()).Returns(new ResultIniCentro { Centro = new List<CentroIni>() });
            comercialManagerMock.Setup(x => x.TraerTodoComercial()).Returns(new ResultIniComercial { Comercial = new List<ComercialIni>() });

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

            var result = target.TablaConfiguracionEspacioDinamico() as PartialViewResult;

            Assert.NotNull(result);

            Assert.AreEqual("_ListaConfiguracionEspacioDinamico", result.ViewName);
            Assert.IsInstanceOf<ConfiguracionEspacioDinamicoModel>(result.Model);
        }
        [Test]
        public void GrabarCuposTest()
        {
            configuracionEspacioDinamicoManagerMock.Setup(x => x.GrabarConfiguracionEspacioDinamico(It.IsAny<ConfiguracionEspacioDinamico>(), It.IsAny<List<DiaCupo>>()))
                .Returns(new Resultado());
            var result = target.Grabar(new ConfiguracionEspacioDinamicoModel { Id = 1, CantidadCupo = 1, CentroId = 1, MaterialId = 1, CalidadId = null, ComercialId = 1, ProveedorId = 1, FechaDesde = new DateTime(2019, 8, 1), FechaHasta = new DateTime(2019, 8, 1)}) as RedirectToRouteResult;

            Assert.NotNull(result);
            materialManagerMock.Verify(x => x.TraerTodoMaterial(), Times.Once);
            centroManagerMock.Verify(x => x.TraerTodoCentro(), Times.Once);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }
        [Test]
        public void DatosConfiguracionTest()
        {
            configuracionEspacioDinamicoManagerMock.Setup(x => x.TraerTodaConfiguracionEspacioDinamico(It.IsAny<KendoGridMvcRequest>()))
                .Returns(new KendoGrid<ConfiguracionEspacioDinamicoDto>(new List<ConfiguracionEspacioDinamicoDto>() { new ConfiguracionEspacioDinamicoDto { Id = 1, CantidadDeCupo = 1, CentroId = 1, MaterialId = 1, Calidad = "", ComercialId = 1, ProveedorId = 1, Fecha = new DateTime(2000,1,1) } }, 20));
            var result = target.DatosConfiguracion(new KendoGridMvcRequest());

            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            configuracionEspacioDinamicoManagerMock.Verify(x => x.TraerTodaConfiguracionEspacioDinamico(It.IsAny<KendoGridMvcRequest>()), Times.Once);
            Assert.AreEqual(
               "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Groups\":null,\"Data\":[{\"Id\":1,\"CentroId\":1,\"Centro\":null,\"MaterialId\":1,\"Material\":null,\"Fecha\":\"\\/Date(946695600000)\\/\",\"CantidadDeCupo\":1,\"ProveedorId\":1,\"ProveedorCUIT\":null,\"ProveedorRazonSocial\":null,\"Calidad\":\"\",\"ComercialId\":1,\"ComercialNombre\":null}],\"Aggregates\":null,\"Total\":20},\"JsonRequestBehavior\":0,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
               a);
        }
        [Test]
        public void EliminarTest()
        {
            int id = 1;
            configuracionEspacioDinamicoManagerMock.Setup(x => x.EliminarConfiguracionEspacioDinamico(id)).Returns(new Resultado { Errores = new List<ErrorMessage>() });
            var result = target.Eliminar(id);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            configuracionEspacioDinamicoManagerMock.Verify(x => x.EliminarConfiguracionEspacioDinamico(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":0,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
    }
}
