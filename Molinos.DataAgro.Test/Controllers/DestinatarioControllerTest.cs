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
    public class DestinatarioControllerTest
    {
        private DestinatarioController target;
        private Mock<IDestinatarioManager> destinatarioManagerMock;
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            destinatarioManagerMock = new Mock<IDestinatarioManager>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            HttpContext.Current.Session["perfil"] = 1;
            target = new DestinatarioController(destinatarioManagerMock.Object);            
        }

        [Test]
        public void BuscarDestinatarioTest()
        {
            destinatarioManagerMock.Setup(x => x.TraerTodoDestinatario()).Returns(new ResultIniDestinatario
            {
                Destinatario = new List<DestinatarioIni>()
                {
                    new DestinatarioIni()
                    {
                        DestinatarioId = 1,
                        Descripcion = "A",
                        Inhabilitado = false
                    }
                }
            });
            var result = target.Buscar();

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Datos\":[{\"DestinatarioId\":1,\"Descripcion\":\"A\",\"Inhabilitado\":false}],\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void AplicarDestinatarioTest()
        {
            destinatarioManagerMock.Setup(x => x.TraerDestinatario(1)).Returns(new DestinatarioDto
            {
                DestinatarioId = 1,
                Inhabilitado = false,
                Descripcion = "A"
            }
            );
            var result = target.Aplicar(new AbmDestinatarioParam { DestinatarioId = 1 });

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Destinatario\":{\"DestinatarioId\":1,\"Descripcion\":\"A\",\"Inhabilitado\":false},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void GrabarDestinatarioTest()
        {
            var destinatario = new Destinatario
            {
                DestinatarioId = 1,
                Inhabilitado = false,
                Descripcion = "A"
            };
            destinatarioManagerMock.Setup(x => x.GrabarDestinatario(destinatario)).Returns(new Resultado { Errores = new List<ErrorMessage>() });
            var result = target.Grabar(destinatario);

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Destinatario\":{\"DestinatarioId\":0,\"Descripcion\":\"\",\"Inhabilitado\":false},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void EliminarDestinatarioTest()
        {
            destinatarioManagerMock.Setup(x => x.EliminarDestinatario(1)).Returns(new Resultado { Errores = new List<ErrorMessage>() });
            var result = target.Eliminar(new AbmDestinatarioParam { DestinatarioId = 1});

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void CancelarDestinatarioTest()
        {
            var result = target.Cancelar();

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Destinatario\":{\"DestinatarioId\":0,\"Descripcion\":\"\",\"Inhabilitado\":false},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
    }
}
