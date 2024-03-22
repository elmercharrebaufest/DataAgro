using Autofac.Extras.NLog;
using Kendo.DynamicLinq;
using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Business.Managers;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces.Agent;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
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
        private Mock<IClienteResearchAgent> clienteResearchAgentMock;


        [SetUp]
        public void SetUp()
        {
            logger = new Mock<ILogger>();
            repositorioMock = new Mock<IRepositorio>();
            clienteResearchAgentMock = new Mock<IClienteResearchAgent>();
            target = new ResearchManager(logger.Object, repositorioMock.Object, clienteResearchAgentMock.Object);
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
                Avance = 1,
                CambioAA = 4,
                ComercialId = 1,
                FechaHora = new DateTime(2018, 10, 26),
                Id = 1,
                IntencionSiembra = 3,
                LocalidadId = 4,
                MaterialId = 5
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
                Avance = 1,
                ComercialId = 1,
                FechaHora = new DateTime(2018, 10, 26),
                Id = 1,
                LocalidadId = 1,
                MaterialId = 1,
                RangoDesde = 1,
                RangoHasta = 2,
                Rendimiento = 2
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
                ComercialId = 1,
                EstadioId = 1,
                FechaHora = new DateTime(2018, 10, 26),
                Id = 1,
                LocalidadId = 4,
                MaterialId = 6,
                Situacion = ""
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
                ComercialId = 1,
                MaterialId = 1,
                LocalidadId = 1,
                Almacenado = 6,
                FechaHora = new DateTime(2018, 10, 26),
                Id = 1,
                VendidoAPrecio = 5,
                Observaciones = ""
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
        public void GrabarNotificacionError()
        {
            var notificacion = new NotificacionResearch();

            repositorioMock.Setup(x => x.Obtener<NotificacionResearch>(1)).Returns(notificacion);
            repositorioMock.Setup(x => x.Agregar(notificacion)).Returns(notificacion);
            repositorioMock.Setup(x => x.GuardarCambios());

            var result = target.GrabarNotificacion(notificacion);

            Assert.NotNull(result);

            repositorioMock.Verify(x => x.Obtener<NotificacionResearch>(It.IsAny<int>()), Times.Never);
            repositorioMock.Verify(x => x.Agregar(notificacion), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
            Assert.NotNull(result);
            Assert.IsTrue(result.HayErrores);
            Assert.AreEqual(4, result.Errores.Count);
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
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.IsTrue(result.HayErrores);
            Assert.AreEqual(200, result.ListaErrores[0].ErrorCode);

        }
        [Test]
        public void GrabarResearchAvanceSiembraError()
        {
            var researchAvanceSiembra = new ResearchAvanceSiembra();

            var result = target.GrabarResearchAvanceSiembra(researchAvanceSiembra, 1);

            Assert.NotNull(result);

            repositorioMock.Verify(x => x.Agregar(researchAvanceSiembra), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);

            Assert.IsTrue(result.HayErrores);
            Assert.AreEqual(3, result.Errores.Count);
        }
        [Test]
        public void GrabarResearchAvanceCosechaOk()
        {
            var researchAvanceCosecha = new ResearchAvanceCosecha
            {
                Avance = 1,
                Campania = new Campaña(),
                ComercialId = 1,
                FechaHora = new DateTime(2018, 10, 26),
                Id = 1,
                Observaciones = "a",
                LocalidadId = 4,
                MaterialId = 5,
                CampaniaId = 1,
                RangoDesde = 1,
                RangoHasta = 2,
                Rendimiento = 1
            };

            repositorioMock.Setup(x => x.Agregar(researchAvanceCosecha)).Returns(researchAvanceCosecha);
            repositorioMock.Setup(x => x.GuardarCambios());

            var result = target.GrabarResearchAvanceCosecha(researchAvanceCosecha, 1);

            Assert.NotNull(result);

            repositorioMock.Verify(x => x.Agregar(It.IsAny<ResearchAvanceCosecha>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios());

            Assert.IsTrue(result.HayErrores);
            Assert.AreEqual(200, result.ListaErrores[0].ErrorCode);

        }
        [Test]
        public void GrabarResearchAvanceCosechaError()
        {
            var researchAvanceC = new ResearchAvanceCosecha();

            var result = target.GrabarResearchAvanceCosecha(researchAvanceC, 1);

            Assert.NotNull(result);

            repositorioMock.Verify(x => x.Agregar(researchAvanceC), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);

            Assert.IsTrue(result.HayErrores);
            Assert.AreEqual(8, result.Errores.Count);
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
        public void GrabarResearchSituacionCultivoError()
        {
            var researchC = new ResearchSituacionCultivo();

            var result = target.GrabarResearchSituacionCultivo(researchC, 1);

            Assert.NotNull(result);

            repositorioMock.Verify(x => x.Agregar(researchC), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);

            Assert.IsTrue(result.HayErrores);
            Assert.AreEqual(5, result.Errores.Count);
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
        public void GrabarResearchVentaStockError()
        {
            var researchC = new ResearchVentaStock();

            var result = target.GrabarResearchVentaStock(researchC, 1);

            Assert.NotNull(result);

            repositorioMock.Verify(x => x.Agregar(researchC), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);

            Assert.IsTrue(result.HayErrores);
            Assert.AreEqual(5, result.Errores.Count);
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
        public void TraerTipoResearchOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TipoResearch, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
          .Returns(new List<TipoResearch>() { new TipoResearch { Id = 1 } });

            var result = target.TraerTipoResearch();

            Assert.NotNull(result);

            repositorioMock.Verify(y => y.Listar(It.IsAny<Expression<Func<TipoResearch, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);

            Assert.AreEqual(1, result.Count);
        }
        [Test]
        public void TraerResearchIdOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Research, int>>>(), It.IsAny<Expression<Func<Research, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
          .Returns(new List<int>() { 1 } );

            var result = target.TraerResearchId();

            Assert.NotNull(result);

            repositorioMock.Verify(y => y.Listar(It.IsAny<Expression<Func<Research, int>>>(), It.IsAny<Expression<Func<Research, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);

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
        [Test]
        public void TraerAvanceSiembra()
        {
            repositorioMock.Setup(x => x.ObtenerConsultaEscalar(It.IsAny<TraerAvanceSiembra>())).Returns(new KendoGrid<ResearchAvanceSiembraDto>(new List<ResearchAvanceSiembraDto>(), 1));
            var res = target.TraerAvanceSiembra(It.IsAny<KendoGridMvcRequest>());
            repositorioMock.Verify(x => x.ObtenerConsultaEscalar(It.IsAny<TraerAvanceSiembra>()), Times.Once);

        }

        [Test]
        public void TraerAvanceCosecha()
        {
            repositorioMock.Setup(x => x.ObtenerConsultaEscalar(It.IsAny<TraerAvanceCosecha>())).Returns(new KendoGrid<ResearchAvanceCosechaDto>(new List<ResearchAvanceCosechaDto>(), 1));
            var res = target.TraerAvanceCosecha(It.IsAny<KendoGridMvcRequest>());
            repositorioMock.Verify(x => x.ObtenerConsultaEscalar(It.IsAny<TraerAvanceCosecha>()), Times.Once);

        }
        [Test]
        public void TraerSituacionCultivoParcial()
        {
            repositorioMock.Setup(x => x.ObtenerConsultaEscalar(It.IsAny<TraerSituacionCultivoParcial>())).Returns(new KendoGrid<ResearchSituacionCultivoDto>(new List<ResearchSituacionCultivoDto>(), 1));
            var res = target.TraerSituacionCultivoParcial(It.IsAny<KendoGridMvcRequest>());
            repositorioMock.Verify(x => x.ObtenerConsultaEscalar(It.IsAny<TraerSituacionCultivoParcial>()), Times.Once);

        }

        [Test]
        public void TraerVentaStockTest()
        {
            repositorioMock.Setup(x => x.ObtenerConsultaEscalar(It.IsAny<TraerVentaStock>())).Returns(new KendoGrid<ResearchVentaStockDto>(new List<ResearchVentaStockDto>(), 1));
            var res = target.TraerVentaStock(It.IsAny<KendoGridMvcRequest>());
            repositorioMock.Verify(x => x.ObtenerConsultaEscalar(It.IsAny<TraerVentaStock>()), Times.Once);

        }

        [Test]
        public void BuscaDatosTabla_DeberiaRetornarDataSourceResult()
        {
            // Arrange
            var filtro = new DataSourceRequest();
            repositorioMock.Setup(r => r.ObtenerConsultaEscalar(It.IsAny<TraerResearchPorFiltro>()))
                           .Returns(new DataSourceResult());

            // Act
            var result = target.BuscaDatosTabla(filtro);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<DataSourceResult>(result);
        }

        [Test]
        public void TraerResearchCondicion_DeberiaRetornarListaResearchCondicion()
        {
            // Arrange
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ResearchCondicion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
          .Returns(new List<ResearchCondicion>() { new ResearchCondicion { CondicionId = 1 } });
            // Act
            var result = target.TraerResearchCondicion();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<List<ResearchCondicion>>(result);
        }

        [Test]
        public void TraerResearchEstadio_DeberiaRetornarListaResearchEstadio()
        {
            // Arrange
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ResearchEstadio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
          .Returns(new List<ResearchEstadio>() { new ResearchEstadio { EstadioId = 1 } });
            // Act
            var result = target.TraerResearchEstadio();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<List<ResearchEstadio>>(result);
        }

        [Test]
        public void BorrarResearch_DeberiaBorrarRegistroYRetornarResultado()
        {
            // Arrange
            var id = 1;
            var registroMock = new Mock<Research>();
            repositorioMock.Setup(r => r.Obtener<Research>(id)).Returns(registroMock.Object);

            // Act
            var result = target.BorrarResearch(id);

            // Assert
            repositorioMock.Verify(r => r.GuardarCambios(), Times.Once);
            Assert.IsNotNull(result);
            Assert.AreEqual(false, result.HayError);
        }

        [Test]
        public void TraerResearchTipoCarga_DeberiaRetornarListaResearchTipoCarga()
        {
            // Arrange
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ResearchTipoCarga, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
          .Returns(new List<ResearchTipoCarga>() { new ResearchTipoCarga { TipoCargaId = 1 } });
            // Act
            var result = target.TraerResearchTipoCarga();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<List<ResearchTipoCarga>>(result);
        }

        [Test]
        public void TraerResearchTipoMuestra_DeberiaRetornarListaResearchTipoMuestra()
        {
            // Arrange
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ResearchTipoMuestra, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
          .Returns(new List<ResearchTipoMuestra>() { new ResearchTipoMuestra { TipoMuestraId = 1 } });
            // Act
            var result = target.TraerResearchTipoMuestra();
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ResearchEstadio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
          .Returns(new List<ResearchEstadio>() { new ResearchEstadio { EstadioId = 1 } });
            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<List<ResearchTipoMuestra>>(result);
        }

        [Test]
        public void TraerResearchHumedadSuelo_DeberiaRetornarListaResearchHumedadSuelo()
        {
            // Arrange
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ResearchHumedadSuelo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
          .Returns(new List<ResearchHumedadSuelo>() { new ResearchHumedadSuelo { HumedadSueloId = 1 } });
            // Act
            var result = target.TraerResearchHumedadSuelo();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<List<ResearchHumedadSuelo>>(result);
        }

    }

}
