using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using WebDataAgro.Controllers;
using WebDataAgro.Models;

namespace Molinos.DataAgro.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class FechaFeriadoControllerTest
    {
        private FechaFeriadoController target;
        private Mock<IFechaFeriadoManager> fechaFeriadoManagerMock;
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            fechaFeriadoManagerMock = new Mock<IFechaFeriadoManager>();
            target = new FechaFeriadoController(fechaFeriadoManagerMock.Object);
        }


        [Test]
        public void BuscarFechaFeriadoTest()
        {
            fechaFeriadoManagerMock.Setup(x => x.TraerTodo()).Returns(
                new List<FechaFeriadoDto>()
                {
                    new FechaFeriadoDto()
                    {
                        Id = 1,
                        Feriado = new DateTime (2020,1,1)
                    }
                }
            );
            var result = target.Buscar();

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Datos\":[{\"Id\":1,\"Feriado\":\"\\/Date(1577847600000)\\/\"}],\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void AplicarFechaFeriadoTest()
        {
            fechaFeriadoManagerMock.Setup(x => x.Traer(1)).Returns(new FechaFeriadoDto
            {
                Id = 1,
                Feriado = new DateTime(2020, 1, 1)
            }
            );
            var result = target.Aplicar(new AbmFechaFeriadoParam { Id = 1 });

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"FechaFeriado\":{\"Id\":1,\"Feriado\":\"\\/Date(1577847600000)\\/\"},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void GrabarFechaFeriadoTest()
        {
            var fechaFeriado = new FechaFeriado
            {
                Id = 1,
                Feriado = new DateTime(2020, 1, 1)

            };
            fechaFeriadoManagerMock.Setup(x => x.Grabar(fechaFeriado)).Returns(new Resultado { Errores = new List<ErrorMessage>() });
            var result = target.Grabar(fechaFeriado);

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"FechaFeriado\":{\"Id\":0,\"Feriado\":\"\\/Date(-62135586000000)\\/\"},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

    
        [Test]
        public void EliminarFechaFeriadoTest()
        {
            fechaFeriadoManagerMock.Setup(x => x.Eliminar(1)).Returns(new Resultado { Errores = new List<ErrorMessage>() });
            var result = target.Eliminar(new AbmFechaFeriadoParam { Id = 1 });

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void CancelarFechaFeriadoTest()
        {
            fechaFeriadoManagerMock.Setup(x => x.Eliminar(1)).Returns(new Resultado { Errores = new List<ErrorMessage>() });
            var result = target.Cancelar();

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"FechaFeriado\":{\"Id\":0,\"Feriado\":\"\\/Date(-62135586000000)\\/\"},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
    }
}
