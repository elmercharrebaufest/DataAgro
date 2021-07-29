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
    public class CanalOperacionManagerTest
    {
        private CanalOperacionManager target;
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

            target = new CanalOperacionManager(logger.Object, repositorioMock.Object);
        }

        [Test]
        public void TraerTodoCanalOperacionTestOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CanalOperacion, CanalOperacionIni>>>(), It.IsAny<Expression<Func<CanalOperacion, bool >>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<CanalOperacionIni>() { new CanalOperacionIni { CanalOperacionId = 1, Descripcion = "1" } });
            var result = target.TraerTodoCanalOperacion();
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<CanalOperacion, CanalOperacionIni>>>(), It.IsAny<Expression<Func<CanalOperacion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            Assert.NotNull(result);
            Assert.AreEqual(1, result.CanalOperacion.Count);
        }
        [Test]
        public void TraerCanalOperacionOk()
        {
            
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<CanalOperacion, bool>>>(),It.IsAny<Expression<Func<CanalOperacion, CanalOperacionDto>>>()))
                .Returns( new CanalOperacionDto { CanalOperacionId=1 } );
            
            var resultado = target.TraerCanalOperacion(1);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<CanalOperacion, bool>>>(), It.IsAny<Expression<Func<CanalOperacion, CanalOperacionDto>>>()), Times.Once);

            Assert.NotNull(resultado);
            Assert.AreEqual(1, resultado.CanalOperacionId);
        }
        [Test]
        public void GrabarCanalOperacionOk()
        {
            var prov = new CanalOperacion { CanalOperacionId= 1, Descripcion = "a" };
            repositorioMock.Setup(y => y.Obtener<CanalOperacion>(It.IsAny<int>()))
                .Returns( new CanalOperacion { CanalOperacionId = 1 });

            var resultado = target.GrabarCanalOperacion(prov);
            repositorioMock.Verify(x => x.Obtener<CanalOperacion>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<CanalOperacion>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.NotNull(resultado);
            Assert.IsFalse(resultado.HayErrores);
        }
        [Test]
        public void GrabarCanalOperacionNuevaOk()
        {
            var prov = new CanalOperacion { CanalOperacionId = 0, Descripcion = "a" };
            repositorioMock.Setup(y => y.Obtener<CanalOperacion>(It.IsAny<int>()))
                .Returns(new CanalOperacion { CanalOperacionId = 1 });

            var resultado = target.GrabarCanalOperacion(prov);
            repositorioMock.Verify(x => x.Obtener<CanalOperacion>(It.IsAny<int>()), Times.Never);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<CanalOperacion>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.NotNull(resultado);
            Assert.IsFalse(resultado.HayErrores);
        }
        [Test]
        public void EliminarCanalOperacionOk()
        {
            var resultado = target.EliminarCanalOperacion(1);
            repositorioMock.Verify(x => x.Remover<CanalOperacion>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.NotNull(resultado);
            Assert.IsFalse(resultado.HayErrores);
        }

        [Test]
        public void EliminarCaunalOperacionOk()
        {
            var CanalOperacion = new CanalOperacion ();
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<CanalOperacion, bool>>>())).Returns(false);
            var resultado = target.ValidarCanalOperacion(CanalOperacion);
            Assert.NotNull(resultado);
            Assert.IsFalse(resultado.HayErrores);
        }

        [Test]
        public void EliminarCaunalOperacionError()
        {
            var CanalOperacion = new CanalOperacion();
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<CanalOperacion, bool>>>())).Returns(true);
            var resultado = target.ValidarCanalOperacion(CanalOperacion);
            Assert.NotNull(resultado);
            Assert.IsTrue(resultado.HayErrores);
        }
    }
}
