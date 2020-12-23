using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebDataAgro.Controllers;
using WebDataAgro.Models;

namespace Molinos.DataAgro.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class AgendaControllerTest
    {
        private AgendaController target;
        private Mock<IAgendaManager> mobjAgendaManager;
        private Mock<IReportesManager> mobjReportesManager;
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            mobjAgendaManager = new Mock<IAgendaManager>();
            mobjReportesManager = new Mock<IReportesManager>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            HttpContext.Current.Session["perfil"] = 1;
            target = new AgendaController(mobjAgendaManager.Object, mobjReportesManager.Object);
        }

        [Test]
        public void IndexOk()
        {
            var result = target.Index() as ViewResult;

            Assert.NotNull(result);
            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }
            
        [Test]
        public void InicializarOk()
        {
            HttpContext.Current.Session["equipo"] = new List<int>{ 1, 2};
            mobjAgendaManager.Setup(x => x.TraerDatosIniciales(It.Is<List<int>>(y => y.Count == 2))).Returns(new Entities.Dto.DatosIniAgendaActividad
            {
                Proveedores = new List<Entities.Dto.ProveedorCombo> {
                    new Entities.Dto.ProveedorCombo { ProveedorId = 1, RazonSocial = "a" },
                    new Entities.Dto.ProveedorCombo { ProveedorId = 2, RazonSocial = "b" }},
                TiposActividades = new List<Entities.Dto.TipoActividadCombo> {
                    new Entities.Dto.TipoActividadCombo { Descripcion = "1", TipoActividadId = 2 },
                    new Entities.Dto.TipoActividadCombo { Descripcion = "2", TipoActividadId = 2 }
                }
                
            });
            var result = target.Inicializar() as JsonResult;

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Datos\":{\"TiposActividades\":[{\"TipoActividadId\":2,\"Descripcion\":\"1\"},{\"TipoActividadId\":2,\"Descripcion\":\"2\"}],\"Proveedores\":[{\"ProveedorId\":1,\"RazonSocial\":\"a\",\"Cuit\":null},{\"ProveedorId\":2,\"RazonSocial\":\"b\",\"Cuit\":null}]},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void ExportarAgendaOk()
        {
            HttpContext.Current.Session["equipo"] = new List<int> { 1, 2 };
            mobjAgendaManager.Setup(x => x.ExportarAgenda(It.IsAny<RptActividadAgendaParam>(), It.Is<List<int>>(y => y.Count == 2))).Returns(new List<AgendaStore>()
            {
                new AgendaStore{}
            });

            var result = target.ExportarAgenda(new RptActividadAgendaParam()) as JsonResult;

            mobjReportesManager.Verify(x => x.GrabarReporte(It.IsAny<Reportes>()), Times.Once);

            Assert.IsTrue(((ReportesModel)result.Data).DownloadKey != "");
        }

        [Test]
        public void VistaPreviaAgendaOk()
        {
            HttpContext.Current.Session["equipo"] = new List<int> { 1, 2 };
            mobjAgendaManager.Setup(x => x.VistaPreviaAgenda(It.Is<RptActividadAgendaParam>(y => y.ActiveDirectoryId == "name"))).Returns(new List<AgendaStore>()
            {
                new AgendaStore{}
            });

            var result = target.VistaPreviaAgenda(new RptActividadAgendaParam()) as JsonResult;

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":null,\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
    }
}
