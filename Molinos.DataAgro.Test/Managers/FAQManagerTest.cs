using NLog;
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
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Test.Managers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class FAQManagerTest
    {
        private FAQManager target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ILogger> logger;
        private Mock<IComercialManager> comercialManagerMock;
        private Mock<IMailManager> mailManagerMock;
        private Mock<IHttpContextManager> contextoManager;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            logger = new Mock<ILogger>();
            comercialManagerMock = new Mock<IComercialManager>();
            mailManagerMock = new Mock<IMailManager>();
            contextoManager = new Mock<IHttpContextManager>();

            ConfigurationManager.AppSettings["ValorPruebaSap"] = "1";
            target = new FAQManager(logger.Object, repositorioMock.Object, comercialManagerMock.Object, mailManagerMock.Object, contextoManager.Object);
            repositorioMock.Setup(x => x.Obtener<Configuracion>(1)).Returns(new Configuracion { ConexionABMStop = true });
        }

        [Test]
        public void TraerManualesTest()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Manuales, ManualesDto>>>(), It.IsAny<Expression<Func<Manuales, bool>>>(), 0, null, DirOrden.Asc))
                .Returns(new List<ManualesDto>() { 
                    new ManualesDto {
                        Id = 1,
                        Titulo = "",
                        Descripcion = "",
                        Path = "",
                        FechaUltimaActualizacion = DateTime.Now,
                        Version = 0,
                        CantidadVisitas = 0,
                    }
                });

            var result = target.TraerManuales();
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Manuales, ManualesDto>>>(), It.IsAny<Expression<Func<Manuales, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc), Times.Once);
            Assert.IsNotNull(result);
        }

        [Test]
        public void EnviarSugerenciaTest()
        {
            mailManagerMock.Setup(x => x.GetEmailUserActiveDirectory(It.IsAny<string>()));
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>())).Returns(new Comercial { ComercialId = 1, Apellido = "a", Nombres = "b" });
            //contextoManager.Setup(x => x.ObtenerPathLogoMail()).Returns(It.IsAny<string>());
            contextoManager.Setup(x => x.ObtenerPathLogoMail()).Returns(TestContext.CurrentContext.TestDirectory + "\\Util\\MolinosAgro.png");
            mailManagerMock.Setup(y => y.EnviarMail(It.IsAny<Comercial>(), It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>(),
                 It.IsAny<List<string>>(), It.IsAny<AlternateView>(), It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<List<string>>())).Verifiable();

            var result = target.EnviarSugerencia(new ManualesDto { 
                Id = 1, 
                Titulo = "a", 
                Descripcion = "b", 
                Path = "c", 
                FechaUltimaActualizacion = DateTime.Now, 
                CantidadVisitas = 0, 
                Version = 0 }, "sugerencia", "GSIAN");

            Assert.NotNull(result);
            Assert.IsFalse(result.HayError);
        }

        [Test]
        public void RegistrarVisitaTest()
        {
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<Manuales, bool>>>())).Returns(new Manuales {
                Id = 1,
                Titulo = "a",
                Descripcion = "b",
                Path = "c",
                FechaUltimaActualizacion = DateTime.Now,
                CantidadVisitas = 0,
                Version = 0
            });

            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>())).Returns(new Comercial { ComercialId = 1, Apellido = "a", Nombres = "b" });

            ManualResult result = target.RegistrarVisita(1,"GSIAN");

            Assert.NotNull(result);
            Assert.IsFalse(result.HayError);
        }

        [Test]
        public void ActualizarFechaUltimaActualizacionManualesFAQTest()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Manuales, Manuales>>>(), It.IsAny<Expression<Func<Manuales, bool>>>(), 0, null, DirOrden.Asc))
                .Returns(new List<Manuales>() {
                    new Manuales {
                        Id = 1,
                        Titulo = "",
                        Descripcion = "",
                        Path = "",
                        FechaUltimaActualizacion = DateTime.Now,
                        Version = 0,
                        CantidadVisitas = 0,
                    }
                });

            target.ActualizarFechaUltimaActualizacionManualesFAQ();
        }
    }
}
