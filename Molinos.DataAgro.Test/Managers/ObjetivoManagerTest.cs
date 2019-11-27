using Autofac.Extras.NLog;
using Molinos.DataAgro.Business;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Repository;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Molinos.DataAgro.Test.Managers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class ObjetivoManagerTest
    {
        private ObjetivoManager target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ILogger> loggerMock;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            loggerMock = new Mock<ILogger>();
            target = new ObjetivoManager(loggerMock.Object,repositorioMock.Object);
        }

        [Test]
        public void TraerObjetivoHomeTest()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ObjetivoComercial, MaterialObjetivo>>>(), It.IsAny<Expression<Func<ObjetivoComercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<MaterialObjetivo>() { new MaterialObjetivo { Id = 1, Campana = "1", Comercial = "a", ComercialId = 1, Material = "a", MaterialId = 1, Toneladas = 1 } });

            var resultado = target.TraerObjetivoHome(1, new List<int>());

            repositorioMock.Verify(y => y.Listar(It.IsAny<Expression<Func<ObjetivoComercial, MaterialObjetivo>>>(), It.IsAny<Expression<Func<ObjetivoComercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()));

            Assert.NotNull(resultado);
            Assert.AreEqual(1, resultado.Objetivos.Count);
            Assert.AreEqual(1, resultado.Comerciales.Count);
        }
        [Test]
        public void GuardarObjetivoTestOk()
        {
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<ObjetivoComercial, bool>>>())).Returns(false);

            var resultado = target.GuardarObjetivo(new ObjetivoComercial() { MaterialId=1,ComercialId=1,CampanaId=1,ToneladasObjetivos=1});

            repositorioMock.Verify(y => y.Existe(It.IsAny<Expression<Func<ObjetivoComercial, bool>>>()),Times.Once);
            repositorioMock.Verify(y => y.Agregar(It.IsAny<ObjetivoComercial>()), Times.Once);
            repositorioMock.Verify(y => y.GuardarCambios(), Times.Once);

            Assert.NotNull(resultado);
            Assert.IsFalse(resultado.HayError);
        }
        [Test]
        public void GuardarObjetivoTestError()
        {
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<ObjetivoComercial, bool>>>())).Returns(true);

            var resultado = target.GuardarObjetivo(new ObjetivoComercial());

            repositorioMock.Verify(y => y.Existe(It.IsAny<Expression<Func<ObjetivoComercial, bool>>>()), Times.Once);
            repositorioMock.Verify(y => y.Agregar(It.IsAny<ObjetivoComercial>()), Times.Never);
            repositorioMock.Verify(y => y.GuardarCambios(), Times.Never);

            Assert.NotNull(resultado);
            Assert.IsTrue(resultado.HayError);
            Assert.AreEqual(5,resultado.Errores.Count);
        }
        [Test]
        public void EliminarObjetivoTestOk()
        {
            var resultado = target.EliminarObjetivo(1);

            repositorioMock.Verify(y => y.Remover<ObjetivoComercial>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(y => y.GuardarCambios(), Times.Once);

            Assert.NotNull(resultado);
            Assert.IsFalse(resultado.HayError);
        }
    }    
}
