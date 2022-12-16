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
using static WebDataAgro.MvcApplication;

namespace Molinos.DataAgro.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class TareaProgramadaControllerTest
    {
        private TareaProgramadaController target;
        private Mock<IComercialManager> comercialManagerMock;
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            HttpContext.Current.Session["perfil"] = 1;
            comercialManagerMock = new Mock<IComercialManager>();
            target = new TareaProgramadaController(comercialManagerMock.Object);
        }

        [Test]
        public void IndexOk()
        {
            comercialManagerMock.Setup(x => x.ComercialExiste(GlobalVariables.IdActiveDirectory)).Returns(true);
            var result = target.Index() as ViewResult;

            Assert.NotNull(result);
            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }

        [Test]
        public void BuscarTareaProgramadaTest()
        {            
            var result = target.Buscar();

            Assert.NotNull(result);
            
            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Datos\":[],\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void GrabarTareaProgramadaTest()
        {            
            var result = target.Grabar(new TaskModel()) as JsonResult;

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            var model = serializer.Deserialize<Resultado>(serializer.Serialize(result.Data));
            Assert.AreEqual(true, model.HayErrores);
        }

        [Test]
        public void EliminarTareaProgramadaTest()
        {            
            var result = target.Eliminar(new TaskModel());

            Assert.NotNull(result);

        }

        [Test]
        public void CancelarCanalOperacionTest()
        {
            var result = target.Cancelar();

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Rango\":{\"Id\":0,\"PrecioMinimo\":0,\"PrecioMaximo\":0,\"Material\":null,\"MaterialId\":0,\"Moneda\":null,\"MonedaId\":null},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }        
    }
}
