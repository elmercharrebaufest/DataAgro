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
    public class CondicionManagerTest
    {
        private CondicionManager target;
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

            target = new CondicionManager(logger.Object, repositorioMock.Object);
        }

        [Test]
        public void TraerTodoCondicionTestOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Condicion, CondicionIni>>>(), It.IsAny<Expression<Func<Condicion, bool >>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<CondicionIni>() { new CondicionIni { CondicionId = 1, Descripcion = "1" } });
            var result = target.TraerTodoCondicion();
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Condicion, CondicionIni>>>(), It.IsAny<Expression<Func<Condicion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            Assert.NotNull(result);
            Assert.AreEqual(1, result.Condicion.Count);
        }
        [Test]
        public void TraerCondicionOk()
        {
            
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Condicion, bool>>>(),It.IsAny<Expression<Func<Condicion, CondicionDto>>>()))
                .Returns( new CondicionDto { CondicionId=1 } );
            
            var resultado = target.TraerCondicion(1);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Condicion, bool>>>(), It.IsAny<Expression<Func<Condicion, CondicionDto>>>()), Times.Once);

            Assert.NotNull(resultado);
            Assert.AreEqual(1, resultado.CondicionId);
        }
        [Test]
        public void GrabarCondicionOk()
        {
            var prov = new Condicion { CondicionId= 1, Descripcion = "a" };
            repositorioMock.Setup(y => y.Obtener<Condicion>(It.IsAny<int>()))
                .Returns( new Condicion { CondicionId = 1 });

            var resultado = target.GrabarCondicion(prov);
            repositorioMock.Verify(x => x.Obtener<Condicion>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Condicion>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.NotNull(resultado);
            Assert.IsFalse(resultado.HayErrores);
        }
        [Test]
        public void GrabarCondicionNuevaOk()
        {
            var prov = new Condicion { CondicionId = 0, Descripcion = "a" };
            repositorioMock.Setup(y => y.Obtener<Condicion>(It.IsAny<int>()))
                .Returns(new Condicion { CondicionId = 1 });

            var resultado = target.GrabarCondicion(prov);
            repositorioMock.Verify(x => x.Obtener<Condicion>(It.IsAny<int>()), Times.Never);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Condicion>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.NotNull(resultado);
            Assert.IsFalse(resultado.HayErrores);
        }
        [Test]
        public void EliminarCondicionOk()
        {
            var resultado = target.EliminarCondicion(1);
            repositorioMock.Verify(x => x.Remover<Condicion>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.NotNull(resultado);
            Assert.IsFalse(resultado.HayErrores);
        }
    }
}
