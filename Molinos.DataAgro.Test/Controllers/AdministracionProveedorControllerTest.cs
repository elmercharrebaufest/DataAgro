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
        private Mock<IComercialManager> comercialManagerMock;
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            rolManagerMock = new Mock<IRolManager>();
            proveedorManagerMock = new Mock<IProveedorManager>();
            comercialManagerMock = new Mock<IComercialManager>();
            target = new AdministracionProveedorController(rolManagerMock.Object, proveedorManagerMock.Object, comercialManagerMock.Object);
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
            proveedorManagerMock.Setup(x => x.GrabarRol(It.IsAny<int>(), It.IsAny<List<Rol>>(), It.IsAny<List<Comercial>>())).Returns(new GrabarProveedorResult { Errores = new List<ErrorMessage>() });
            var result = target.GrabarProveedor(1, new List<Rol>() { new Rol { Id = 1, Descripcion = "a" } }, new List<Comercial>() { new Comercial { ComercialId = 1, Apellido = "a", Nombres = "a" } });

            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"ProveedorId\":null,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void TraerDatosProveedorTest()
        {
            proveedorManagerMock.Setup(x => x.TraerRolesProveedor(It.IsAny<int>())).Returns(new List<RolBasicoDto> { new RolBasicoDto { Descripcion = "a", Id = 1 } });
            comercialManagerMock.Setup(x => x.TraerComercialesProveedor(It.IsAny<int>())).Returns(new List<ComercialDto> { new ComercialDto { Apellido = "a", Nombres = "a", ComercialId = 1, AsignarNegocios = true } });
            var result = target.TraerDatosProveedor(1);

            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"roles\":[{\"Id\":1,\"Descripcion\":\"a\"}],\"comerciales\":[{\"ComercialId\":1,\"Apellido\":\"a\",\"Nombres\":\"a\",\"PerfilId\":0,\"EmpleadorACargoId\":null,\"IdActiveDirectory\":null,\"GrupoDeComprasId\":null,\"GrupoDeCompras\":null,\"Administrador\":null,\"Cupera\":null,\"RolesAsociados\":null,\"NombreCompleto\":\"A A\",\"Deshabilitado\":false,\"FechaDeshabilitado\":null,\"AsignarNegocios\":true,\"IdUsuarioSAP\":null}]},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
    }
}
