using NLog;
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
    public class DestinatarioManagerTest
    {
        private DestinatarioManager target;
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

            target = new DestinatarioManager(logger.Object, repositorioMock.Object);
        }

        [Test]
        public void TraerTodoDestinatarioTestOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Destinatario, DestinatarioIni>>>(), It.IsAny<Expression<Func<Destinatario, bool >>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<DestinatarioIni>() { new DestinatarioIni { DestinatarioId = 1, Descripcion = "1" } });
            var result = target.TraerTodoDestinatario();
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Destinatario, DestinatarioIni>>>(), It.IsAny<Expression<Func<Destinatario, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            Assert.NotNull(result);
            Assert.AreEqual(1, result.Destinatario.Count);
        }
        [Test]
        public void TraerDestinatarioOk()
        {
            
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Destinatario, bool>>>(),It.IsAny<Expression<Func<Destinatario, DestinatarioDto>>>()))
                .Returns( new DestinatarioDto { DestinatarioId=1 } );
            
            var resultado = target.TraerDestinatario(1);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Destinatario, bool>>>(), It.IsAny<Expression<Func<Destinatario, DestinatarioDto>>>()), Times.Once);

            Assert.NotNull(resultado);
            Assert.AreEqual(1, resultado.DestinatarioId);
        }
        [Test]
        public void GrabarDestinatarioOk()
        {
            var prov = new Destinatario { DestinatarioId= 1, Descripcion = "a" };
            repositorioMock.Setup(y => y.Obtener<Destinatario>(It.IsAny<int>()))
                .Returns( new Destinatario { DestinatarioId = 1 });

            var resultado = target.GrabarDestinatario(prov);
            repositorioMock.Verify(x => x.Obtener<Destinatario>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Destinatario>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.NotNull(resultado);
            Assert.IsFalse(resultado.HayErrores);
        }
        [Test]
        public void GrabarDestinatarioNuevaOk()
        {
            var prov = new Destinatario { DestinatarioId = 0, Descripcion = "a" };
            repositorioMock.Setup(y => y.Obtener<Destinatario>(It.IsAny<int>()))
                .Returns(new Destinatario { DestinatarioId = 1 });

            var resultado = target.GrabarDestinatario(prov);
            repositorioMock.Verify(x => x.Obtener<Destinatario>(It.IsAny<int>()), Times.Never);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Destinatario>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.NotNull(resultado);
            Assert.IsFalse(resultado.HayErrores);
        }
        [Test]
        public void EliminarDestinatarioOk()
        {
            var resultado = target.EliminarDestinatario(1);
            repositorioMock.Verify(x => x.Remover<Destinatario>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.NotNull(resultado);
            Assert.IsFalse(resultado.HayErrores);
        }
    }
}
