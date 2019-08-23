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
    public class ResearchManagerTest
    {

        private ResearchManager target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ILogger> logger;


        [SetUp]
        public void SetUp()
        {
            logger = new Mock<ILogger>();
            repositorioMock = new Mock<IRepositorio>();
            target = new ResearchManager(logger.Object, repositorioMock.Object);
        }

        [Test]
        public void EliminarNotificacionOk()
        {
            repositorioMock.Setup(x => x.Remover<NotificacionResearch>(1));
            repositorioMock.Setup(x => x.GuardarCambios());

            var result = target.EliminarNotificacion(1);

            Assert.NotNull(result);

            repositorioMock.Verify(x => x.Remover<NotificacionResearch>(1), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.IsTrue(result.HayErrores);
            Assert.AreEqual(200, result.ListaErrores[0].ErrorCode);
        }

        [Test]
        public void EliminarResearchAvanceSiembraOk()
        {
            var researchAvanceSiembra = new ResearchAvanceSiembra
            {
                Avance = 1, CambioAA = 4, ComercialId = 1, FechaHora = new DateTime(2018, 10, 26), Id = 1, IntencionSiembra = 3, LocalidadId = 4, MaterialId = 5
            };

            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<ResearchAvanceSiembra, bool>>>())).Returns(researchAvanceSiembra);
            repositorioMock.Setup(x => x.Remover(researchAvanceSiembra));
            repositorioMock.Setup(x => x.GuardarCambios());

            var result = target.EliminarResearchAvanceSiembra(1);

            Assert.NotNull(result);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<ResearchAvanceSiembra, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Remover(researchAvanceSiembra), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.IsTrue(result.HayErrores);
            Assert.AreEqual(200, result.ListaErrores[0].ErrorCode);


        }
        [Test]
        public void EliminarResearchAvanceCosechaOk()
        {
            var researchAvanceCosecha = new ResearchAvanceCosecha
            {
                Avance = 1, ComercialId = 1, FechaHora = new DateTime(2018, 10, 26), Id = 1, LocalidadId = 1, MaterialId = 1, RangoDesde = 1, RangoHasta = 2, Rendimiento = 2
            };

            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<ResearchAvanceCosecha, bool>>>())).Returns(researchAvanceCosecha);
            repositorioMock.Setup(x => x.Remover(researchAvanceCosecha));
            repositorioMock.Setup(x => x.GuardarCambios());

            var result = target.EliminarResearchAvanceCosecha(1);

            Assert.NotNull(result);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<ResearchAvanceCosecha, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Remover(researchAvanceCosecha), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.IsTrue(result.HayErrores);
            Assert.AreEqual(200, result.ListaErrores[0].ErrorCode);
        }
        [Test]
        public void EliminarResearchSituacionCultivoOk()
        {
            var researchSituacionCultivo = new ResearchSituacionCultivo
            {
                ComercialId = 1, EstadioId = 1, FechaHora = new DateTime(2018, 10, 26), Id = 1, LocalidadId = 4, MaterialId = 6, Situacion = ""
            };

            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<ResearchSituacionCultivo, bool>>>())).Returns(researchSituacionCultivo);
            repositorioMock.Setup(x => x.Remover(researchSituacionCultivo));
            repositorioMock.Setup(x => x.GuardarCambios());

            var result = target.EliminarResearchSituacionCultivo(1);

            Assert.NotNull(result);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<ResearchSituacionCultivo, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Remover(researchSituacionCultivo), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.IsTrue(result.HayErrores);
            Assert.AreEqual(200, result.ListaErrores[0].ErrorCode);
        }

        [Test]
        public void EliminarResearchVentaStockOk()
        {
            var researchVentaStock = new ResearchVentaStock
            {
                ComercialId = 1, MaterialId = 1, LocalidadId = 1, Almacenado = 6, FechaHora = new DateTime(2018, 10, 26), Id = 1, VendidoAPrecio = 5, Observaciones = ""
            };

            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<ResearchVentaStock, bool>>>())).Returns(researchVentaStock);
            repositorioMock.Setup(x => x.Remover(researchVentaStock));
            repositorioMock.Setup(x => x.GuardarCambios());

            var result = target.EliminarResearchVentaStock(1);

            Assert.NotNull(result);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<ResearchVentaStock, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Remover(researchVentaStock), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.IsTrue(result.HayErrores);
            Assert.AreEqual(200, result.ListaErrores[0].ErrorCode);
        }
        [Test]
        public void ActualizarNotificacionOk()
        {
            var notificacion = new NotificacionResearch
            {
                Id = 1,
                CampanaId = 1,
                FechaDesde = new DateTime(2018, 10, 26),
                FechaHasta = new DateTime(2018, 10, 26),
                MaterialId = 1,
                TipoResearchId = 1,
                Mensaje = "aaa"
            };

            repositorioMock.Setup(x => x.Obtener<NotificacionResearch>(1)).Returns(notificacion);
            repositorioMock.Setup(x => x.Agregar(notificacion)).Returns(notificacion);
            repositorioMock.Setup(x => x.GuardarCambios());

            var result = target.GrabarNotificacion(notificacion);

            Assert.NotNull(result);

            repositorioMock.Verify(x => x.Obtener<NotificacionResearch>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Agregar(notificacion), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.IsFalse(result.HayErrores);
        }
        [Test]
        public void GrabarNotificacionOk()
        {
            var notificacion = new NotificacionResearch
            {
                Id = 0,
                CampanaId = 1,
                FechaDesde = new DateTime(2018, 10, 26),
                FechaHasta = new DateTime(2018, 10, 26),
                MaterialId = 1,
                TipoResearchId = 1,
                Mensaje = "aaa"
            };

            repositorioMock.Setup(x => x.Obtener<NotificacionResearch>(1)).Returns(notificacion);
            repositorioMock.Setup(x => x.Agregar(notificacion)).Returns(notificacion);
            repositorioMock.Setup(x => x.GuardarCambios());

            var result = target.GrabarNotificacion(notificacion);

            Assert.NotNull(result);

            repositorioMock.Verify(x => x.Obtener<NotificacionResearch>(It.IsAny<int>()), Times.Never);
            repositorioMock.Verify(x => x.Agregar(notificacion), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.IsFalse(result.HayErrores);
        }


        [Test]
        public void GrabarResearchAvanceSiembraOk()
        {
            var researchAvanceSiembra = new ResearchAvanceSiembra
            {
                Avance = 1,
                CambioAA = 4,
                ComercialId = 1,
                FechaHora = new DateTime(2018, 10, 26),
                Id = 1,
                IntencionSiembra = 3,
                LocalidadId = 4,
                MaterialId = 5,
                CampaniaId = 1                
            };

            repositorioMock.Setup(x => x.Agregar(researchAvanceSiembra)).Returns(researchAvanceSiembra);
            repositorioMock.Setup(x => x.GuardarCambios());

            var result = target.GrabarResearchAvanceSiembra(researchAvanceSiembra, 1);

            Assert.NotNull(result);

            repositorioMock.Verify(x => x.Agregar(researchAvanceSiembra), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios());

            Assert.IsTrue(result.HayErrores);
            Assert.AreEqual(200, result.ListaErrores[0].ErrorCode);

        }
        [Test]
        public void GrabarResearchSituacionCultivo()
        {
            var researchSituacionCultivo = new ResearchSituacionCultivo
            {
                ComercialId = 1,
                EstadioId = 1,
                FechaHora = new DateTime(2018, 10, 26),
                Id = 1,
                LocalidadId = 4,
                MaterialId = 6,
                Situacion = "aa",
                CampaniaId = 1
            };

            repositorioMock.Setup(x => x.Agregar(researchSituacionCultivo)).Returns(researchSituacionCultivo);
            repositorioMock.Setup(x => x.GuardarCambios());

            var result = target.GrabarResearchSituacionCultivo(researchSituacionCultivo, 1);

            Assert.NotNull(result);

            repositorioMock.Verify(x => x.Agregar(researchSituacionCultivo), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios());

            Assert.IsTrue(result.HayErrores);
            Assert.AreEqual(200, result.ListaErrores[0].ErrorCode);

        }

        [Test]
        public void GrabarResearchVentaStockOk()
        {
            var researchVentaStock = new ResearchVentaStock
            {
                ComercialId = 1,
                MaterialId = 1,
                LocalidadId = 1,
                Almacenado = 6,
                FechaHora = new DateTime(2018, 10, 26),
                Id = 1,
                VendidoAPrecio = 5,
                Observaciones = "",
                CampaniaId = 1
            };

            repositorioMock.Setup(x => x.Agregar(researchVentaStock)).Returns(researchVentaStock);
            repositorioMock.Setup(x => x.GuardarCambios());

            var result = target.GrabarResearchVentaStock(researchVentaStock, 1);

            Assert.NotNull(result);

            repositorioMock.Verify(x => x.Agregar(researchVentaStock), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios());

            Assert.IsTrue(result.HayErrores);
            Assert.AreEqual(200, result.ListaErrores[0].ErrorCode);


        }

        [Test]
        public void TraerTodoResearchAvanceSiembraOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ResearchAvanceSiembra, ResearchAvanceSiembraDto>>>(), It.IsAny<Expression<Func<ResearchAvanceSiembra, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
            .Returns(new List<ResearchAvanceSiembraDto>() { new ResearchAvanceSiembraDto { Id = 1 } });

            var result = target.TraerTodoResearchAvanceSiembra();

            Assert.NotNull(result);

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ResearchAvanceSiembra, ResearchAvanceSiembraDto>>>(), It.IsAny<Expression<Func<ResearchAvanceSiembra, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);

            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void TraerTodoResearchAvanceCosechaOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ResearchAvanceCosecha, ResearchAvanceCosechaDto>>>(), It.IsAny<Expression<Func<ResearchAvanceCosecha, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
            .Returns(new List<ResearchAvanceCosechaDto>() { new ResearchAvanceCosechaDto { Id = 1 } });

            var result = target.TraerTodoResearchAvanceCosecha();

            Assert.NotNull(result);

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ResearchAvanceCosecha, ResearchAvanceCosechaDto>>>(), It.IsAny<Expression<Func<ResearchAvanceCosecha, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);

            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void TraerTodoResearchSituacionCultivoOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ResearchSituacionCultivo, ResearchSituacionCultivoDto>>>(), It.IsAny<Expression<Func<ResearchSituacionCultivo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
           .Returns(new List<ResearchSituacionCultivoDto>() { new ResearchSituacionCultivoDto { Id = 1 } });

            var result = target.TraerTodoResearchSituacionCultivo();

            Assert.NotNull(result);

            repositorioMock.Verify(y => y.Listar(It.IsAny<Expression<Func<ResearchSituacionCultivo, ResearchSituacionCultivoDto>>>(), It.IsAny<Expression<Func<ResearchSituacionCultivo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);

            Assert.AreEqual(1, result.Count);

        }

        [Test]
        public void TraerTodoResearchVentaStockOk()
        {
           repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ResearchVentaStock, ResearchVentaStockDto>>>(), It.IsAny<Expression<Func<ResearchVentaStock, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
          .Returns(new List<ResearchVentaStockDto>() { new ResearchVentaStockDto { Id = 1 } });

            var result = target.TraerTodoResearchVentaStock();

            Assert.NotNull(result);

            repositorioMock.Verify(y => y.Listar(It.IsAny<Expression<Func<ResearchVentaStock, ResearchVentaStockDto>>>(), It.IsAny<Expression<Func<ResearchVentaStock, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);

            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void TraerTodasNotificacionesOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<NotificacionResearch, NotificacionResearchDto>>>(), It.IsAny<Expression<Func<NotificacionResearch, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
          .Returns(new List<NotificacionResearchDto>() { new NotificacionResearchDto { Id = 1 } });

            var result = target.TraerTodasNotificaciones();

            Assert.NotNull(result);

            repositorioMock.Verify(y => y.Listar(It.IsAny<Expression<Func<NotificacionResearch, NotificacionResearchDto>>>(), It.IsAny<Expression<Func<NotificacionResearch, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);

            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void TraerResearchOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TipoResearch, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
          .Returns(new List<TipoResearch>() { new TipoResearch { Id = 1 } });

            var result = target.TraerResearch();

            Assert.NotNull(result);

            repositorioMock.Verify(y => y.Listar(It.IsAny<Expression<Func<TipoResearch, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);

            Assert.AreEqual(1, result.Count);
        }
        [Test]
        public void TraerNotificacioneoOk()
        {
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<NotificacionResearch, bool>>>(), It.IsAny<Expression<Func<NotificacionResearch, NotificacionResearchDto>>>()))
            .Returns(new NotificacionResearchDto { Id = 1 });

            var result = target.TraerNotificaciones(It.IsAny<int>());

            Assert.NotNull(result);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<NotificacionResearch, bool>>>(), It.IsAny<Expression<Func<NotificacionResearch, NotificacionResearchDto>>>()), Times.Once);

        }


    }
        
}
