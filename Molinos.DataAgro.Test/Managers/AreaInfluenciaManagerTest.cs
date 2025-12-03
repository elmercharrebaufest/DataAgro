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
    public class AreaInfluenciaManagerTest
    {
        private AreaInfluenciaManager target;
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

            target = new AreaInfluenciaManager(logger.Object, repositorioMock.Object);
        }

        [Test]
        public void TraerTodoAreaInfluenciaTestOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AreaInfluencia, AreaInfluenciaIni>>>(), It.IsAny<Expression<Func<AreaInfluencia, bool >>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<AreaInfluenciaIni>() { new AreaInfluenciaIni { AreaInfluenciaId = 1, Descripcion = "1" } });
            var result = target.TraerTodoAreaInfluencia();
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<AreaInfluencia, AreaInfluenciaIni>>>(), It.IsAny<Expression<Func<AreaInfluencia, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            Assert.NotNull(result);
            Assert.AreEqual(1, result.AreaInfluencia.Count);
        }
        [Test]
        public void TraerAreaInfluenciaOk()
        {
            
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<AreaInfluencia, bool>>>(),It.IsAny<Expression<Func<AreaInfluencia, AreaInfluenciaDto>>>()))
                .Returns( new AreaInfluenciaDto { AreaInfluenciaId=1 } );
            
            var resultado = target.TraerAreaInfluencia(1);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<AreaInfluencia, bool>>>(), It.IsAny<Expression<Func<AreaInfluencia, AreaInfluenciaDto>>>()), Times.Once);

            Assert.NotNull(resultado);
            Assert.AreEqual(1, resultado.AreaInfluenciaId);
        }
        [Test]
        public void GrabarAreaInfluenciaOk()
        {
            var prov = new AreaInfluencia { AreaInfluenciaId= 1, Descripcion = "a" };
            repositorioMock.Setup(y => y.Obtener<AreaInfluencia>(It.IsAny<int>()))
                .Returns( new AreaInfluencia { AreaInfluenciaId = 1 });

            var resultado = target.GrabarAreaInfluencia(prov);
            repositorioMock.Verify(x => x.Obtener<AreaInfluencia>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<AreaInfluencia>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.NotNull(resultado);
            Assert.IsFalse(resultado.HayErrores);
        }
        [Test]
        public void GrabarAreaInfluenciaNuevaOk()
        {
            var prov = new AreaInfluencia { AreaInfluenciaId = 0, Descripcion = "a" };
            repositorioMock.Setup(y => y.Obtener<AreaInfluencia>(It.IsAny<int>()))
                .Returns(new AreaInfluencia { AreaInfluenciaId = 1 });

            var resultado = target.GrabarAreaInfluencia(prov);
            repositorioMock.Verify(x => x.Obtener<AreaInfluencia>(It.IsAny<int>()), Times.Never);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<AreaInfluencia>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.NotNull(resultado);
            Assert.IsFalse(resultado.HayErrores);
        }
        [Test]
        public void EliminarAreaInfluenciaOk()
        {
            var resultado = target.EliminarAreaInfluencia(1);
            repositorioMock.Verify(x => x.Remover<AreaInfluencia>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.NotNull(resultado);
            Assert.IsFalse(resultado.HayErrores);
        }
    }
}
