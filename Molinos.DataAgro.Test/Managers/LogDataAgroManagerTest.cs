using Autofac.Extras.NLog;
using Kendo.DynamicLinq;
using Molinos.DataAgro.Business.Managers;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
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

namespace Molinos.DataAgro.Test.Managers
{
    [TestFixture]
    public class LogDataAgroManagerTest
    {
        private LogDataAgroManager target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ILogger> loggerMock;
        IList<LogDataAgro> listaDeLogs;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            loggerMock = new Mock<ILogger>();

            target = new LogDataAgroManager(repositorioMock.Object, loggerMock.Object);
            repositorioMock.Setup(x => x.Obtener<Configuracion>(1)).Returns(new Configuracion { ConexionABMStop = true });

            repositorioMock.Setup(x => x.Agregar(It.IsAny<LogDataAgro>()));
            repositorioMock.Setup(x => x.GuardarCambios()).Returns(1);

            listaDeLogs = new List<LogDataAgro> {
                    new LogDataAgro { Id = 1, DatoModificado = "{'cambio':'cambio'}",ClaseId=1,Tipo="",AccionRealizada="Crear" },
                    new LogDataAgro { Id = 2, DatoModificado = "{'cambio':'Otro'}" ,ClaseId=1,Tipo= "", AccionRealizada = "Modificar"}
                };
        }


        [Test]
        public void LogCambiosDataAgroNegocioOk()
        {
            var respuesta = target.LogCambiosDataAgro(new BasicoContrato { Id = 1 }, TipoAccionLogDataAgro.Modificar, typeof(Fason));

            Assert.AreEqual(respuesta, 1);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once());
            repositorioMock.Verify(x => x.Agregar(It.IsAny<LogDataAgro>()), Times.Once());

        }

        [Test]
        public void LogCambiosDataAgroCupoOk()
        {
            var respuesta = target.LogCambiosDataAgro(new CupoDto { Id = 1 }, TipoAccionLogDataAgro.Modificar);

            Assert.AreEqual(respuesta, 1);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once());
            repositorioMock.Verify(x => x.Agregar(It.IsAny<LogDataAgro>()), Times.Once());
        }

        [Test]
        public void LogCambiosDataAgroProveedorOk()
        {
            var respuesta = target.LogCambiosDataAgro(new StoredPorProveedorResult(), TipoAccionLogDataAgro.Modificar, 1);

            Assert.AreEqual(respuesta, 1);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once());
            repositorioMock.Verify(x => x.Agregar(It.IsAny<LogDataAgro>()), Times.Once());
        }

        [Test]
        public void ListarDatosLogDataAgroOk()
        {
            repositorioMock.Setup(x => x.ObtenerConsultaEscalar(It.IsAny<TraerTodosLogsDataAgro>()))
                .Returns(new DataSourceResult { });

            var respuesta = target.ListarDatosLogDataAgro(new DataSourceRequest(), new List<int> { 1, 2, 3 });

            Assert.NotNull(respuesta);
            Assert.AreEqual(respuesta.GetType().Name, typeof(DataSourceResult).Name);
            repositorioMock.Verify(x => x.ObtenerConsultaEscalar(It.IsAny<TraerTodosLogsDataAgro>()), Times.Once());
        }

        [Test]
        public void TraerDatosModificadosPorIdOkCrear()
        {
            repositorioMock.Setup(x => x.Listar<LogDataAgro>(It.IsAny<Expression<Func<LogDataAgro, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(listaDeLogs.ToList());

            var respuesta = target.TraerDatosModificadosPorId(1);

            Assert.NotNull(respuesta);
            Assert.AreEqual(1, respuesta.CamposCambiados.Count);
            Assert.AreEqual(respuesta.LogActual.Id, 1);
            repositorioMock.Verify(x => x.Listar<LogDataAgro>(It.IsAny<Expression<Func<LogDataAgro, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Exactly(2));
        }

        [Test]
        public void TraerDatosModificadosPorIdOkModificar()
        {
            repositorioMock.Setup(x => x.Listar<LogDataAgro>(It.IsAny<Expression<Func<LogDataAgro, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns((Expression<Func<LogDataAgro, bool>> condicion, int x, string y, DirOrden z) => listaDeLogs.Where(condicion.Compile()).ToList());

            var respuesta = target.TraerDatosModificadosPorId(2);

            Assert.NotNull(respuesta);
            Assert.AreEqual(1, respuesta.CamposCambiados.Count);
            Assert.AreNotEqual(respuesta.LogActual.Id, respuesta.LogAnterior.Id);
            repositorioMock.Verify(x => x.Listar<LogDataAgro>(It.IsAny<Expression<Func<LogDataAgro, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Exactly(2));
        }

        [Test]
        public void LogCambiosDataAgroRangoConfirmacionAutomaticaOk()
        {
            var respuesta = target.LogCambiosDataAgro(new RangoConfirmacionAutomaticaDto { Id = 1 }, TipoAccionLogDataAgro.Modificar);

            Assert.AreEqual(respuesta, 1);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once());
            repositorioMock.Verify(x => x.Agregar(It.IsAny<LogDataAgro>()), Times.Once());
        }

        [Test]
        public void LogCambiosDataAgroPrecioMoaOk()
        {
            var respuesta = target.LogCambiosDataAgro(new PrecioMoaDto { Id = 1 }, TipoAccionLogDataAgro.Modificar);

            Assert.AreEqual(respuesta, 1);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once());
            repositorioMock.Verify(x => x.Agregar(It.IsAny<LogDataAgro>()), Times.Once());
        }
        [Test]
        public void LogCambiosDataAgroHabilitacionFijacionOk()
        {
            var respuesta = target.LogCambiosDataAgro(new HabilitacionFijacionDto { Id = 1 }, TipoAccionLogDataAgro.Modificar);

            Assert.AreEqual(respuesta, 1);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once());
            repositorioMock.Verify(x => x.Agregar(It.IsAny<LogDataAgro>()), Times.Once());
        }
        [Test]
        public void LogCambiosDataAgroHabilitacionPizarraOk()
        {
            var respuesta = target.LogCambiosDataAgro(new HabilitacionPizarraDto { Id = 1 }, TipoAccionLogDataAgro.Modificar);

            Assert.AreEqual(respuesta, 1);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once());
            repositorioMock.Verify(x => x.Agregar(It.IsAny<LogDataAgro>()), Times.Once());
        }

        [Test]
        public void ObtenerCuposIdOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Cupo, int>>>(), It.IsAny<Expression<Func<Cupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<int>() { 1 });
            var result = target.ObtenerCuposId(new List<string> { "asd" });

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Cupo, int>>>(), It.IsAny<Expression<Func<Cupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
        }
        [Test]
        public void ObtenerNegociosIdOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Negocio, int>>>(), It.IsAny<Expression<Func<Negocio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<int>() { 1 });
            var result = target.ObtenerNegociosId(new List<string> { "asd" });

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Negocio, int>>>(), It.IsAny<Expression<Func<Negocio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
        }
    }
}
