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
        private Mock<IDiasHabilesAgent> diasHabilesAgentMock;

        [SetUp]
        public void SetUp()
        {
            logger = new Mock<ILogger>();
            repositorioMock = new Mock<IRepositorio>();
            logDataAgroMock = new Mock<ILogDataAgroManager>();
            diasHabilesAgentMock = new Mock<IDiasHabilesAgent>();

            HttpContext.Current = Mock.FakeContext.FakeHttpContext();

            target = new ConfiguracionInternaManager(repositorioMock.Object, logger.Object, logDataAgroMock.Object, diasHabilesAgentMock.Object);
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
                TipoNegocioId = 3,
                MonedaId = "ARP",
                Precio = 120,
                DesdeEntrega = new DateTime(2099, 1, 30),
                DesdeFijacion = new DateTime(2099, 1, 30),
                HastaEntrega = new DateTime(2099, 1, 30),
                HastaFijacion = new DateTime(2099, 1, 30),
            };
            var resultado = target.GrabarPrecio(config, "");

            Assert.IsTrue(resultado.HayError);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<PrecioMoa>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            Assert.AreEqual(1, resultado.Errores.Count);
        }
        [Test]
        public void GrabarPrecioTestOkNegocioAPrecio()
        {
            var config = new PrecioMoa
            {
                Id = 0,
                DesdeVigencia = new DateTime(2099, 1, 30),
                HastaVigencia = new DateTime(2099, 1, 30),
                MaterialId = 1,
                TipoNegocioId = 1,
                MonedaId = "ARP",
                Precio = 120,
                DesdeEntrega = new DateTime(2099, 1, 30),
                DesdeFijacion = new DateTime(2099, 1, 30),
                HastaEntrega = new DateTime(2099, 1, 30),
                HastaFijacion = new DateTime(2099, 1, 30),
            };
            var resultado = target.GrabarPrecio(config, "");

            Assert.IsTrue(resultado.HayError);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<PrecioMoa>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            Assert.AreEqual(1, resultado.Errores.Count);
        }
        [Test]
        public void GrabarPrecioTestOkNegocioAFijar()
        {
            var config = new PrecioMoa
            {
                Id = 0,
                DesdeVigencia = new DateTime(2099, 1, 30),
                HastaVigencia = new DateTime(2099, 1, 30),
                MaterialId = 1,
                TipoNegocioId = 3,
                MonedaId = "ARP",
                Precio = 120,
                DesdeEntrega = new DateTime(2099, 1, 30),
                DesdeFijacion = new DateTime(2099, 1, 30),
                HastaEntrega = new DateTime(2099, 1, 30),
                HastaFijacion = new DateTime(2099, 1, 30),
            };
            var resultado = target.GrabarPrecio(config, "");

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
                DesdeEntrega = new DateTime(2099, 1, 30),
                HastaEntrega = new DateTime(2099, 1, 30),
                TipoNegocioId = 3,
                MaterialId = 1,
                Dia = new DateTime(2099, 1, 30)
            };
            var resultado = target.GrabarPizarra(config, "", new DateTime(2099, 1, 30));

            Assert.IsTrue(resultado.HayError);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<HabilitacionPizarra>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            Assert.AreEqual(1, resultado.Errores.Count);
        }
        [Test]
        public void GrabarCampañaTestOk()
        {
            var config = new HabilitacionCampaña
            {
                Id = 0,                
                MaterialId = 1,
                CampañaId=1
            };
            var resultado = target.GrabarCampaña(config);

            Assert.IsTrue(resultado.HayError);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<HabilitacionCampaña>()), Times.Once);
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
            diasHabilesAgentMock.Setup(y => y.UltimoDiaHabil(null)).Returns(DateTime.Now.Date);
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
        public void TraerCampañaTestOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<HabilitacionCampaña, HabilitacionCampañaDto>>>(), It.IsAny<Expression<Func<HabilitacionCampaña, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<HabilitacionCampañaDto>() { new HabilitacionCampañaDto { Id = 1 } });
            var resultado = target.TraerCampaña();

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<HabilitacionCampaña, HabilitacionCampañaDto>>>(), It.IsAny<Expression<Func<HabilitacionCampaña, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
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
        public void EliminarCampañaTestOk()
        {
            repositorioMock.Setup(y => y.Obtener<HabilitacionCampaña>(It.IsAny<int>()))
                .Returns(new HabilitacionCampaña { Id = 1 });
            var resultado = target.EliminarCampaña(1);

            repositorioMock.Verify(x => x.Obtener<HabilitacionCampaña>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Remover(It.IsAny<HabilitacionCampaña>()), Times.Once);
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
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TipoNegocio, TipoNegocioDto>>>(), It.IsAny<Expression<Func<TipoNegocio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<TipoNegocioDto>() { new TipoNegocioDto { TipoNegocioId = 1, Descripcion = "a" } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Material, MaterialDto>>>(), It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<MaterialDto>() { new MaterialDto { MaterialId = 1, Descripcion = "a" } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<EstadoPrecioMOA, EstadoPrecioMOADto>>>(), It.IsAny<Expression<Func<EstadoPrecioMOA, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
           .Returns(new List<EstadoPrecioMOADto>() { new EstadoPrecioMOADto { MaterialId = 1, Descripcion = "a" } });
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
        public void TraerCampañaOk()
        {

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<HabilitacionCampaña, bool>>>(), It.IsAny<Expression<Func<HabilitacionCampaña, HabilitacionCampañaDto>>>()))
                .Returns(new HabilitacionCampañaDto { Id = 1 });

            var resultado = target.TraerCampaña(1);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<HabilitacionCampaña, bool>>>(), It.IsAny<Expression<Func<HabilitacionCampaña, HabilitacionCampañaDto>>>()), Times.Once);

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

        [Test]
        public void TraerCampañasTestOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<HabilitacionCampaña, HabilitacionCampañaDto>>>(), It.IsAny<Expression<Func<HabilitacionCampaña, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<HabilitacionCampañaDto>() { new HabilitacionCampañaDto { Id = 1 } });
            var resultado = target.TraerCampaña();

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<HabilitacionCampaña, HabilitacionCampañaDto>>>(), It.IsAny<Expression<Func<HabilitacionCampaña, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            Assert.AreEqual(1, resultado.Count);
        }

        [Test]
        public void HabilitarPizarraExternoOk()
        {

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<HabilitacionPizarra, bool>>>(), It.IsAny<Expression<Func<HabilitacionPizarra, HabilitacionPizarraDto>>>()))
                .Returns(new HabilitacionPizarraDto { Id = 1 });
            var resultado = target.HabilitarPizarraExterno(1, 1);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<HabilitacionPizarra, bool>>>(), It.IsAny<Expression<Func<HabilitacionPizarra, HabilitacionPizarraDto>>>()), Times.Once);
            Assert.NotNull(resultado);
            Assert.AreEqual(1, resultado.Id);
        }

        [Test]
        public void HabilitarCampañaExterno()
        {

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<HabilitacionCampaña, HabilitacionCampañaDto>>>(), It.IsAny<Expression<Func<HabilitacionCampaña, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                 .Returns(new List<HabilitacionCampañaDto>() { new HabilitacionCampañaDto { Id = 1 } });
            var resultado = target.HabilitarCampañaExterno(1);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<HabilitacionCampaña, HabilitacionCampañaDto>>>(), It.IsAny<Expression<Func<HabilitacionCampaña, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            Assert.NotNull(resultado);
            Assert.AreEqual(1, resultado.Count);
        }

        [Test]
        public void TraerSustentableOk()
        {

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<HabilitacionSustentable, bool>>>(), It.IsAny<Expression<Func<HabilitacionSustentable, HabilitacionSustentableDto>>>()))
                .Returns(new HabilitacionSustentableDto { Id = 1 });

            var resultado = target.TraerSustentable(1);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<HabilitacionSustentable, bool>>>(), It.IsAny<Expression<Func<HabilitacionSustentable, HabilitacionSustentableDto>>>()), Times.Once);

            Assert.NotNull(resultado);
            Assert.AreEqual(1, resultado.Id);
        }

        [Test]
        public void TraerSustentablesTestOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<HabilitacionSustentable, HabilitacionSustentableDto>>>(), It.IsAny<Expression<Func<HabilitacionSustentable, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<HabilitacionSustentableDto>() { new HabilitacionSustentableDto { Id = 1 } });
            var resultado = target.TraerSustentables();

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<HabilitacionSustentable, HabilitacionSustentableDto>>>(), It.IsAny<Expression<Func<HabilitacionSustentable, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            Assert.AreEqual(1, resultado.Count);
        }

        [Test]
        public void GrabarSustentableTestOk()
        {
            var config = new HabilitacionSustentable
            {
                Id = 0,
                DesdeVigencia = new DateTime(2099, 1, 30),
                HastaVigencia = new DateTime(2099, 1, 30),
                TipoNegocioId = 3,
                MonedaId = "USDM",
                Precio = 120,
                DesdeEntrega = new DateTime(2099, 1, 30),
                HastaEntrega = new DateTime(2099, 1, 30),
            };
            var resultado = target.GrabarSustentable(config, "");

            Assert.IsTrue(resultado.HayError);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<HabilitacionSustentable>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            Assert.AreEqual(1, resultado.Errores.Count);
        }
        [Test]
        public void EliminarSustentableTestOk()
        {
            repositorioMock.Setup(y => y.Obtener<HabilitacionSustentable>(It.IsAny<int>()))
                .Returns(new HabilitacionSustentable { Id = 1 });
            var resultado = target.EliminarSustentable(1);

            repositorioMock.Verify(x => x.Obtener<HabilitacionSustentable>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Remover(It.IsAny<HabilitacionSustentable>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            Assert.IsTrue(resultado.HayError);
            Assert.AreEqual(1, resultado.Errores.Count);
        }

        [Test]
        public void ActualizarPrecioTestOk()
        {
            repositorioMock.Setup(y => y.Obtener<PrecioMoa>(It.IsAny<int>()))
                .Returns(new PrecioMoa { Id = 1 });
            var resultado = target.ActualizarPrecio(1,1,"a");

            repositorioMock.Verify(x => x.Obtener<PrecioMoa>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            Assert.IsFalse(resultado.HayError);
            Assert.AreEqual(0, resultado.Errores.Count);
        }

        [Test]
        public void GrabarPagoDiferidoTestOk()
        {
            var config = new HabilitacionPagoDiferido
            {
                Id = 0,
                CantidadDia = 1,
                Tasa = 20,
                DesdeVigencia = new DateTime(2099, 1, 30),
                HastaVigencia = new DateTime(2099, 1, 30),
               
            };
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<HabilitacionPagoDiferido, bool>>>()))
             .Returns(false);

            var resultado = target.GrabarPagoDiferido(config, "");

            Assert.IsTrue(resultado.HayError);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<HabilitacionPagoDiferido>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            Assert.AreEqual(1, resultado.Errores.Count);
        }

        [Test]
        public void GrabarPagoDiferidoTestError()
        {
            var config = new HabilitacionPagoDiferido
            {
                Id = 0,
                CantidadDia = 0,
                Tasa = 10,
                DesdeVigencia = new DateTime(2099, 1, 30),
                HastaVigencia = new DateTime(2011, 1, 30),

            };
            repositorioMock.Setup(y => y.Existe(It.IsAny<Expression<Func<HabilitacionPagoDiferido, bool>>>()))
             .Returns(true);

            var resultado = target.GrabarPagoDiferido(config, "");

            Assert.IsTrue(resultado.HayError);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<HabilitacionPagoDiferido>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
            Assert.AreEqual(4, resultado.Errores.Count);
        }

        [Test]
        public void EliminarHabilitacionPagoDiferidoTestOk()
        {
            repositorioMock.Setup(y => y.Obtener<HabilitacionPagoDiferido>(It.IsAny<int>()))
                .Returns(new HabilitacionPagoDiferido { Id = 1 });
            var resultado = target.EliminarHabilitacionPagoDiferido(1);

            repositorioMock.Verify(x => x.Obtener<HabilitacionPagoDiferido>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            Assert.IsTrue(resultado.HayError);
            Assert.AreEqual(1, resultado.Errores.Count);
        }
        
    }
}