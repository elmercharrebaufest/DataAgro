using Autofac.Extras.NLog;
using Molinos.DataAgro.Business;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using System.Web.Script.Serialization;

namespace Molinos.DataAgro.Test.Managers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class DiferencialManagerTest
    {
        private DiferencialManager target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ILogger> logger;
        private Mock<IHedgeManager> hedgeManagerMock;
        private Mock<IMailManager> mailManagerManagerMock;
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            logger = new Mock<ILogger>();
            repositorioMock = new Mock<IRepositorio>();
            hedgeManagerMock = new Mock<IHedgeManager>();
            mailManagerManagerMock = new Mock<IMailManager>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            target = new DiferencialManager(logger.Object, repositorioMock.Object, mailManagerManagerMock.Object, hedgeManagerMock.Object);
        }

        [Test]
        public void TraerDiferencialTestOk()
        {
            repositorioMock.Setup(y => y.ObtenerMayor(It.IsAny<Expression<Func<Diferencial, bool>>>(), It.IsAny<Expression<Func<Diferencial, int>>>(),
                It.IsAny<Expression<Func<Diferencial, DiferencialDto>>>()))
                .Returns(new DiferencialDto() { Id = 1, Comercial = "aaa", DiferencialDefault = 1, Fecha = new DateTime(2020, 01, 08) });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Diferencial, DiferencialDto>>>(), It.IsAny<Expression<Func<Diferencial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
              .Returns(new List<DiferencialDto>() { new DiferencialDto { Id = 1 } });
            var resultado = target.TraerDiferencial();
            repositorioMock.Verify(y => y.Listar(It.IsAny<Expression<Func<Diferencial, DiferencialDto>>>(), It.IsAny<Expression<Func<Diferencial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);

            Assert.NotNull(resultado);
            Assert.AreEqual(1, resultado.Id);

        }

        [Test]
        public void GrabarDiferencialOk()
        {
            var diferencial = new Diferencial()
            {
                DiferencialDefault = 1
            };
            repositorioMock.Setup(y => y.ObtenerMayor(It.IsAny<Expression<Func<Diferencial, bool>>>(),
                It.IsAny<Expression<Func<Diferencial, int>>>())).Returns(new Diferencial() { DiferencialDefault = 2 });
            var result = target.GrabarDiferencial(diferencial) as Resultado;
            repositorioMock.Verify(y => y.ObtenerMayor(It.IsAny<Expression<Func<Diferencial, bool>>>(),
                It.IsAny<Expression<Func<Diferencial, int>>>()), Times.Once);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Diferencial>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            Assert.NotNull(result);
            Assert.IsFalse(result.HayError);

        }

        [Test]
        public void GrabarDiferencialConErrorDefault()
        {
            var diferencial = new Diferencial()
            {
                DiferencialDefault = 0
            };
            repositorioMock.Setup(y => y.ObtenerMayor(It.IsAny<Expression<Func<Diferencial, bool>>>(),
                It.IsAny<Expression<Func<Diferencial, int>>>())).Returns(new Diferencial() { DiferencialDefault = 2 });
            var result = target.GrabarDiferencial(diferencial) as Resultado;
            Assert.NotNull(result);
            Assert.IsFalse(!result.HayError);

        }
        [Test]
        public void GrabarDiferencialConError()
        {
            var diferencial = new Diferencial()
            {
                DiferencialDefault = 2
            };
            repositorioMock.Setup(y => y.ObtenerMayor(It.IsAny<Expression<Func<Diferencial, bool>>>(),
                It.IsAny<Expression<Func<Diferencial, int>>>())).Returns(new Diferencial() { DiferencialDefault = 2 });
            var result = target.GrabarDiferencial(diferencial) as Resultado;
            Assert.NotNull(result);
            Assert.IsFalse(!result.HayError);

        }
        [Test]
        public void EliminarDiferencialOk()
        {
            repositorioMock.Setup(y => y.Obtener<Diferencial>(It.IsAny<int>())).Returns(new Diferencial { Id = 1 });
            var resultado = target.EliminarDiferencial(1);
            repositorioMock.Verify(x => x.Obtener<Diferencial>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Remover(It.IsAny<Diferencial>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }



    }
}