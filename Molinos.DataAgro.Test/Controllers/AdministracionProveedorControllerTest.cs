using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Interfaces.Managers;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebDataAgro.Controllers;
using WebDataAgro.Models;

namespace Molinos.DataAgro.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class AdministracionProveedorControllerTest
    {
        private AdministracionProveedorController target;
        private Mock<IRolManager> rolManagerMock; 
        private Mock<IProveedorManager> proveedorManagerMock; 
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            rolManagerMock = new Mock<IRolManager>();
            proveedorManagerMock = new Mock<IProveedorManager>();
            target = new AdministracionProveedorController(rolManagerMock.Object, proveedorManagerMock.Object);
        }

        [Test]
        public void IndexTest()
        {
            var result = target.Index() as ViewResult;

            Assert.NotNull(result);

            Assert.NotNull(result);
            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }


        [Test]
        public void GrabarRolProveedorTest()
        {
            proveedorManagerMock.Setup(x => x.GrabarRol(It.IsAny<int>(), It.IsAny<List<Rol>>())).Returns(new GrabarProveedorResult { Errores = new List<ErrorMessage>()});
            var result = target.GrabarRolProveedor(1, new List<Rol>() { new Rol { Id= 1, Descripcion="a"} });

            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"ProveedorId\":null,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void TraerRolesProveedorTest()
        {
            proveedorManagerMock.Setup(x => x.TraerRolesProveedor(It.IsAny<int>())).Returns(new List<RolBasicoDto> { new RolBasicoDto { Descripcion="a",Id=1} });
            var result = target.TraerRolesProveedor(1);

            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"Id\":1,\"Descripcion\":\"a\"}],\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
    }
}
