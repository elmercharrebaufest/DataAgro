using Autofac.Extras.NLog;
using Molinos.DataAgro.Business;
using Molinos.DataAgro.Business.Managers;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq.Expressions;
using System.Web;
using System.Web.Script.Serialization;

namespace Molinos.DataAgro.Test.Managers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class OperadorManagerTest
    {
        private OperadorManager target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ILogger> logger;
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            logger = new Mock<ILogger>();
            repositorioMock = new Mock<IRepositorio>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();

            target = new OperadorManager(logger.Object, repositorioMock.Object);
        }

        [Test]
        public void TraerTodoOperadorTestOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Operador, OperadorCombo>>>(), It.IsAny<Expression<Func<Operador, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<OperadorCombo>() { new OperadorCombo { Id = 1, Descripcion="1" } });
            var result = target.TraerDatosIniciales();
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Operador, OperadorCombo>>>(), It.IsAny<Expression<Func<Operador, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            Assert.NotNull(result);
            Assert.AreEqual(1, result.Operador.Count);
        }

        [Test]
        public void TraerOperadorOk()
        {
            
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Operador, bool>>>(),It.IsAny<Expression<Func<Operador, OperadorDto>>>()))
                .Returns( new OperadorDto { Id=1 } );
            
            var resultado = target.TraerOperador(1);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Operador, bool>>>(), It.IsAny<Expression<Func<Operador, OperadorDto>>>()), Times.Once);

            Assert.NotNull(resultado);
            Assert.AreEqual(1, resultado.Id);
        }
        [Test]
        public void GrabarOperadorOk()
        {
            var prov = new Operador { Id = 1, Descripcion = "a" };
            repositorioMock.Setup(y => y.Obtener<Operador>(It.IsAny<int>()))
                .Returns( new Operador { Id = 1 });

            var resultado = target.GrabarOperador(prov);
            repositorioMock.Verify(x => x.Obtener<Operador>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Operador>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.NotNull(resultado);
            Assert.IsFalse(resultado.HayErrores);
        }
        [Test]
        public void GrabarOperadorNuevaOk()
        {
            var prov = new Operador { Id = 0, Descripcion = "a" };
            repositorioMock.Setup(y => y.Obtener<Operador>(It.IsAny<int>()))
                .Returns(new Operador { Id = 1 });

            var resultado = target.GrabarOperador(prov);
            repositorioMock.Verify(x => x.Obtener<Operador>(It.IsAny<int>()), Times.Never);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Operador>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.NotNull(resultado);
            Assert.IsFalse(resultado.HayErrores);
        }
        [Test]
        public void EliminarOperadorOk()
        {
            var resultado = target.EliminarOperador(1);
            repositorioMock.Verify(x => x.Remover<Operador>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.NotNull(resultado);
            Assert.IsFalse(resultado.HayErrores);
        }


        [Test]
        public void ListarOperadorOkTest()
        {
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<Operador, OperadorIni>>>(), It.IsAny<Expression<Func<Operador, bool>>>(), It.IsAny<int>(), null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<OperadorIni>());

            var result = target.ListarOperador("aa");

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Operador, OperadorIni>>>(), It.IsAny<Expression<Func<Operador, bool>>>(), It.IsAny<int>(), null, Entities.Helpers.DirOrden.Asc), Times.Once);
            Assert.NotNull(result);
            Assert.AreEqual(0, result.Count);
        }
    }
}
