using Kendo.DynamicLinq;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
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

namespace Molinos.DataAgro.Test.Controllers
{
    [TestFixture]
    class LogDataAgroControllerTest
    {
        private LogDataAgroController target;
        private Mock<ILogDataAgroManager> mockLogDataAgroManager;
        private Mock<IReportesManager> mockReportesManager;
        private Mock<IComercialManager> mockComercialManager;
        private Mock<IProveedorManager> mockProveedorManager;
        
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            mockLogDataAgroManager = new Mock<ILogDataAgroManager>();
            mockReportesManager = new Mock<IReportesManager>();
            mockComercialManager = new Mock<IComercialManager>();
            mockProveedorManager = new Mock<IProveedorManager>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            HttpContext.Current.Session["perfil"] = 1;

            target = new LogDataAgroController(mockLogDataAgroManager.Object, mockReportesManager.Object, mockComercialManager.Object, mockProveedorManager.Object);

        }

        [Test]
        public void IndexOk()
        {
            mockComercialManager.Setup(c => c.TraerTodoComercial())
            .Returns(new ResultIniComercial
            {
                Comercial = new List<ComercialIni> {
                    new ComercialIni { Nombres = "nombreTest", Apellido = "apellidoTest", IdActiveDirectory = "idTest" }
                }
            });

            var result = target.Index() as ViewResult;
            Assert.NotNull(result);
            Assert.IsEmpty(result.ViewName);
            Assert.NotNull(result.ViewBag);
        }

        [Test]
        public void BuscarDatosLogDataAgro()
        {
            var responseDeBusqueda = new DataSourceResult { Data = new List<LogDataAgro> { new LogDataAgro { Id = 1 } } };
            mockLogDataAgroManager.Setup(x => x.ListarDatosLogDataAgro(It.IsAny<DataSourceRequest>(), It.IsAny<List<int>>()))
                .Returns(responseDeBusqueda);
            var result = target.BuscarDatosLogDataAgro(new DataSourceRequest { }) as JsonResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(responseDeBusqueda, result.Data);
            mockLogDataAgroManager.Verify(x => x.ListarDatosLogDataAgro(It.IsAny<DataSourceRequest>(), It.IsAny<List<int>>()), Times.Once);
        }

        [Test]
        public void MostrarDiferencias()
        {
            var responseDeBusqueda = new LogDataAgroDto { };

            mockLogDataAgroManager.Setup(x => x.Obtener(It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(responseDeBusqueda);

            var result = target.MostrarDiferencias(1) as JsonResult;

            Assert.IsNull(result);
            mockLogDataAgroManager.Verify(x => x.Obtener(It.IsAny<int>(), It.IsAny<bool>()), Times.Exactly(2));
        }

        [Test]
        public void CargarFillViewBag()
        {
            mockComercialManager.Setup(c => c.TraerTodoComercial())
                .Returns(new ResultIniComercial
                {
                    Comercial = new List<ComercialIni> {
                        new ComercialIni {
                            Nombres = "nombreTest",
                            Apellido = "apellidoTest",
                            IdActiveDirectory = "idTest"
                        }
                    }
                });

            var vistaIndex = target.Index() as ViewResult;

            var datosParaFiltro = vistaIndex.ViewBag;

            var clases = datosParaFiltro.Clase as IEnumerable<SelectListItem>;
            var acciones = datosParaFiltro.Accion as IEnumerable<SelectListItem>;
            var comerciales = datosParaFiltro.Comercial as IEnumerable<SelectListItem>;

            Assert.IsNotNull(datosParaFiltro);
            Assert.IsNotNull(clases);
            Assert.IsNotNull(acciones);
            Assert.IsNotNull(comerciales);
            Assert.That(clases.Count() > 0 && acciones.Count() > 0 && comerciales.Count() == 1, "");
            mockComercialManager.Verify(c => c.TraerTodoComercial(), Times.Once);
        }


        [Test]
        public void BuscarProveedorOk()
        {
            mockProveedorManager.Setup(x => x.DevolverProveedoresCorredores(It.IsAny<string>(),null)).
                Returns(new List<BusquedaHome>() { new BusquedaHome { Alias = "ACA", RazonSocial = "ACA", Cuit = "1233", Id = 1, Filtro = "",} });
            var result = target.BuscarProveedor(It.IsAny<string>()) as JsonResult;

            var a = serializer.Serialize(result);
            Assert.AreEqual(
               "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"ProveedorId\":1,\"Proveedor\":\"ACA\"}],\"JsonRequestBehavior\":0,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
                a);
        }
    }
}
