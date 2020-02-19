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
    public class AgenteCompraManagerTest
    {
        private AgenteCompraManager target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ILogger> logger;
        private Mock<IHedgeManager> hedgeManagerMock;

        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            logger = new Mock<ILogger>();
            repositorioMock = new Mock<IRepositorio>();
            hedgeManagerMock = new Mock<IHedgeManager>();

            HttpContext.Current = Mock.FakeContext.FakeHttpContext();

            target = new AgenteCompraManager(logger.Object, repositorioMock.Object,hedgeManagerMock.Object);
        }

        [Test]
        public void GrabarAgenteTestOk()
        {
            var fecha = new DateTime(2019, 10, 01);
            var agente = new AgenteCompra { Id = 1, Cantidad = 1, ComercialCreadorId = 1, ComercialId = 1, EstadoId = 1, MaterialId = 1, MonedaId = "1", OperadorId = 1, Fecha = fecha, Posicion = "1", Precio = 1, TipoAgenteCompraId = 1, CampanaId=1 };
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<RangoPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<RangoPrecio>());
            repositorioMock.Setup(y => y.Obtener<AgenteCompra>(It.IsAny<int>()))
                            .Returns( new AgenteCompra());
            var result = target.GrabarAgente(agente);

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<RangoPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);

            repositorioMock.Verify(x => x.Obtener<AgenteCompra>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<AgenteCompra>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.NotNull(result);
            Assert.IsFalse(result.HayError);
        }
        [Test]
        public void GrabarAgenteNuevoTestOk()
        {
            var fecha = new DateTime(2019, 10, 01);
            var agente = new AgenteCompra { Id = 0, Cantidad = 1, ComercialCreadorId = 1, ComercialId = 1, EstadoId = 1, MaterialId = 1, MonedaId = "1", OperadorId = 1, Fecha = fecha, Posicion = "1", Precio = 1, TipoAgenteCompraId = 1,CampanaId =1 };
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<RangoPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<RangoPrecio>());
            repositorioMock.Setup(y => y.Obtener<AgenteCompra>(It.IsAny<int>()))
                            .Returns(new AgenteCompra());
            var result = target.GrabarAgente(agente);

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<RangoPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);

            repositorioMock.Verify(x => x.Obtener<AgenteCompra>(It.IsAny<int>()), Times.Never);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<AgenteCompra>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.NotNull(result);
            Assert.IsFalse(result.HayError);
        }
        [Test]
        public void GrabarAgenteTestConError()
        {
            var fecha = new DateTime(2019, 10, 01);
            var agente = new AgenteCompra { Id = 1 };
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<RangoPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<RangoPrecio>());
            repositorioMock.Setup(y => y.Obtener<AgenteCompra>(It.IsAny<int>()))
                            .Returns(new AgenteCompra());
            var result = target.GrabarAgente(agente);

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<RangoPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);

            repositorioMock.Verify(x => x.Obtener<AgenteCompra>(It.IsAny<int>()), Times.Never);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<AgenteCompra>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);

            Assert.NotNull(result);
            Assert.IsTrue(result.HayError);
            Assert.AreEqual(8,result.ListaErrores.Count);
        }
        [Test]
        public void FinalizarAgenteTestOk()
        {
            var fecha = new DateTime(2019, 10, 01);
            var agente = new AgenteCompra { Id = 0, Cantidad = 1,Ampliaciones=1, ComercialCreadorId = 1, ComercialId = 1, EstadoId = 2, MaterialId = 1, MonedaId = "1", OperadorId = 1, Fecha = fecha, Posicion = "1", Precio = 1, TipoAgenteCompraId = 1 };
            
            repositorioMock.Setup(y => y.Obtener<AgenteCompra>(It.IsAny<int>()))
                            .Returns(agente);
            repositorioMock.Setup(y => y.Obtener<EstadoContrato>(It.IsAny<int>()))
                            .Returns(new EstadoContrato { Descripcion="a",EstadoContratoId=5,Orden=1});
            var result = target.FinalizarAgente(1);

            repositorioMock.Verify(x => x.Obtener<AgenteCompra>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<EstadoContrato>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.NotNull(result);
            Assert.IsFalse(result.HayError);
        }
        [Test]
        public void FinalizarAgenteFinalizadoTest()
        {
            var fecha = new DateTime(2019, 10, 01);
            var agente = new AgenteCompra { Id = 0, Cantidad = 1, Ampliaciones = 1, ComercialCreadorId = 1, ComercialId = 1, EstadoId = 5, MaterialId = 1, MonedaId = "1", OperadorId = 1, Fecha = fecha, Posicion = "1", Precio = 1, TipoAgenteCompraId = 1 };

            repositorioMock.Setup(y => y.Obtener<AgenteCompra>(It.IsAny<int>()))
                            .Returns(agente);
            repositorioMock.Setup(y => y.Obtener<EstadoContrato>(It.IsAny<int>()))
                            .Returns(new EstadoContrato { Descripcion = "a", EstadoContratoId = 5, Orden = 1 });
            var result = target.FinalizarAgente(1);

            repositorioMock.Verify(x => x.Obtener<AgenteCompra>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<EstadoContrato>(It.IsAny<int>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);

            Assert.NotNull(result);
            Assert.IsTrue(result.HayError);
            Assert.AreEqual("Agente de Compras ya se encuentra Finalizadao", result.ListaErrores[0].Message);
        }
        [Test]
        public void FinalizarAgenterechazadoTest()
        {
            var fecha = new DateTime(2019, 10, 01);
            var agente = new AgenteCompra { Id = 0, Cantidad = 1, Ampliaciones = 1, ComercialCreadorId = 1, ComercialId = 1, EstadoId = 6, MaterialId = 1, MonedaId = "1", OperadorId = 1, Fecha = fecha, Posicion = "1", Precio = 1, TipoAgenteCompraId = 1 };

            repositorioMock.Setup(y => y.Obtener<AgenteCompra>(It.IsAny<int>()))
                            .Returns(agente);
            repositorioMock.Setup(y => y.Obtener<EstadoContrato>(It.IsAny<int>()))
                            .Returns(new EstadoContrato { Descripcion = "a", EstadoContratoId = 5, Orden = 1 });
            var result = target.FinalizarAgente(1);

            repositorioMock.Verify(x => x.Obtener<AgenteCompra>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<EstadoContrato>(It.IsAny<int>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);

            Assert.NotNull(result);
            Assert.IsTrue(result.HayError);
            Assert.AreEqual("Agente de Compras ya ha sido Rechazado", result.ListaErrores[0].Message);
        }
        [Test]
        public void BorrarAgenteTestOk()
        {
            var fecha = new DateTime(2019, 10, 01);
            var agente = new AgenteCompra { Id = 0, Cantidad = 1, Ampliaciones = 1, ComercialCreadorId = 1, ComercialId = 1, EstadoId = 2, MaterialId = 1, MonedaId = "1", OperadorId = 1, Fecha = fecha, Posicion = "1", Precio = 1, TipoAgenteCompraId = 1 };

            repositorioMock.Setup(y => y.Obtener<AgenteCompra>(It.IsAny<int>()))
                            .Returns(agente);
            repositorioMock.Setup(y => y.Obtener<EstadoContrato>(It.IsAny<int>()))
                            .Returns(new EstadoContrato { Descripcion = "a", EstadoContratoId = 5, Orden = 1 });
            var result = target.BorrarAgente(agente);

            repositorioMock.Verify(x => x.Obtener<AgenteCompra>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<EstadoContrato>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.NotNull(result);
            Assert.IsFalse(result.HayError);
        }
        [Test]
        public void BorrarAgenteTestError()
        {
            var fecha = new DateTime(2019, 10, 01);
            var agente = new AgenteCompra { Id = 0, Cantidad = 1, Ampliaciones = 1, ComercialCreadorId = 1, ComercialId = 1, EstadoId = 6, MaterialId = 1, MonedaId = "1", OperadorId = 1, Fecha = fecha, Posicion = "1", Precio = 1, TipoAgenteCompraId = 1 };

            repositorioMock.Setup(y => y.Obtener<AgenteCompra>(It.IsAny<int>()))
                            .Returns(agente);
            repositorioMock.Setup(y => y.Obtener<EstadoContrato>(It.IsAny<int>()))
                            .Returns(new EstadoContrato { Descripcion = "a", EstadoContratoId = 6, Orden = 1 });
            var result = target.BorrarAgente(agente);

            repositorioMock.Verify(x => x.Obtener<AgenteCompra>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<EstadoContrato>(It.IsAny<int>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);

            Assert.NotNull(result);
            Assert.IsTrue(result.HayError);
            Assert.AreEqual("Agente de Compras no se puede rechazar", result.ListaErrores[0].Message);
        }
        [Test]
        public void TraerAgenteTestOk()
        {
            var fecha = new DateTime(2019, 10, 01);
            var agente = new AgenteCompra { Id = 0, Cantidad = 1, Ampliaciones = 1, ComercialCreadorId = 1, ComercialId = 1, EstadoId = 6, MaterialId = 1, MonedaId = "1", OperadorId = 1, Fecha = fecha, Posicion = "1", Precio = 1, TipoAgenteCompraId = 1 };

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<AgenteCompra, bool>>>(),It.IsAny<Expression<Func<AgenteCompra, BasicoContrato>>>()))
                            .Returns(new BasicoContrato { Id=1});
            var result = target.TraerAgente(1);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<AgenteCompra, bool>>>(), It.IsAny<Expression<Func<AgenteCompra, BasicoContrato>>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<EstadoContrato>(It.IsAny<int>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);

            Assert.NotNull(result);
            Assert.AreEqual(1, result.Id);
        }
        [Test]
        public void GrabarAmpliacionAgenteTestOk()
        {
            var fecha = new DateTime(2019, 10, 01);
            var agente = new AgenteCompra { Id = 0, Cantidad = 1, Ampliaciones = 1, ComercialCreadorId = 1, ComercialId = 1, EstadoId = 3, MaterialId = 1, MonedaId = "1", OperadorId = 1, Fecha = fecha, Posicion = "1", Precio = 1, TipoAgenteCompraId = 1 };

            repositorioMock.Setup(y => y.Obtener<AgenteCompra>(It.IsAny<int>()))
                            .Returns(new AgenteCompra { Id = 1, EstadoId = 3 });
            hedgeManagerMock.Setup(x => x.Dia()).Returns(new FinDelDiaDto());
            var result = target.GrabarAmpliacionAgente(agente);

            repositorioMock.Verify(x => x.Obtener<AgenteCompra>(It.IsAny<int>()), Times.Once);
            hedgeManagerMock.Verify(x => x.Dia(), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.NotNull(result);
            Assert.IsFalse(result.HayError);
        }
        [Test]
        public void GrabarAmpliacionAgenteTestDiaCerrado()
        {
            var fecha = new DateTime(2019, 10, 01);
            var agente = new AgenteCompra { Id = 0, Cantidad = 1, Ampliaciones = 1, ComercialCreadorId = 1, ComercialId = 1, EstadoId = 3, MaterialId = 1, MonedaId = "1", OperadorId = 1, Fecha = fecha, Posicion = "1", Precio = 1, TipoAgenteCompraId = 1 };

            repositorioMock.Setup(y => y.Obtener<AgenteCompra>(It.IsAny<int>()))
                            .Returns(new AgenteCompra { Id = 1, EstadoId = 3 });
            hedgeManagerMock.Setup(x => x.Dia()).Returns(new FinDelDiaDto { Cerrado= true});
            var result = target.GrabarAmpliacionAgente(agente);

            repositorioMock.Verify(x => x.Obtener<AgenteCompra>(It.IsAny<int>()), Times.Once);
            hedgeManagerMock.Verify(x => x.Dia(), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);

            Assert.NotNull(result);
            Assert.IsTrue(result.HayError);
            Assert.AreEqual("El Día de Operación ya se ha cerrado", result.Errores[0].Message);
        }
        [Test]
        public void GrabarAmpliacionAgenteTestFinalizado()
        {
            var fecha = new DateTime(2019, 10, 01);
            var agente = new AgenteCompra { Id = 0, Cantidad = 1, Ampliaciones = 1, ComercialCreadorId = 1, ComercialId = 1, EstadoId = 6, MaterialId = 1, MonedaId = "1", OperadorId = 1, Fecha = fecha, Posicion = "1", Precio = 1, TipoAgenteCompraId = 1 };

            repositorioMock.Setup(y => y.Obtener<AgenteCompra>(It.IsAny<int>()))
                            .Returns(new AgenteCompra { Id = 1, EstadoId = 6 });
            hedgeManagerMock.Setup(x => x.Dia()).Returns(new FinDelDiaDto { Cerrado = false });
            var result = target.GrabarAmpliacionAgente(agente);

            repositorioMock.Verify(x => x.Obtener<AgenteCompra>(It.IsAny<int>()), Times.Once);
            hedgeManagerMock.Verify(x => x.Dia(), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);

            Assert.NotNull(result);
            Assert.IsTrue(result.HayError);
            Assert.AreEqual("El Agente de Compras no se puede modificar", result.Errores[0].Message);
        }
    }
}
