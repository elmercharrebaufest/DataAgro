using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Interfaces.Managers;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Web;
using System.Web.Script.Serialization;
using WebDataAgro.Controllers;
using WebDataAgro.Models;
using static WebDataAgro.MvcApplication;

namespace Molinos.DataAgro.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class ComercialControllerTest
    {
        private ComercialController target;
        private Mock<IComercialManager> comercialManagerMock;
        private Mock<IRolManager> rolManagerMock;
        private JavaScriptSerializer serializer;
        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            comercialManagerMock = new Mock<IComercialManager>();
            rolManagerMock = new Mock<IRolManager>();
            target = new ComercialController(comercialManagerMock.Object, rolManagerMock.Object);
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
        }

        [Test]
        public void InicializarTest()
        {
            comercialManagerMock.Setup(x => x.TraerDatosIniciales()).Returns(new DatosIniAbmComercial()
            {
                Comercial = new List<ComercialCombo>() { new ComercialCombo { ComercialId = 1, Apellido = "A" } },
                Perfil = new List<Perfil>() { new Perfil { PerfilId = 1, Descripcion = "Mesa" } },
                GrupoDeCompras = new List<GrupoDeCompras>() { new GrupoDeCompras { Id = 1, Descripcion = "A" } }
            });
            var result = target.Inicializar();

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Datos\":{\"Comercial\":[{\"ComercialId\":1,\"Apellido\":\"A\"}],\"Perfil\":[{\"PerfilId\":1,\"Descripcion\":\"Mesa\"}],\"GrupoDeCompras\":[{\"Id\":1,\"Descripcion\":\"A\",\"Corredor\":false}],\"Rol\":null},\"Comercial\":{\"ComercialId\":0,\"Apellido\":\"\",\"Nombres\":\"\",\"PerfilId\":null,\"EmpleadorACargoId\":null,\"IdActiveDirectory\":\"\",\"GrupoDeComprasId\":null,\"Administrador\":null,\"Cupera\":null,\"Deshabilitado\":null,\"FechaDeshabilitado\":null,\"AsignarNegocios\":false,\"IdUsuarioSAP\":null,\"Perfil\":null,\"EmpleadorACargo\":null,\"GrupoDeCompras\":null,\"RolesAsociados\":null},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void BuscarComercialTest()
        {
            comercialManagerMock.Setup(x => x.TraerTodoComercial()).Returns(new ResultIniComercial
            {
                Comercial = new List<ComercialIni>()
                {
                    new ComercialIni()
                    {
                        ComercialId = 1,
                        Apellido = "A",
                        Nombres = "A",
                        PerDescripcion = "Mesa"
                    }
                }
            });
            var result = target.Buscar();

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual("{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Datos\":[{\"ComercialId\":1,\"Apellido\":\"A\",\"Nombres\":\"A\",\"PerDescripcion\":\"Mesa\",\"Rol\":null,\"NombreCompleto\":\"A A\",\"Disabled\":false,\"Deshabilitado\":false,\"FechaDeshabilitado\":null,\"IdActiveDirectory\":null,\"Equipo\":null,\"AsignarNegocios\":false}],\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void ComercialComboTest()
        {
            comercialManagerMock.Setup(x => x.ObtenerComerciales(GlobalVariables.Equipo, 1)).Returns(new List<ComercialCombo>
            { new ComercialCombo {
                ComercialId = 1,
                Apellido= "A"}
            });
            var result = target.ComercialCombo(new AbmComercialParam { ComercialId = 1 });

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Comercial\":[{\"ComercialId\":1,\"Apellido\":\"A\"}],\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void AplicarComercialTest()
        {
            comercialManagerMock.Setup(x => x.TraerComercial(1)).Returns(new ComercialDto
            {
                ComercialId = 1,
                Nombres = "A",
                Apellido = "B",
                Administrador = false,
                EmpleadorACargoId = null,
                GrupoDeComprasId = 44,
                IdActiveDirectory = "ba",
                PerfilId = 1
            }
            );
            var result = target.Aplicar(new AbmComercialParam { ComercialId = 1 });

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Comercial\":{\"ComercialId\":1,\"Apellido\":\"B\",\"Nombres\":\"A\",\"PerfilId\":1,\"EmpleadorACargoId\":null,\"IdActiveDirectory\":\"ba\",\"GrupoDeComprasId\":44,\"GrupoDeCompras\":null,\"Administrador\":false,\"Cupera\":null,\"RolesAsociados\":null,\"NombreCompleto\":\"B A\",\"Deshabilitado\":false,\"FechaDeshabilitado\":null,\"AsignarNegocios\":false,\"IdUsuarioSAP\":null},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}"
                , a);
        }

        [Test]
        public void GrabarComercialTest()
        {
            var comercial = new Comercial
            {
                ComercialId = 1,
                Nombres = "A",
                Apellido = "B",
                Administrador = false,
                EmpleadorACargoId = null,
                GrupoDeComprasId = 44,
                IdActiveDirectory = "ba",
                PerfilId = 1,
                AsignarNegocios= true
            };
            var roles = new List<Rol>() { new Rol { Id = 1, Descripcion = "A" } };
            comercialManagerMock.Setup(x => x.GrabarComercial(comercial, roles)).Returns(new Resultado { Errores = new List<ErrorMessage>() });
            var result = target.Grabar(comercial, roles);

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Comercial\":{\"ComercialId\":0,\"Apellido\":\"\",\"Nombres\":\"\",\"PerfilId\":null,\"EmpleadorACargoId\":null,\"IdActiveDirectory\":\"\",\"GrupoDeComprasId\":null,\"Administrador\":null,\"Cupera\":null,\"Deshabilitado\":null,\"FechaDeshabilitado\":null,\"AsignarNegocios\":false,\"IdUsuarioSAP\":null,\"Perfil\":null,\"EmpleadorACargo\":null,\"GrupoDeCompras\":null,\"RolesAsociados\":null},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void EliminarComercialTest()
        {
            comercialManagerMock.Setup(x => x.EliminarComercial(1)).Returns(new Resultado { Errores = new List<ErrorMessage>() });
            var result = target.Eliminar(new AbmComercialParam { ComercialId = 1 });

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void CancelarComercialTest()
        {
            comercialManagerMock.Setup(x => x.EliminarComercial(1)).Returns(new Resultado { Errores = new List<ErrorMessage>() });
            var result = target.Cancelar();

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Comercial\":{\"ComercialId\":0,\"Apellido\":\"\",\"Nombres\":\"\",\"PerfilId\":null,\"EmpleadorACargoId\":null,\"IdActiveDirectory\":\"\",\"GrupoDeComprasId\":null,\"Administrador\":null,\"Cupera\":null,\"Deshabilitado\":null,\"FechaDeshabilitado\":null,\"AsignarNegocios\":false,\"IdUsuarioSAP\":null,\"Perfil\":null,\"EmpleadorACargo\":null,\"GrupoDeCompras\":null,\"RolesAsociados\":null},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
    }
}
