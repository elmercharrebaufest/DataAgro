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
    public class HedgeManagerTest
    {
        private HedgeManager target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ILogger> logger;
        private Mock<IMailManager> mailManagerMock;
        private Mock<IReportesManager> reportesManagerMock;
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            logger = new Mock<ILogger>();
            repositorioMock = new Mock<IRepositorio>();
            mailManagerMock = new Mock<IMailManager>();
            reportesManagerMock = new Mock<IReportesManager>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();

            target = new HedgeManager(logger.Object, repositorioMock.Object,
                mailManagerMock.Object, reportesManagerMock.Object);
        }

        [Test]
        public void DiaTestOk()
        {
            repositorioMock.Setup(x => x.ObtenerMayor(It.IsAny<Expression<Func<FinDelDia, bool>>>(), It.IsAny<Expression<Func<FinDelDia, DateTime>>>(), It.IsAny<Expression<Func<FinDelDia, FinDelDiaDto>>>()))
                .Returns(new FinDelDiaDto { Cerrado = false, Diferencial = 1000 });
            var result = target.Dia();

            Assert.NotNull(result);
        }
        [Test]
        public void TraerTodosHedgeMaterialOk()
        {

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<HedgeMaterial, HedgeMaterialDto>>>(), It.IsAny<Expression<Func<HedgeMaterial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<HedgeMaterialDto>() { new HedgeMaterialDto { Id = 1 } });

            var resultado = target.TraerTodosHedgeMaterial();
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<HedgeMaterial, HedgeMaterialDto>>>(), It.IsAny<Expression<Func<HedgeMaterial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);

            Assert.NotNull(resultado);
            Assert.AreEqual(1, resultado.Count);
        }
        [Test]
        public void TraerTodosHedgeObjetivoOk()
        {

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<HedgeObjetivo, HedgeObjetivoDto>>>(), It.IsAny<Expression<Func<HedgeObjetivo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<HedgeObjetivoDto>() { new HedgeObjetivoDto { Id = 1 } });

            var resultado = target.TraerTodosHedgeObjetivo();
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<HedgeObjetivo, HedgeObjetivoDto>>>(), It.IsAny<Expression<Func<HedgeObjetivo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);

            Assert.NotNull(resultado);
            Assert.AreEqual(1, resultado.Count);
        }
        [Test]
        public void TraerTodosHedgeTCOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<HedgeTC, HedgeTCDto>>>(), It.IsAny<Expression<Func<HedgeTC, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<HedgeTCDto>() { new HedgeTCDto { Id = 1 } });

            var resultado = target.TraerTodosHedgeTC();
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<HedgeTC, HedgeTCDto>>>(), It.IsAny<Expression<Func<HedgeTC, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);

            Assert.NotNull(resultado);
            Assert.AreEqual(1, resultado.Count);
        }
        [Test]
        public void TraerTodosHedgeMargenMoliendaOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<HedgeMargenMolienda, HedgeMargenMoliendaDto>>>(), It.IsAny<Expression<Func<HedgeMargenMolienda, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<HedgeMargenMoliendaDto>() { new HedgeMargenMoliendaDto { Id = 1 } });

            var resultado = target.TraerTodosHedgeMargenMolienda();
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<HedgeMargenMolienda, HedgeMargenMoliendaDto>>>(), It.IsAny<Expression<Func<HedgeMargenMolienda, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);

            Assert.NotNull(resultado);
            Assert.AreEqual(1, resultado.Count);
        }
        [Test]
        public void GrabarHedgeMaterialOk()
        {
            var fecha = new DateTime(2019, 04, 20);
            var hed = new List<HedgeMaterial>() { new HedgeMaterial { Id = 1, Cantidad = 1, ComercialId = 1, Fecha = fecha }, new HedgeMaterial { Cantidad = 0, ComercialId = 1, Fecha = fecha } };

            repositorioMock.Setup(x => x.ObtenerMayor(It.IsAny<Expression<Func<FinDelDia, bool>>>(), It.IsAny<Expression<Func<FinDelDia, DateTime>>>(), It.IsAny<Expression<Func<FinDelDia, FinDelDiaDto>>>()))
                .Returns(new FinDelDiaDto { Cerrado = false, Diferencial = 1000 });

            var resultado = target.GrabarHedgeMaterial(hed, 1);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<HedgeMaterial>()), Times.Once);

            Assert.NotNull(resultado);
            Assert.IsTrue(resultado.HayErrores);
            Assert.AreEqual(200, resultado.ListaErrores[0].ErrorCode);
        }
        [Test]
        public void GrabarHedgeObjetivoOk()
        {
            var fecha = new DateTime(2019, 04, 20);
            var hed = new List<HedgeObjetivo>() { new HedgeObjetivo { Id = 1, Cantidad = 1, ComercialId = 1, Fecha = fecha }, new HedgeObjetivo { Cantidad = 0, ComercialId = 1, Fecha = fecha } };

            repositorioMock.Setup(x => x.ObtenerMayor(It.IsAny<Expression<Func<FinDelDia, bool>>>(), It.IsAny<Expression<Func<FinDelDia, DateTime>>>(), It.IsAny<Expression<Func<FinDelDia, FinDelDiaDto>>>()))
                .Returns(new FinDelDiaDto { Cerrado = false, Diferencial = 1000 });

            var resultado = target.GrabarHedgeObjetivo(hed, 1);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<HedgeObjetivo>()), Times.Once);

            Assert.NotNull(resultado);
            Assert.IsTrue(resultado.HayErrores);
            Assert.AreEqual(200, resultado.ListaErrores[0].ErrorCode);
        }
        [Test]
        public void GrabarHedgeTCOk()
        {
            var fecha = new DateTime(2019, 04, 20);
            var hed = new HedgeTC { Id = 1, HedgePesos = 1, TipoCambio = 1, ComercialId = 1, Fecha = fecha };

            repositorioMock.Setup(x => x.ObtenerMayor(It.IsAny<Expression<Func<FinDelDia, bool>>>(), It.IsAny<Expression<Func<FinDelDia, DateTime>>>(), It.IsAny<Expression<Func<FinDelDia, FinDelDiaDto>>>()))
                .Returns(new FinDelDiaDto { Cerrado = false, Diferencial = 1000 });

            var resultado = target.GrabarHedgeTC(hed, 1);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<HedgeTC>()), Times.Once);

            Assert.NotNull(resultado);
            Assert.IsTrue(resultado.HayErrores);
            Assert.AreEqual(200, resultado.ListaErrores[0].ErrorCode);
        }
        [Test]
        public void GrabarHedgeMargenMoliendaOk()
        {
            var fecha = new DateTime(2022, 08, 20);
            var hed = new HedgeMargenMolienda { Id = 1, MargenMolienda = 1, ComercialId = 1, Fecha = fecha };

            repositorioMock.Setup(x => x.ObtenerMayor(It.IsAny<Expression<Func<FinDelDia, bool>>>(), It.IsAny<Expression<Func<FinDelDia, DateTime>>>(), It.IsAny<Expression<Func<FinDelDia, FinDelDiaDto>>>()))
                .Returns(new FinDelDiaDto { Cerrado = false, Diferencial = 1000 });

            var resultado = target.GrabarHedgeMargenMolienda(hed, 1);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<HedgeMargenMolienda>()), Times.Once);

            Assert.NotNull(resultado);
            Assert.IsTrue(resultado.HayErrores);
            Assert.AreEqual(200, resultado.ListaErrores[0].ErrorCode);
        }
        [Test]
        public void EliminarHedgeTCOk()
        {
            var fecha = new DateTime(2019, 04, 20);
            var hed = new HedgeTC { Id = 1, HedgePesos = 1, TipoCambio = 1, ComercialId = 1, Fecha = fecha };

            repositorioMock.Setup(x => x.ObtenerMayor(It.IsAny<Expression<Func<FinDelDia, bool>>>(), It.IsAny<Expression<Func<FinDelDia, DateTime>>>(), It.IsAny<Expression<Func<FinDelDia, FinDelDiaDto>>>()))
                .Returns(new FinDelDiaDto { Cerrado = false, Diferencial = 1000 });
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<HedgeTC, bool>>>()))
                .Returns(hed);

            var resultado = target.EliminarHedgeTC(1);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<HedgeTC, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Remover(It.IsAny<HedgeTC>()), Times.Once);

            Assert.NotNull(resultado);
            Assert.IsTrue(resultado.HayErrores);
            Assert.AreEqual(200, resultado.ListaErrores[0].ErrorCode);
        }
        [Test]
        public void CerrarDiaOk()
        {
            var fecha = new DateTime(2019, 04, 20);
            var arc = new byte[10];
            var hed = new HedgeTC { Id = 1, HedgePesos = 1, TipoCambio = 1, ComercialId = 1, Fecha = fecha };

            repositorioMock.Setup(x => x.ObtenerMayor(It.IsAny<Expression<Func<FinDelDia, bool>>>(), It.IsAny<Expression<Func<FinDelDia, DateTime>>>(), It.IsAny<Expression<Func<FinDelDia, FinDelDiaDto>>>()))
                .Returns(new FinDelDiaDto { Cerrado = false, Diferencial = 1000 });

            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<FinDelDia, bool>>>()))
                .Returns(new FinDelDia { Id = 1, ComercialId = 1 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<Contrato>() { new Contrato { Id = 1 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<FijacionDePrecioContrato>() { new FijacionDePrecioContrato { Id = 1 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Fason, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<Fason>() { new Fason { Id = 1 } });

            var resultado = target.CerrarDia(1, arc, false, "a", 10);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<FinDelDia>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(2));
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Fason, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);

            Assert.NotNull(resultado);
            Assert.IsFalse(resultado.HayErrores);
        }
        [Test]
        public void ReabrirDiaOk()
        {
            var fecha = new DateTime(2019, 04, 20);
            var arc = new byte[10];
            var hed = new HedgeTC { Id = 1, HedgePesos = 1, TipoCambio = 1, ComercialId = 1, Fecha = fecha };

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<FinDelDia, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<FinDelDia>() { new FinDelDia { Id = 1 } });

            var resultado = target.ReabrirDia(1, 10);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<FinDelDia, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);

            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.NotNull(resultado);
            Assert.IsFalse(resultado.HayErrores);
        }
        [Test]
        public void DiferencialMayorOk()
        {
            var fecha = new DateTime(2019, 04, 20);
            var arc = new byte[10];
            var hed = new HedgeTC { Id = 1, HedgePesos = 1, TipoCambio = 1, ComercialId = 1, Fecha = fecha };


            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<FinDelDia, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<FinDelDia>() { new FinDelDia { Id = 1, Diferencial = 10 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<Contrato>() { new Contrato { Id = 1, Cantidad = 10 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<FijacionDePrecioContrato>() { new FijacionDePrecioContrato { Id = 1, Cantidad = 10 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Fason, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<Fason>() { new Fason { Id = 1, Cantidad = 10 } });

            var resultado = target.Diferencial();
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<FinDelDia, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Fason, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);


            Assert.NotNull(resultado);
            Assert.IsTrue(resultado.HayErrores);
            Assert.AreEqual(1, resultado.ListaErrores[0].ErrorCode);
        }
        [Test]
        public void DiferencialMenorOk()
        {
            var fecha = new DateTime(2019, 04, 20);
            var arc = new byte[10];
            var hed = new HedgeTC { Id = 1, HedgePesos = 1, TipoCambio = 1, ComercialId = 1, Fecha = fecha };


            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<FinDelDia, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<FinDelDia>() { new FinDelDia { Id = 1, Diferencial = 100 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<Contrato>() { new Contrato { Id = 1, Cantidad = 10 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<FijacionDePrecioContrato>() { new FijacionDePrecioContrato { Id = 1, Cantidad = 10 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Fason, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<Fason>() { new Fason { Id = 1, Cantidad = 10 } });

            var resultado = target.Diferencial();
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<FinDelDia, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<FijacionDePrecioContrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Fason, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);


            Assert.NotNull(resultado);
            Assert.IsTrue(resultado.HayErrores);
            Assert.AreEqual(2, resultado.ListaErrores[0].ErrorCode);
        }
    }
}
