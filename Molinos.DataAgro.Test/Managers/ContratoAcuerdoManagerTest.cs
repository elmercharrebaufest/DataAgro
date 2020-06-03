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
    public class ContratoAcuerdoManagerTest
    {
        private Mock<ILogDataAgroManager> logDataAgroManagerMock;
        private ContratoAcuerdoManager target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ILogger> logger;
        private Mock<IDiasHabilesAgent> diasHabilesAgentMock;

        [SetUp]
        public void SetUp()
        {
            logger = new Mock<ILogger>();
            repositorioMock = new Mock<IRepositorio>();
            diasHabilesAgentMock = new Mock<IDiasHabilesAgent>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            logDataAgroManagerMock = new Mock<ILogDataAgroManager>();

            target = new ContratoAcuerdoManager(logger.Object, repositorioMock.Object, diasHabilesAgentMock.Object, logDataAgroManagerMock.Object);
        }
        [Test]
        public void BorrarAcuerdoTest()
        {
            var acuerdo = new ContratoAcuerdo
            {
                Id = 10,
                MotivoRechazo = "test"
            };
            repositorioMock.Setup(x => x.Obtener<ContratoAcuerdo>(It.IsAny<int>()))
                .Returns(new ContratoAcuerdo { Id = 1, Estado = new EstadoContrato { EstadoContratoId = 2 }, EstadoId = 2, MotivoRechazo = "test" });
            repositorioMock.Setup(x => x.Obtener<EstadoContrato>(It.IsAny<int>()))
                .Returns(new EstadoContrato { EstadoContratoId = 6 });
            var resultado = target.BorrarAcuerdo(acuerdo);

            Assert.That(!resultado.HayError);
            repositorioMock.Verify(x => x.Obtener<ContratoAcuerdo>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }
        [Test]
        public void GrabarAcuerdoOkTest()
        {
            var acuerdo = new ContratoAcuerdo
            {
                Id = 0,
                Precio = 10,
                Cantidad = 10,
                ComercialCreadorId = 1,
                DestinoId = 1,
                MaterialId = 1,
                CampanaId = 1,
                MonedaId = "a",
                FechaHasta = new DateTime(2019, 08, 08)
            };

            repositorioMock.Setup(x => x.Agregar(It.IsAny<ContratoAcuerdo>()));
            var resultado = target.GrabarAcuerdo(acuerdo);

            Assert.That(!resultado.HayError);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<ContratoAcuerdo>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }
        [Test]
        public void UpdateAcuerdoOkTest()
        {
            var acuerdo = new ContratoAcuerdo
            {
                Id = 10,
                Precio = 10,
                Cantidad = 10,
                ComercialCreadorId = 1,
                DestinoId = 1,
                CampanaId = 1,
                MaterialId = 1,
                MonedaId = "a",
                FechaHasta = new DateTime(2019, 08, 08)
            };

            repositorioMock.Setup(x => x.Obtener<ContratoAcuerdo>(It.IsAny<int>()))
                .Returns(new ContratoAcuerdo { Id = 10, Precio = 10, Cantidad = 10, ComercialCreadorId = 1, DestinoId = 1, MaterialId = 1, MonedaId = "a", FechaHasta = new DateTime(2019, 08, 08) });
            var resultado = target.GrabarAcuerdo(acuerdo);

            Assert.That(!resultado.HayError);
            repositorioMock.Verify(x => x.Obtener<ContratoAcuerdo>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }
        [Test]
        public void GrabarAcuerdoConErrorTest()
        {
            var acuerdo = new ContratoAcuerdo
            {
                Id = 10,
                Precio = -10,
                Cantidad = 0,
                ComercialCreadorId = 0,
                DestinoId = 0,
                MaterialId = 0,
                MonedaId = null,
                FechaHasta = new DateTime()
            };

            repositorioMock.Setup(x => x.Obtener<ContratoAcuerdo>(It.IsAny<int>()))
                .Returns(new ContratoAcuerdo { Id = 10, Precio = 10, Cantidad = 10, ComercialCreadorId = 1, DestinoId = 1, MaterialId = 1, MonedaId = "a", FechaHasta = new DateTime(2019, 08, 08) });
            var resultado = target.GrabarAcuerdo(acuerdo);

            Assert.That(resultado.HayError);
            Assert.AreEqual(8, resultado.Errores.Count);
            repositorioMock.Verify(x => x.Obtener<ContratoAcuerdo>(It.IsAny<int>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }

        [Test]
        public void TraerAcuerdoTest()
        {
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<ContratoAcuerdo, bool>>>(), It.IsAny<Expression<Func<ContratoAcuerdo, BasicoContrato>>>()))
                .Returns(new BasicoContrato { Id = 10 });
            var resultado = target.TraerAcuerdo(1);

            Assert.AreEqual(10, resultado.Id);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<ContratoAcuerdo, bool>>>(), It.IsAny<Expression<Func<ContratoAcuerdo, BasicoContrato>>>()), Times.Once);
        }
        [Test]
        public void TraerDatosCombo()
        {
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<Material, MaterialQry>>>(), It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<MaterialQry>() { new MaterialQry { MaterialId = 1, Descripcion = "a" } });
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<Centro, CentroQry>>>(), It.IsAny<Expression<Func<Centro, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<CentroQry>() { new CentroQry { Id = 1, Descripcion = "a" } });
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<Comercial, ComercialQry>>>(), It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<ComercialQry>() { new ComercialQry { ComercialId = 1, Nombre = "a" } });
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<Moneda, MonedaQry>>>(), It.IsAny<Expression<Func<Moneda, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<MonedaQry>() { new MonedaQry { MonedaId = "a", Descripcion = "a" } });
            var resultado = target.TraerDatosCombo();

            Assert.AreEqual(1, resultado.comercial.Count);
            Assert.AreEqual(1, resultado.material.Count);
            Assert.AreEqual(1, resultado.moneda.Count);
            Assert.AreEqual(1, resultado.destino.Count);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Material, MaterialQry>>>(), It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Centro, CentroQry>>>(), It.IsAny<Expression<Func<Centro, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Comercial, ComercialQry>>>(), It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Moneda, MonedaQry>>>(), It.IsAny<Expression<Func<Moneda, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
        }
        [Test]
        public void TraerTodoContratoAcuerdoTest()
        {
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<ContratoAcuerdo, ContratoAcuerdoIni>>>(), It.IsAny<Expression<Func<ContratoAcuerdo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<ContratoAcuerdoIni>() { new ContratoAcuerdoIni { Id = 1, Cantidad = 10 } });
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<Contrato>());
            var resultado = target.TraerTodoContratoAcuerdo();

            Assert.AreEqual(1, resultado.ContratoAcuerdo.Count);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ContratoAcuerdo, ContratoAcuerdoIni>>>(), It.IsAny<Expression<Func<ContratoAcuerdo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
        }
        [Test]
        public void ObtenerContratoAcuerdoParaAsociarTest()
        {
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<ContratoAcuerdo, bool>>>(), It.IsAny<Expression<Func<ContratoAcuerdo, ContratoAcuerdoDto>>>()))
                .Returns(new ContratoAcuerdoDto { Id = 1 });
            var resultado = target.ObtenerContratoAcuerdoParaAsociar(new DateTime(2019, 8, 8), 1, 1, 1);

            Assert.AreEqual(1, resultado.Id = 1);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<ContratoAcuerdo, bool>>>(), It.IsAny<Expression<Func<ContratoAcuerdo, ContratoAcuerdoDto>>>()), Times.Once);
        }
        [Test]
        public void ConfirmarContratoAcuerdoTest()
        {
            repositorioMock.Setup(x => x.Obtener<ContratoAcuerdo>(It.IsAny<int>()))
                .Returns(new ContratoAcuerdo { Id = 1, EstadoId = 1 });
            var resultado = target.ConfirmarContratoAcuerdo(1);

            Assert.That(!resultado.HayError);
            repositorioMock.Verify(x => x.Obtener<ContratoAcuerdo>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }
        [Test]
        public void FinalizarAcuerdoErrorFinalizadoTest()
        {
            repositorioMock.Setup(x => x.Obtener<ContratoAcuerdo>(It.IsAny<int>()))
                .Returns(new ContratoAcuerdo { Id = 1, EstadoId = 5 });
            repositorioMock.Setup(x => x.Obtener<EstadoContrato>(It.IsAny<int>()))
                .Returns(new EstadoContrato { EstadoContratoId = 5, Descripcion = "a" });
            var resultado = target.FinalizarAcuerdo(1);

            Assert.That(resultado.HayError);
            Assert.AreEqual("El Acuerdo ya se encuentra Finalizado", resultado.Errores[0].Message);
            repositorioMock.Verify(x => x.Obtener<ContratoAcuerdo>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<EstadoContrato>(It.IsAny<int>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }
        [Test]
        public void FinalizarAcuerdoErrorRechazadoTest()
        {
            repositorioMock.Setup(x => x.Obtener<ContratoAcuerdo>(It.IsAny<int>()))
                .Returns(new ContratoAcuerdo { Id = 1, EstadoId = 6 });
            repositorioMock.Setup(x => x.Obtener<EstadoContrato>(It.IsAny<int>()))
                .Returns(new EstadoContrato { EstadoContratoId = 5, Descripcion = "a" });
            var resultado = target.FinalizarAcuerdo(1);

            Assert.That(resultado.HayError);
            Assert.AreEqual("El Acuerdo ya ha sido Rechazado", resultado.Errores[0].Message);
            repositorioMock.Verify(x => x.Obtener<ContratoAcuerdo>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<EstadoContrato>(It.IsAny<int>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }
    }
}