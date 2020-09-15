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
    public class FechaFeriadoManagerTest
    {
        private FechaFeriadoManager target;
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

            target = new FechaFeriadoManager(logger.Object, repositorioMock.Object);
        }

        
        [Test]
        public void TraerTodoFechaFeriadoTestOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<FechaFeriado, FechaFeriadoDto>>>(), It.IsAny<Expression<Func<FechaFeriado, bool >>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<FechaFeriadoDto>() { new FechaFeriadoDto { Id = 1, Feriado = DateTime.Now.Date } });
            var result = target.TraerTodo();
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<FechaFeriado, FechaFeriadoDto>>>(), It.IsAny<Expression<Func<FechaFeriado, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            Assert.NotNull(result);
            Assert.AreEqual(1, result.Count);
        }
        [Test]
        public void TraerFechaFeriadoOk()
        {
            
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<FechaFeriado, bool>>>(),It.IsAny<Expression<Func<FechaFeriado, FechaFeriadoDto>>>()))
                .Returns( new FechaFeriadoDto { Id=1 } );
            
            var resultado = target.Traer(1);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<FechaFeriado, bool>>>(), It.IsAny<Expression<Func<FechaFeriado, FechaFeriadoDto>>>()), Times.Once);

            Assert.NotNull(resultado);
            Assert.AreEqual(1, resultado.Id);
        }
        [Test]
        public void GrabarFechaFeriadoOk()
        {
            var feriado = new FechaFeriado { Id = 1, Feriado = DateTime.Now.Date };
            repositorioMock.Setup(y => y.Obtener<FechaFeriado>(It.IsAny<int>()))
                .Returns( new FechaFeriado { Id = 1 });

            var resultado = target.Grabar(feriado);
            repositorioMock.Verify(x => x.Obtener<FechaFeriado>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<FechaFeriado>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.NotNull(resultado);
            Assert.IsFalse(resultado.HayErrores);
        }

        [Test]
        public void GrabarFechaFeriadoNuevaOk()
        {
            var feriado = new FechaFeriado { Id = 0, Feriado = DateTime.Now.Date };
            repositorioMock.Setup(y => y.Obtener<FechaFeriado>(It.IsAny<int>()))
                .Returns(new FechaFeriado { Id = 1 });

            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<FechaFeriado, bool>>>())).Returns(false);


            var resultado = target.Grabar(feriado);
            repositorioMock.Verify(x => x.Obtener<FechaFeriado>(It.IsAny<int>()), Times.Never);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<FechaFeriado>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.NotNull(resultado);
            Assert.IsFalse(resultado.HayErrores);
        }

        [Test]
        public void GrabarFechaFeriadoNuevaError()
        {
            var feriado = new FechaFeriado { Id = 0, Feriado = DateTime.Now.Date };
            repositorioMock.Setup(y => y.Obtener<FechaFeriado>(It.IsAny<int>()))
                .Returns(new FechaFeriado { Id = 1 });

            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<FechaFeriado, bool>>>())).Returns(true);

            var resultado = target.Grabar(feriado);
            repositorioMock.Verify(x => x.Obtener<FechaFeriado>(It.IsAny<int>()), Times.Never);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<FechaFeriado>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);

            Assert.NotNull(resultado);
            Assert.IsTrue(resultado.HayErrores);
        }

        [Test]
        public void EliminarFechaFeriadoOk()
        {
            var resultado = target.Eliminar(1);
            repositorioMock.Verify(x => x.Remover<FechaFeriado>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.NotNull(resultado);
            Assert.IsFalse(resultado.HayErrores);
        }
    }
}
