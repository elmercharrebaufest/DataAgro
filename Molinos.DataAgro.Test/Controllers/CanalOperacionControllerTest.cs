using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using WebDataAgro.Controllers;
using WebDataAgro.Models;

namespace Molinos.DataAgro.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class CanalOperacionControllerTest
    {
        private CanalOperacionController target;
        private Mock<ICanalOperacionManager> canalOperacionManagerMock;
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            canalOperacionManagerMock = new Mock<ICanalOperacionManager>();
            target = new CanalOperacionController(canalOperacionManagerMock.Object);
        }

        [Test]
        public void BuscarCanalOperacionTest()
        {
            canalOperacionManagerMock.Setup(x => x.TraerTodoCanalOperacion()).Returns(new ResultIniCanalOperacion
            {
                CanalOperacion = new List<CanalOperacionIni>()
                {
                    new CanalOperacionIni()
                    {
                        CanalOperacionId = 1,
                        Inhabilitado = false,
                        Descripcion = "A"
                    }
                }
            });
            var result = target.Buscar();

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Datos\":[{\"CanalOperacionId\":1,\"Descripcion\":\"A\",\"Inhabilitado\":false}],\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void AplicarCanalOperacionTest()
        {
            canalOperacionManagerMock.Setup(x => x.TraerCanalOperacion(1)).Returns(new CanalOperacionDto
            {
                CanalOperacionId = 1,
                Inhabilitado = false,
                Descripcion = "A"
            }
            );
            var result = target.Aplicar(new AbmCanalOperacionParam { CanalOperacionId = 1 });

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"CanalOperacion\":{\"CanalOperacionId\":1,\"Descripcion\":\"A\",\"Inhabilitado\":false},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void GrabarCanalOperacionTest()
        {
            var canalOp = new CanalOperacion
            {
                CanalOperacionId = 1,
                Inhabilitado = false,
                Descripcion = "A"
            };
            canalOperacionManagerMock.Setup(x => x.GrabarCanalOperacion(canalOp)).Returns(new Resultado { Errores = new List<ErrorMessage>() });
            var result = target.Grabar(canalOp);

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"CanalOperacion\":{\"CanalOperacionId\":0,\"Descripcion\":null,\"Inhabilitado\":null},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void EliminarCanalOperacionTest()
        {
            canalOperacionManagerMock.Setup(x => x.EliminarCanalOperacion(1)).Returns(new Resultado { Errores = new List<ErrorMessage>() });
            var result = target.Eliminar(new AbmCanalOperacionParam { CanalOperacionId = 1});

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void CancelarCanalOperacionTest()
        {
            var result = target.Cancelar();

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"CanalOperacion\":{\"CanalOperacionId\":0,\"Descripcion\":null,\"Inhabilitado\":null},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
    }
}
