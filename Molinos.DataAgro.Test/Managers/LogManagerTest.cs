using NLog;
using Molinos.DataAgro.Business.Managers;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Repository;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Molinos.DataAgro.Test.Managers
{
    [TestFixture]
    public class LogManagerTest
    {
        private Mock<IRepositorio> repositorioMock;
        private Mock<ILogger> loggerMock;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            loggerMock = new Mock<ILogger>();
        }

        [Test]
        public void TraerTodoLogOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Log, LogDto>>>(), It.IsAny<Expression<Func<Log, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                .Returns(new List<LogDto>() { new LogDto { Fecha = DateTime.Now, Id = 1, Xml = "a" } });

            var target = new LogManager(repositorioMock.Object, loggerMock.Object);
            var resultado = target.TraerTodoLog(DateTime.Now);

            repositorioMock.Verify(y => y.Listar(It.IsAny<Expression<Func<Log, LogDto>>>(), It.IsAny<Expression<Func<Log, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()));

            Assert.NotNull(resultado);
            Assert.AreEqual(1, resultado.Count);
        }

        [Test]
        public void EliminarLogsAntiguosOk_SinArchivos_LogueaQueNoSeEliminoNada()
        {
            var target = new LogManagerTestable(repositorioMock.Object, loggerMock.Object, new List<Archivo>());

            target.EliminarLogsAntiguos();

            loggerMock.Verify(x => x.Info("No se elimino ningun archivo."), Times.Once);
        }

        [Test]
        public void EliminarLogsAntiguosOk_ConArchivos_LogueaDetalle()
        {
            var archivosEliminados = new List<Archivo>
            {
                new Archivo
                {
                    Nombre = @"L:\logs\archivo1.txt",
                    PesoKB = 1024,
                    FechaCreacion = DateTime.Now.AddMonths(-7),
                    FechaUltimaModificacion = DateTime.Now.AddMonths(-6)
                },
                new Archivo
                {
                    Nombre = @"L:\logs\archivo2.log",
                    PesoKB = 2048,
                    FechaCreacion = DateTime.Now.AddMonths(-8),
                    FechaUltimaModificacion = DateTime.Now.AddMonths(-7)
                }
            };

            var target = new LogManagerTestable(repositorioMock.Object, loggerMock.Object, archivosEliminados);

            target.EliminarLogsAntiguos();

            loggerMock.Verify(x => x.Info("ARCHIVOS ELIMINADOS:"), Times.Once);

            foreach (var archivo in archivosEliminados)
            {
                loggerMock.Verify(x => x.Info(It.Is<string>(msg =>
                    msg.Contains(archivo.Nombre) &&
                    msg.Contains(archivo.PesoKB.ToString()) &&
                    msg.Contains(archivo.FechaCreacion.ToString("yyyy")) // año como referencia
                )), Times.Once);
            }
        }

        // Clase que permite testear sin tocar el filesystem
        public class LogManagerTestable : LogManager
        {
            private readonly List<Archivo> archivosMock;

            public LogManagerTestable(IRepositorio repo, ILogger logger, List<Archivo> archivosMock)
                : base(repo, logger)
            {
                this.archivosMock = archivosMock;
            }

            protected override List<Archivo> BorrarArchivosViejos(string path, TimeSpan antiguedad)
            {
                return archivosMock;
            }
        }
    }
}
