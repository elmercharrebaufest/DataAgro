using Autofac.Extras.NLog;
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
    class ConfiguracionInternaControllerTest
    {
        private ConfiguracionInternaController target;

        private Mock<IConfiguracionInternaManager> configuracionInternaMock;
        private Mock<IMaterialManager> materialMock;
        private Mock<IPrecioPizarraManager> precioMock;
        private Mock<ICampañaManager> campaniaMock;
        private Mock<ITipoNegocioManager> tipoNegocioMock;

        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            configuracionInternaMock = new Mock<IConfiguracionInternaManager>();
            materialMock = new Mock<IMaterialManager>();
            precioMock = new Mock<IPrecioPizarraManager>();
            campaniaMock = new Mock<ICampañaManager>();
            tipoNegocioMock = new Mock<ITipoNegocioManager>();

            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            target = new ConfiguracionInternaController(configuracionInternaMock.Object, materialMock.Object, precioMock.Object,
                campaniaMock.Object, tipoNegocioMock.Object);

            HttpContext.Current.Session["perfil"] = 1;
            HttpContext.Current.Session["comercialId"] = 1;

        }

        [Test]
        public void IndexOk()
        {
            materialMock.Setup(x => x.TraerTodoMaterial())
                .Returns(new ResultIniMaterial() { Material = new List<MaterialIni>() { new MaterialIni { Descripcion = "Soja", MaterialId = 3 } } });

            configuracionInternaMock.Setup(x => x.TraerEstadoPrecioMOA())
                .Returns(new List<EstadoPrecioMOADto>() { new EstadoPrecioMOADto { Habilitado = true, MaterialId = 1, Descripcion = "Soja" } });

            precioMock.Setup(x => x.TraerTodoMoneda())
                .Returns(new List<MonedaDto>());

            tipoNegocioMock.Setup(x => x.TraerTodoTipoNegocio())
                .Returns(new List<TipoNegocioDto>());

            campaniaMock.Setup(x => x.TraerTodoCampania())
                .Returns(new List<CampañaDto>());
            configuracionInternaMock.Setup(x => x.TraerPausadoGeneral())
                .Returns(true);

            var result = target.Index() as ViewResult;
            Assert.NotNull(result);
            Assert.IsEmpty(result.ViewName);
        }

        [Test]
        public void PausarPrecios()
        {
            materialMock.Setup(x => x.TraerTodoMaterial())
              .Returns(new ResultIniMaterial() { Material = new List<MaterialIni>() { new MaterialIni { Descripcion = "Soja", MaterialId = 3 } } });

            configuracionInternaMock.Setup(x => x.TraerEstadoPrecioMOA())
                .Returns(new List<EstadoPrecioMOADto>() { new EstadoPrecioMOADto { Habilitado = true, MaterialId = 1, Descripcion = "Soja" } });

            precioMock.Setup(x => x.TraerTodoMoneda())
                .Returns(new List<MonedaDto>());

            tipoNegocioMock.Setup(x => x.TraerTodoTipoNegocio())
                .Returns(new List<TipoNegocioDto>());

            campaniaMock.Setup(x => x.TraerTodoCampania())
                .Returns(new List<CampañaDto>());
            configuracionInternaMock.Setup(x => x.TraerPausadoGeneral())
                .Returns(true);
            var listaPrecio = new List<EstadoPrecioMOADto>() { new EstadoPrecioMOADto { Descripcion = "Soja", MaterialId = 3, Habilitado = true, Id = 1 } };
            configuracionInternaMock.Setup(x => x.CambiarEstadoPrecioMOA(listaPrecio));

            var result = target.PausarPrecios(listaPrecio);

            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            configuracionInternaMock.Verify(x => x.CambiarEstadoPrecioMOA(listaPrecio), Times.Once);
            Assert.AreEqual(
               "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":\"\",\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
               a);
        }

    }
}
