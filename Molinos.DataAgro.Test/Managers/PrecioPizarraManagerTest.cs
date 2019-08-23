using Autofac.Extras.NLog;
using Molinos.DataAgro.Business.Managers;
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
    public class PrecioPizarraManagerTest
    {
        private PrecioPizarraManager target;
        private Mock<IRepositorio> repositorioMock;
        private JavaScriptSerializer serializer;
        private Mock<ILogger> logger;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            logger = new Mock<ILogger>();
            repositorioMock = new Mock<IRepositorio>();
            target = new PrecioPizarraManager(repositorioMock.Object, logger.Object);
        }

        [Test]
        public void TraerTodoPrecioPizarraOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PrecioPizarra, PrecioPizarraDto>>>(), It.IsAny<Expression<Func<PrecioPizarra, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<PrecioPizarraDto>() { new PrecioPizarraDto { Id = 1 } });

            var resultado = target.TraerTodoPrecioPizarra();

            repositorioMock.Verify(y => y.Listar(It.IsAny<Expression<Func<PrecioPizarra, PrecioPizarraDto>>>(), It.IsAny<Expression<Func<PrecioPizarra, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()));

            Assert.NotNull(resultado);
            Assert.AreEqual(1, resultado.Count);
        }

        [Test]
        public void TraerTodoPrecioFiltroOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PrecioPizarra, PrecioPizarraDto>>>(), It.IsAny<Expression<Func<PrecioPizarra, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
            .Returns(new List<PrecioPizarraDto>() { new PrecioPizarraDto { Id = 1 } });

            var resultado = target.TraerTodoPrecioPizarraPorMaterialYPizarra(1, 1);

            repositorioMock.Verify(y => y.Listar(It.IsAny<Expression<Func<PrecioPizarra, PrecioPizarraDto>>>(), It.IsAny<Expression<Func<PrecioPizarra, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()));

            Assert.NotNull(resultado);
            Assert.AreEqual(1, resultado.Count);
        }

        public void TraerMonedaOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Moneda, MonedaDto>>>(), It.IsAny<Expression<Func<Moneda, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
            .Returns(new List<MonedaDto>() { new MonedaDto {  MonedaId = "ARP" } });

            var resultado = target.TraerTodoMoneda();

            repositorioMock.Verify(y => y.Listar(It.IsAny<Expression<Func<Moneda, MonedaDto>>>(), It.IsAny<Expression<Func<Moneda, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()));

            Assert.NotNull(resultado);
            Assert.AreEqual(1, resultado.Count);
        }

        [Test]
        public void GrabarPrecioPizarraOk()
        {
            var precioPizarra = new PrecioPizarra { Id = 1, MaterialId = 1, FechaDesde =new DateTime(2019,8, 6), FechaHasta = new DateTime(2019, 8, 6), Material = new Material(), Moneda = new Moneda(), MonedaId = "a", Pizarra = new Pizarra(), PizarraId = 1, Precio = 100, UnidadMedida = "" };
            repositorioMock.Setup(x => x.ObtenerMayor<PrecioPizarra, DateTime>(It.IsAny<Expression<Func<PrecioPizarra, bool>>>(), It.IsAny<Expression<Func<PrecioPizarra, DateTime>>>()))
                .Returns(new PrecioPizarra { FechaHasta = new DateTime(2019, 8, 5) });

            repositorioMock.Setup(x => x.Agregar<PrecioPizarra>(precioPizarra)).Returns(precioPizarra);
            repositorioMock.Setup(x => x.GuardarCambios());

            var resultado = target.GrabarPrecioPizarra(precioPizarra);

            repositorioMock.Verify(x => x.ObtenerMayor<PrecioPizarra, DateTime>(It.IsAny<Expression<Func<PrecioPizarra, bool>>>(), It.IsAny<Expression<Func<PrecioPizarra, DateTime>>>()));
            repositorioMock.Verify(x => x.Agregar(It.IsAny<PrecioPizarra>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            
            Assert.NotNull(resultado);
            Assert.IsTrue(resultado.HayErrores);
            Assert.AreEqual(200, resultado.ListaErrores[0].ErrorCode);
        }


    }
}
