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
    public class CentroManagerTest
    {
        private CentroManager target;
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

            target = new CentroManager(logger.Object, repositorioMock.Object);
        }

        [Test]
        public void TraerDatosInicialesTestOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Centro, CentroCombo>>>(), It.IsAny<Expression<Func<Centro, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<CentroCombo>() { new CentroCombo { Id = 1, Descripcion="1" } });
            var result = target.TraerDatosIniciales();
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Centro, CentroCombo>>>(), It.IsAny<Expression<Func<Centro, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            Assert.NotNull(result);
            Assert.AreEqual(1, result.Centro.Count);
        }
        [Test]
        public void TraerTodoCentroTestOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Centro, CentroIni>>>(), It.IsAny<Expression<Func<Centro, bool >>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<CentroIni>() { new CentroIni { Id = 1, Descripcion = "1" } });
            var result = target.TraerTodoCentro();
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Centro, CentroIni>>>(), It.IsAny<Expression<Func<Centro, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            Assert.NotNull(result);
            Assert.AreEqual(1, result.Centro.Count);
        }
        [Test]
        public void TraerCentroOk()
        {
            
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Centro, bool>>>(),It.IsAny<Expression<Func<Centro, CentroDto>>>()))
                .Returns( new CentroDto { Id=1 } );
            
            var resultado = target.TraerCentro(1);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Centro, bool>>>(), It.IsAny<Expression<Func<Centro, CentroDto>>>()), Times.Once);

            Assert.NotNull(resultado);
            Assert.AreEqual(1, resultado.Id);
        }
        [Test]
        public void GrabarCentroOk()
        {
            var prov = new Centro { Id = 1, Descripcion = "a", CodigoSap = "a" };
            repositorioMock.Setup(y => y.Obtener<Centro>(It.IsAny<int>()))
                .Returns( new Centro { Id = 1 });

            var resultado = target.GrabarCentro(prov);
            repositorioMock.Verify(x => x.Obtener<Centro>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Centro>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.NotNull(resultado);
            Assert.IsFalse(resultado.HayErrores);
        }
        [Test]
        public void GrabarCentroNuevaOk()
        {
            var prov = new Centro { Id = 0, Descripcion = "a",CodigoSap="a" };
            repositorioMock.Setup(y => y.Obtener<Centro>(It.IsAny<int>()))
                .Returns(new Centro { Id = 1 });

            var resultado = target.GrabarCentro(prov);
            repositorioMock.Verify(x => x.Obtener<Centro>(It.IsAny<int>()), Times.Never);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Centro>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.NotNull(resultado);
            Assert.IsFalse(resultado.HayErrores);
        }
        [Test]
        public void EliminarCentroOk()
        {
            var resultado = target.EliminarCentro(1);
            repositorioMock.Verify(x => x.Remover<Centro>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.NotNull(resultado);
            Assert.IsFalse(resultado.HayErrores);
        }

        [Test]
        public void ObtenerCentroPorCodigoSapTest()
        {
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Centro, bool>>>(), It.IsAny<Expression<Func<Centro, CentroDto>>>()))
               .Returns(new CentroDto { Id = 1, CodigoSap = "1600" });
            var resultado = target.ObtenerCentroPorCodigoSap("1600");            
            Assert.NotNull(resultado);
        }
    }
}
