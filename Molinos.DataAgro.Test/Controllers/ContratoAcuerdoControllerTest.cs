using Autofac.Extras.NLog;
using KendoGridBinder;
using KendoGridBinder.Containers;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
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
    public class ContratoAcuerdoControllerTest
    {
        private ContratoAcuerdoController target;
        private Mock<IContratoAcuerdoManager> contratoAcuerdoManagerMock;
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            contratoAcuerdoManagerMock = new Mock<IContratoAcuerdoManager>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            target = new ContratoAcuerdoController(contratoAcuerdoManagerMock.Object);
            HttpContext.Current.Session["perfil"] = 1;
            HttpContext.Current.Session["comercialId"] = 1;
        }

        [Test]
        public void IndexOk()
        {
            var result = target.Index() as ViewResult;

            Assert.NotNull(result);
            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }
        [Test]
        public void CancelarOk()
        {
            var result = target.Cancelar();
            var a = serializer.Serialize(result);

            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"ContratoAcuerdo\":{\"Id\":0,\"Comercial\":null,\"ComercialId\":0,\"Proveedor\":null,\"ProveedorId\":0,\"Corredor\":null,\"CorredorId\":0,\"Cantidad\":0,\"Precio\":0,\"Material\":null,\"MaterialId\":0,\"Destino\":null,\"DestinoId\":0,\"FechaDesde\":\"\\/Date(-62135586000000)\\/\",\"FechaHasta\":\"\\/Date(-62135586000000)\\/\",\"Fecha\":\"\\/Date(-62135586000000)\\/\",\"FechaModificacionDesde\":null,\"FechaModificacion\":null,\"Estado\":null,\"EstadoId\":0,\"Moneda\":null,\"MonedaId\":null},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void InicializarContratoAcuerdoTest()
        {
            contratoAcuerdoManagerMock.Setup(x => x.TraerDatosCombo())
                .Returns(new DatosIniComboContratoAcuerdo { comercial = new List<ComercialQry>(), destino = new List<CentroQry>(), material = new List<MaterialQry>(), moneda = new List<MonedaQry>() });
            var result = target.InicializarContratoAcuerdo();
            var a = serializer.Serialize(result);

            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Datos\":{\"material\":[],\"comercial\":[],\"destino\":[],\"moneda\":[]},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void EliminarTest()
        {
            contratoAcuerdoManagerMock.Setup(x => x.BorrarAcuerdo(It.IsAny<ContratoAcuerdo>()))
                .Returns(new GrabarAcuerdoResult { AcuerdoId=1, Errores= new List<ErrorMessage>()});
            var result = target.Eliminar(new ContratoAcuerdo());
            var a = serializer.Serialize(result);

            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"AcuerdoId\":1,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void ConfirmarTest()
        {
            contratoAcuerdoManagerMock.Setup(x => x.ConfirmarContratoAcuerdo(It.IsAny<int>()))
                .Returns(new GrabarAcuerdoResult { AcuerdoId = 1, Errores = new List<ErrorMessage>() });
            var result = target.Confirmar(new AbmOperadorParam());
            var a = serializer.Serialize(result);

            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"AcuerdoId\":1,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void GrabarTest()
        {
            contratoAcuerdoManagerMock.Setup(x => x.GrabarAcuerdo(It.IsAny<ContratoAcuerdo>()))
                .Returns(new GrabarAcuerdoResult { AcuerdoId = 1, Errores = new List<ErrorMessage>() });
            var result = target.Grabar(new ContratoAcuerdo());
            var a = serializer.Serialize(result);

            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"AcuerdoId\":1,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
    }
}
