using Autofac.Extras.NLog;
using KendoGridBinder;
using KendoGridBinder.Containers;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Moq;
using NUnit.Framework;
using System;
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
    public class FijacionDePrecioControllerTest
    {
        private FijacionDePrecioController target;
        private Mock<IFijacionDePrecioManager> fijacionDePrecioManagerMock;
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            fijacionDePrecioManagerMock = new Mock<IFijacionDePrecioManager>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            target = new FijacionDePrecioController(fijacionDePrecioManagerMock.Object);
        }

        [Test]
        public void IndexOk()
        {
            var result = target.Index() as ViewResult;

            Assert.NotNull(result);
            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }
        [Test]
        public void InicializarTest()
        {           
            fijacionDePrecioManagerMock.Setup(x => x.TraerDatosIniciales()).Returns(new DatosIniAbmFijacionDePrecio { Material = new List<MaterialCombo> { new MaterialCombo {Descripcion="a", MaterialId=1 } } });
            var result = target.Inicializar();
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            fijacionDePrecioManagerMock.Verify(x => x.TraerDatosIniciales(), Times.Once);
        }
        [Test]
        public void BuscarTest()
        {
            fijacionDePrecioManagerMock.Setup(x => x.TraerTodoFijacionDePrecio()).Returns(new ResultIniFijacionDePrecio { FijacionDePrecio = new List<FijacionDePrecioIni>() { new FijacionDePrecioIni() } });
            var result = target.Buscar();
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            fijacionDePrecioManagerMock.Verify(x => x.TraerTodoFijacionDePrecio(), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Datos\":[{\"FijacionId\":0,\"MatDescripcion\":null,\"Precio\":null,\"Fecha\":null,\"ProveedorId\":null}],\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void AplicarTest()
        {
            fijacionDePrecioManagerMock.Setup(x => x.TraerFijacionDePrecio(1)).Returns(new FijacionDePrecioDto());
            var result = target.Aplicar(new AbmFijacionDePrecioParam {FijacionId = 1});
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            fijacionDePrecioManagerMock.Verify(x => x.TraerFijacionDePrecio(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"FijacionDePrecio\":{\"FijacionId\":0,\"MaterialId\":null,\"Precio\":null,\"Fecha\":null,\"ProveedorId\":null},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void GrabarTest()
        {
            var fijacion = new FijacionDePrecio
            {
                ProveedorId = 1,
                MaterialId = 2,
                Precio = 1,
                Fecha = DateTime.Parse("12/12/2018")
            };
            fijacionDePrecioManagerMock.Setup(x => x.GrabarFijacionDePrecio(fijacion, GlobalVariables.IdActiveDirectory)).Returns(new Resultado { Errores = new List<ErrorMessage>()});
            var result = target.Grabar(fijacion);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            fijacionDePrecioManagerMock.Verify(x => x.GrabarFijacionDePrecio(It.IsAny<FijacionDePrecio>(), It.IsAny<string>()), Times.Once);
        }
        [Test]
        public void EliminarTest()
        {
            fijacionDePrecioManagerMock.Setup(x => x.EliminarFijacionDePrecio(1)).Returns(new Resultado { Errores = new List<ErrorMessage>()});
            var result = target.Eliminar(new AbmFijacionDePrecioParam { FijacionId = 1});
            Assert.NotNull(result);
            var a = serializer.Serialize(result);

            fijacionDePrecioManagerMock.Verify(x => x.EliminarFijacionDePrecio(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void CancelarTest()
        {
            var result = target.Cancelar();
            Assert.NotNull(result);
            var a = serializer.Serialize(result);
        }
    }
}
