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
    public class ZonaManagerTest
    {
        private ZonaManager target;
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

            target = new ZonaManager(logger.Object, repositorioMock.Object);
        }

        [Test]
        public void TraerDatosInicialesTestOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Zona, ZonaCombo>>>(), It.IsAny<Expression<Func<Zona, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<ZonaCombo>() { new ZonaCombo { Id = 1, Descripcion="1" } });
            var result = target.TraerDatosIniciales();
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Zona, ZonaCombo>>>(), It.IsAny<Expression<Func<Zona, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            Assert.NotNull(result);
            Assert.AreEqual(1, result.Zona.Count);
        }
        [Test]
        public void TraerTodoZonaTestOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Zona, ZonaIni>>>(), It.IsAny<Expression<Func<Zona, bool >>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<ZonaIni>() { new ZonaIni { Id = 1, Descripcion = "1" } });
            var result = target.TraerTodoZona();
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Zona, ZonaIni>>>(), It.IsAny<Expression<Func<Zona, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            Assert.NotNull(result);
            Assert.AreEqual(1, result.Zona.Count);
        }
        [Test]
        public void TraerZonaOk()
        {
            
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Zona, bool>>>(),It.IsAny<Expression<Func<Zona, ZonaDto>>>()))
                .Returns( new ZonaDto { Id=1 } );
            
            var resultado = target.TraerZona(1);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Zona, bool>>>(), It.IsAny<Expression<Func<Zona, ZonaDto>>>()), Times.Once);

            Assert.NotNull(resultado);
            Assert.AreEqual(1, resultado.Id);
        }
        [Test]
        public void GrabarZonaOk()
        {
            var prov = new Zona { Id = 1, Descripcion = "a", CodigoSap = "a" };
            repositorioMock.Setup(y => y.Obtener<Zona>(It.IsAny<int>()))
                .Returns( new Zona { Id = 1 });

            var resultado = target.GrabarZona(prov);
            repositorioMock.Verify(x => x.Obtener<Zona>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Zona>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.NotNull(resultado);
            Assert.IsFalse(resultado.HayErrores);
        }
        [Test]
        public void GrabarZonaNuevaOk()
        {
            var prov = new Zona { Id = 0, Descripcion = "a",CodigoSap="a" };
            repositorioMock.Setup(y => y.Obtener<Zona>(It.IsAny<int>()))
                .Returns(new Zona { Id = 1 });

            var resultado = target.GrabarZona(prov);
            repositorioMock.Verify(x => x.Obtener<Zona>(It.IsAny<int>()), Times.Never);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Zona>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.NotNull(resultado);
            Assert.IsFalse(resultado.HayErrores);
        }
        [Test]
        public void EliminarZonaOk()
        {
            var resultado = target.EliminarZona(1);
            repositorioMock.Verify(x => x.Remover<Zona>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.NotNull(resultado);
            Assert.IsFalse(resultado.HayErrores);
        }
    }
}
