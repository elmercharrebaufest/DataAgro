using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Web;
using System.Web.Script.Serialization;
using WebDataAgro.Controllers;
using WebDataAgro.Models;

namespace Molinos.DataAgro.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class CondicionControllerTest
    {
        private CondicionController target;
        private Mock<ICondicionManager> condicionManagerMock;
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();            
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            HttpContext.Current.Session["perfil"] = 1;
            condicionManagerMock = new Mock<ICondicionManager>();
            target = new CondicionController(condicionManagerMock.Object);
        }

        [Test]
        public void BuscarCondicionTest()
        {
            condicionManagerMock.Setup(x => x.TraerTodoCondicion()).Returns(new ResultIniCondicion
            {
                Condicion = new List<CondicionIni>()
                {
                    new CondicionIni()
                    {
                        CondicionId= 1,
                        Inhabilitado = false,
                        Descripcion = "A"
                    }
                }
            });
            var result = target.Buscar();

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Datos\":[{\"CondicionId\":1,\"Descripcion\":\"A\",\"Inhabilitado\":false}],\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void AplicarCondicionTest()
        {
            condicionManagerMock.Setup(x => x.TraerCondicion(1)).Returns(new CondicionDto
            {
                CondicionId = 1,
                Inhabilitado = false,
                Descripcion = "A"
            }
            );
            var result = target.Aplicar(new AbmCondicionParam { CondicionId = 1 });

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Condicion\":{\"CondicionId\":1,\"Descripcion\":\"A\",\"Inhabilitado\":false},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void GrabarCondicionTest()
        {
            var condicion = new Condicion
            {
                CondicionId = 1,
                Inhabilitado = false,
                Descripcion = "A"
            };
            condicionManagerMock.Setup(x => x.GrabarCondicion(condicion)).Returns(new Resultado { Errores = new List<ErrorMessage>() });
            var result = target.Grabar(condicion);

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Condicion\":{\"CondicionId\":0,\"Descripcion\":\"\",\"Inhabilitado\":null},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void EliminarCondicionTest()
        {
            condicionManagerMock.Setup(x => x.EliminarCondicion(1)).Returns(new Resultado { Errores = new List<ErrorMessage>() });
            var result = target.Eliminar(new AbmCondicionParam { CondicionId = 1});

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void CancelarCondicionTest()
        {
            var result = target.Cancelar();

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Condicion\":{\"CondicionId\":0,\"Descripcion\":\"\",\"Inhabilitado\":null},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
    }
}
