using Autofac.Extras.NLog;
using KendoGridBinder;
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
    public class ConfiguracionInternaManagerTest
    {
        private ConfiguracionInternaManager target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ILogger> logger;
        private Mock<ILogDataAgroManager> logDataAgroMock;

        [SetUp]
        public void SetUp()
        {
            logger = new Mock<ILogger>();
            repositorioMock = new Mock<IRepositorio>();
            logDataAgroMock = new Mock<ILogDataAgroManager>();

            HttpContext.Current = Mock.FakeContext.FakeHttpContext();

            target = new ConfiguracionInternaManager(repositorioMock.Object, logger.Object, logDataAgroMock.Object);
        }
        [Test]
        public void GrabarPrecioTestOk()
        {
            var config = new PrecioMoa
            {
                Id = 0,
                DesdeVigencia = new DateTime(2099, 1, 30),
                HastaVigencia = new DateTime(2099, 1, 30),
                MaterialId = 1,
                MonedaId = "ARP",
                Precio = 120
            };
            var resultado = target.GrabarPrecio(config);

            Assert.IsTrue(resultado.HayError);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<PrecioMoa>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            Assert.AreEqual(1, resultado.Errores.Count);
        }
        [Test]
        public void GrabarPizarraTestOk()
        {
            var config = new HabilitacionPizarra
            {
                Id = 0,
                DesdeVigencia = new DateTime(2099, 1, 30, 00, 00, 00),
                HastaVigencia = new DateTime(2099, 1, 30, 23, 00, 00),
                MaterialId = 1,
                Dia = new DateTime(2099, 1, 30)
            };
            var resultado = target.GrabarPizarra(config);

            Assert.IsTrue(resultado.HayError);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<HabilitacionPizarra>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            Assert.AreEqual(1, resultado.Errores.Count);
        }
        [Test]
        public void GrabarFijacionTestOk()
        {
            var config = new HabilitacionFijacion
            {
                Id = 0,
                MaterialId = 1,
                Dia = new DateTime(2099, 1, 30)
            };
            var resultado = target.GrabarFijacion(config);

            Assert.IsTrue(resultado.HayError);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<HabilitacionFijacion>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            Assert.AreEqual(1, resultado.Errores.Count);
        }
        [Test]
        public void TraerPreciosTestOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PrecioMoa, PrecioMoaDto>>>(), It.IsAny<Expression<Func<PrecioMoa, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<PrecioMoaDto>() { new PrecioMoaDto { Id = 1 } });
            var resultado = target.TraerPrecios();

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<PrecioMoa, PrecioMoaDto>>>(), It.IsAny<Expression<Func<PrecioMoa, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            Assert.AreEqual(1, resultado.Count);
        }
        [Test]
        public void TraerFijacionesTestOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<HabilitacionFijacion, HabilitacionFijacionDto>>>(), It.IsAny<Expression<Func<HabilitacionFijacion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<HabilitacionFijacionDto>() { new HabilitacionFijacionDto { Id = 1 } });
            var resultado = target.TraerFijaciones();

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<HabilitacionFijacion, HabilitacionFijacionDto>>>(), It.IsAny<Expression<Func<HabilitacionFijacion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            Assert.AreEqual(1, resultado.Count);
        }
        [Test]
        public void TraerPizarraTestOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<HabilitacionPizarra, HabilitacionPizarraDto>>>(), It.IsAny<Expression<Func<HabilitacionPizarra, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<HabilitacionPizarraDto>() { new HabilitacionPizarraDto { Id = 1 } });
            var resultado = target.TraerPizarra();

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<HabilitacionPizarra, HabilitacionPizarraDto>>>(), It.IsAny<Expression<Func<HabilitacionPizarra, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            Assert.AreEqual(1, resultado.Count);
        }
        [Test]
        public void EliminarPrecioTestOk()
        {
            repositorioMock.Setup(y => y.Obtener<PrecioMoa>(It.IsAny<int>()))
                .Returns(new PrecioMoa { Id = 1 });
            var resultado = target.EliminarPrecio(1);

            repositorioMock.Verify(x => x.Obtener<PrecioMoa>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Remover(It.IsAny<PrecioMoa>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            Assert.IsTrue(resultado.HayError);
            Assert.AreEqual(1, resultado.Errores.Count);
        }
        [Test]
        public void EliminarPizarraTestOk()
        {
            repositorioMock.Setup(y => y.Obtener<HabilitacionPizarra>(It.IsAny<int>()))
                .Returns(new HabilitacionPizarra { Id = 1 });
            var resultado = target.EliminarPizarra(1);

            repositorioMock.Verify(x => x.Obtener<HabilitacionPizarra>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Remover(It.IsAny<HabilitacionPizarra>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            Assert.IsTrue(resultado.HayError);
            Assert.AreEqual(1, resultado.Errores.Count);
        }
        [Test]
        public void EliminarFijacionTestOk()
        {
            repositorioMock.Setup(y => y.Obtener<HabilitacionFijacion>(It.IsAny<int>()))
                .Returns(new HabilitacionFijacion { Id = 1 });
            var resultado = target.EliminarFijacion(1);

            repositorioMock.Verify(x => x.Obtener<HabilitacionFijacion>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Remover(It.IsAny<HabilitacionFijacion>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            Assert.IsTrue(resultado.HayError);
            Assert.AreEqual(1, resultado.Errores.Count);
        }

        [Test]
        public void TraerPrecioMoaCompraNetTestOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Material, MaterialDto>>>(), It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<MaterialDto>() { new MaterialDto { MaterialId = 1, Descripcion = "a" } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Moneda, MonedaDto>>>(), It.IsAny<Expression<Func<Moneda, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<MonedaDto>() { new MonedaDto { MonedaId = "a", Descripcion = "a" } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<HabilitacionPizarra, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<HabilitacionPizarra>() { new HabilitacionPizarra { MaterialId = 1, Id = 1 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PrecioMoa, PrecioMoaCompraNetDto>>>(), It.IsAny<Expression<Func<PrecioMoa, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
               .Returns(new List<PrecioMoaCompraNetDto>() { new PrecioMoaCompraNetDto {
                Material = "a",
                MaterialId = 1,
                Precio = 23,
                MonedaId = "a" } });
            var resultado = target.TraerPrecioCompraNet();

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Material, MaterialDto>>>(), It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Moneda, MonedaDto>>>(), It.IsAny<Expression<Func<Moneda, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<HabilitacionPizarra, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<PrecioMoa, PrecioMoaCompraNetDto>>>(), It.IsAny<Expression<Func<PrecioMoa, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            Assert.IsNotNull(resultado);
        }
        [Test]
        public void TraerPrecioCompraNetTestOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Moneda, string>>>(), It.IsAny<Expression<Func<Moneda, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<string>() { "a", "b" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<PrecioMoa, bool>>>(), It.IsAny<Expression<Func<PrecioMoa, PrecioMoaCompraNetDto>>>()))
               .Returns(new PrecioMoaCompraNetDto
               {
                   Material = "a",
                   MaterialId = 1,
                   Precio = 23,
                   MonedaId = "a"
               });
            var resultado = target.TraerPrecioCompraNet(1);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<PrecioMoa, bool>>>(), It.IsAny<Expression<Func<PrecioMoa, PrecioMoaCompraNetDto>>>()), Times.Exactly(2));
            Assert.IsNotNull(resultado);
        }
        [Test]
        public void HabilitarPizarraTestOk()
        {
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<HabilitacionPizarra, bool>>>()))
               .Returns(true);
            var resultado = target.HabilitarPizarra(1);

            repositorioMock.Verify(x => x.Existe(It.IsAny<Expression<Func<HabilitacionPizarra, bool>>>()), Times.Once);
            Assert.IsTrue(resultado);
        }
        [Test]
        public void TraerPrecioOk()
        {

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<PrecioMoa, bool>>>(), It.IsAny<Expression<Func<PrecioMoa, PrecioMoaDto>>>()))
                .Returns(new PrecioMoaDto { Id = 1 });

            var resultado = target.TraerPrecio(1);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<PrecioMoa, bool>>>(), It.IsAny<Expression<Func<PrecioMoa, PrecioMoaDto>>>()), Times.Once);

            Assert.NotNull(resultado);
            Assert.AreEqual(1, resultado.Id);
        }
        [Test]
        public void TraerPizarraOk()
        {

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<HabilitacionPizarra, bool>>>(), It.IsAny<Expression<Func<HabilitacionPizarra, HabilitacionPizarraDto>>>()))
                .Returns(new HabilitacionPizarraDto { Id = 1 });

            var resultado = target.TraerPizarra(1);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<HabilitacionPizarra, bool>>>(), It.IsAny<Expression<Func<HabilitacionPizarra, HabilitacionPizarraDto>>>()), Times.Once);

            Assert.NotNull(resultado);
            Assert.AreEqual(1, resultado.Id);
        }
        [Test]
        public void TraerFijacionOk()
        {

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<HabilitacionFijacion, bool>>>(), It.IsAny<Expression<Func<HabilitacionFijacion, HabilitacionFijacionDto>>>()))
                .Returns(new HabilitacionFijacionDto { Id = 1 });

            var resultado = target.TraerFijacion(1);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<HabilitacionFijacion, bool>>>(), It.IsAny<Expression<Func<HabilitacionFijacion, HabilitacionFijacionDto>>>()), Times.Once);

            Assert.NotNull(resultado);
            Assert.AreEqual(1, resultado.Id);
        }
    }
}