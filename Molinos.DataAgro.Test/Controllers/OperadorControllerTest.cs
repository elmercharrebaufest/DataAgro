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
    public class OperadorControllerTest
    {
        private OperadorController target;
        private Mock<IOperadorManager> operadorManagerMock;
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            operadorManagerMock = new Mock<IOperadorManager>();
            target = new OperadorController(operadorManagerMock.Object);
        }

        [Test]
        public void InicializarTest()
        {
            operadorManagerMock.Setup(x => x.TraerDatosIniciales()).Returns( new DatosIniAbmOperador()
            {
                Operador = new List<OperadorCombo>()
                    {
                        new OperadorCombo
                        {
                            Id = 1,
                            Descripcion = "A"
                        },
                    }
            });
            var result = target.Inicializar();
            
            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Datos\":{\"Operador\":[{\"Id\":1,\"Descripcion\":\"A\"}]},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void BuscarOperadorTest()
        {
            operadorManagerMock.Setup(x => x.TraerTodoOperador()).Returns(new ResultIniOperador
            {
                Operador = new List<OperadorIni>()
                {
                    new OperadorIni()
                    {
                        Id = 1,
                        Descripcion = "A"
                    }
                }
            });
            var result = target.Buscar();

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Datos\":[{\"Id\":1,\"Descripcion\":\"A\"}],\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void OperadorComboTest()
        {
            operadorManagerMock.Setup(x => x.TraerOperador(1)).Returns(new OperadorDto
            {
                Id = 1,
                Descripcion= "A"
            }                            
            );
            var result = target.OperadorCombo(new AbmOperadorParam { Id = 1 });

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Operador\":{\"Id\":1,\"Descripcion\":\"A\"},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void AplicarOperadorTest()
        {
            operadorManagerMock.Setup(x => x.TraerOperador(1)).Returns(new OperadorDto
            {
                Id = 1,
                Descripcion = "A"
            }
            );
            var result = target.Aplicar(new AbmOperadorParam { Id = 1 });

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Operador\":{\"Id\":1,\"Descripcion\":\"A\"},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void GrabarOperadorTest()
        {
            var operador = new Operador
            {
                Id = 1,
                Descripcion = "A"
            };
            operadorManagerMock.Setup(x => x.GrabarOperador(operador)).Returns(new Resultado { Errores = new List<ErrorMessage>() });
            var result = target.Grabar(operador);

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Operador\":{\"Id\":0,\"Descripcion\":null},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void EliminarOperadorTest()
        {
            operadorManagerMock.Setup(x => x.EliminarOperador(1)).Returns(new Resultado { Errores = new List<ErrorMessage>() });
            var result = target.Eliminar(new AbmOperadorParam {Id = 1});

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void CancelarOperadorTest()
        {
            operadorManagerMock.Setup(x => x.EliminarOperador(1)).Returns(new Resultado { Errores = new List<ErrorMessage>() });
            var result = target.Cancelar();

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Operador\":{\"Id\":0,\"Descripcion\":null},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
    }
}
