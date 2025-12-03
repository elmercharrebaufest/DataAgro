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
    public class CampaniaActualManagerTest
    {
        private CampaniaActualManager target;
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

            target = new CampaniaActualManager(logger.Object, repositorioMock.Object);
        }

        [Test]
        public void ActualizacionCampaniaActualTestConCampania()
        {
            var fecha = new DateTime(2019, 10, 01);
            var agente = new CampaniaActual { Material = "a", Campania = "1-2" };
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Material, bool>>>()))
                            .Returns(new Material { CampañaId = 1, MaterialId = 2, Descripcion = "a" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Campaña, bool>>>()))
                            .Returns(new Campaña());
            var result = target.ActualizacionCampaniaActual(agente);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Material, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Campaña, bool>>>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Campaña>()), Times.Never);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<CampañaMaterialHistorico>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.NotNull(result);
            Assert.IsFalse(result.HayError);
        }
        [Test]
        public void ActualizacionCampaniaActualTestSinCampania()
        {
            var fecha = new DateTime(2019, 10, 01);
            var agente = new CampaniaActual { Material = "a", Campania = "1-2" };
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Material, bool>>>()))
                            .Returns(new Material { CampañaId = 1, MaterialId = 2, Descripcion = "a" });
            repositorioMock.Setup(y => y.Agregar(It.IsAny<Campaña>()))
                            .Returns(new Campaña { CampañaId = 1, Descripcion = "1-2" });
            var result = target.ActualizacionCampaniaActual(agente);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Material, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Campaña, bool>>>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Campaña>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.Agregar(It.IsAny<CampañaMaterialHistorico>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.NotNull(result);
            Assert.IsFalse(result.HayError);
        }
        [Test]
        public void ActualizacionCampaniaActualTestSinMaterial()
        {
            var fecha = new DateTime(2019, 10, 01);
            var agente = new CampaniaActual { Material = "a", Campania = "1-2" };
            var result = target.ActualizacionCampaniaActual(agente);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Material, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Campaña, bool>>>()), Times.Never);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Campaña>()), Times.Never);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<CampañaMaterialHistorico>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);

            Assert.NotNull(result);
            Assert.IsTrue(result.HayError);
            Assert.AreEqual("No existe el material.",result.ListaErrores[0].Message);
        }
    }
}
