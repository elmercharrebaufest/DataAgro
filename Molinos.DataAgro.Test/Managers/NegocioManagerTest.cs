using Autofac.Extras.NLog;
using Molinos.DataAgro.Business.Managers;
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
using System.Linq.Expressions;
using System.Net.Mail;
using System.Web;

namespace Molinos.DataAgro.Test.Managers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class NegocioManagerTest
    {
        private NegocioManager target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ILogger> logger;
        private Mock<IMailManager> mailManagerMock;
        private Mock<IClientePrimaryAPIAgent> clientePrimaryAPIAgentMock;
        private Mock<IHttpContextManager> contextoMock;


        [SetUp]
        public void SetUp()
        {
            logger = new Mock<ILogger>();
            repositorioMock = new Mock<IRepositorio>();
            mailManagerMock = new Mock<IMailManager>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            clientePrimaryAPIAgentMock = new Mock<IClientePrimaryAPIAgent>();
            contextoMock = new Mock<IHttpContextManager>();

            target = new NegocioManager(logger.Object, repositorioMock.Object, mailManagerMock.Object, clientePrimaryAPIAgentMock.Object, contextoMock.Object);
        }

        [Test]
        public void OcultarEnTableroOk()
        {
            var negocio = new Negocio()
            {
                Id = 1,
                OcultarEnTablero = true
            };

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Negocio, bool>>>())).Returns(negocio);

            var resultado = target.OcultarEnTablero(negocio);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }

        [Test]
        public void EnvioMailNegociosAnulaYReemplazaTestOk()
        {
            var negocio = new Contrato()
            {
                Id = 1,
                AnulaYReemplazaContratoId = 1,
                AnulaYReemplazaContrato = new Contrato { ContratoSAP = "1" },
                MotivoReemplazo = "a",
                TipoNegocio = new TipoNegocio { Descripcion = "a" },
                ProveedorId = 1,
                Proveedor = new Proveedor { RazonSocial = "a" },
                ContratoSAP = "12",
                Fecha = DateTime.Now,
                FechaOperacion = DateTime.Now.AddDays(-10),
                TipoNegocioId = 1,
                Cantidad = 11111,
                Comercial = new Comercial { Nombres = "", Apellido = "" },
                Material = new Material { Descripcion = "" }
            };

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>())).Returns(new List<Contrato> { negocio });
            mailManagerMock.Setup(x => x.GetEmailUserActiveDirectory(It.IsAny<string>())).Returns("emartin@baufest.com");
            contextoMock.Setup(x => x.ObtenerPathLogoMail()).Returns(TestContext.CurrentContext.TestDirectory + "\\Util\\MolinosAgro.png");
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Comercial, string>>>(), It.IsAny<Expression<Func<Comercial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<string>() { "a" });

            target.EnvioMailNegociosAnulaYReemplaza();
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }

        [Test]
        public void EnvioMailNegociosAnulaYReemplazaTestOkSinContratos()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Contrato, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>())).Returns(new List<Contrato>());
            mailManagerMock.Setup(x => x.GetEmailUserActiveDirectory(It.IsAny<string>())).Returns("emartin@baufest.com");
            contextoMock.Setup(x => x.ObtenerPathLogoMail()).Returns(TestContext.CurrentContext.TestDirectory + "\\Util\\MolinosAgro.png");
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Comercial, string>>>(), It.IsAny<Expression<Func<Comercial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<string>() { "a" });

            target.EnvioMailNegociosAnulaYReemplaza();
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }

        [Test]
        public void EnvioMailNegociosConDiaAnteriorTestOk()
        {

            var negocio = new Negocio()
            {
                Id = 1,
                TipoNegocio = new TipoNegocio { Descripcion = "a" },
                ProveedorId = 1,
                Proveedor = new Proveedor { RazonSocial = "a" },
                ContratoSAP = "12",
                Fecha = DateTime.Now,
                FechaOperacion = DateTime.Now.AddDays(-10),
                TipoNegocioId = 1,
                Cantidad = 11111,
                Comercial = new Comercial { Nombres = "", Apellido = "" },
                Material = new Material { Descripcion = "" },

            };
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>())).Returns(new Comercial { IdActiveDirectory = "a" });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Negocio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>())).Returns(new List<Negocio> { negocio });
            mailManagerMock.Setup(x => x.GetEmailUserActiveDirectory(It.IsAny<string>())).Returns("emartin@baufest.com");
            contextoMock.Setup(x => x.ObtenerPathLogoMail()).Returns(TestContext.CurrentContext.TestDirectory + "\\Util\\MolinosAgro.png");
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Comercial, string>>>(), It.IsAny<Expression<Func<Comercial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<string>() { "a" });

            target.EnvioMailNegociosConDiaAnterior();
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }

        [Test]
        public void EnviarMailErrorFinalizarNegocioTestOk()
        {

            var negocio = new Negocio()
            {
                Id = 1,
                TipoNegocio = new TipoNegocio { Descripcion = "a" },
                ProveedorId = 1,
                Proveedor = new Proveedor { RazonSocial = "a" },
                ContratoSAP = "12",
                Fecha = DateTime.Now,
                FechaOperacion = DateTime.Now.AddDays(-10),
                TipoNegocioId = 1,
                Cantidad = 11111,
                Comercial = new Comercial { Nombres = "", Apellido = "" },
                Material = new Material { Descripcion = "" },
                Estado = new EstadoContrato { Descripcion = "a", EstadoContratoId = 1 },

            };

            repositorioMock.Setup(y => y.Obtener<Negocio>(It.IsAny<int>())).Returns(negocio);
            mailManagerMock.Setup(x => x.GetEmailUserActiveDirectory(It.IsAny<string>())).Returns("emartin@baufest.com");
            contextoMock.Setup(x => x.ObtenerPathLogoMail()).Returns(TestContext.CurrentContext.TestDirectory + "\\Util\\MolinosAgro.png");
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Comercial, string>>>(), It.IsAny<Expression<Func<Comercial, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
                .Returns(new List<string>() { "a" });
            ConfigurationManager.AppSettings["EmailDASoporte"] = "dataagro.baufest@gmail.com";

            target.EnviarMailErrorFinalizarNegocio(1);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
            mailManagerMock.Verify(x => x.EnviarMail(It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<AlternateView>(), null, null, null), Times.Once);
        }

        [Test]
        public void MigrarContratosPrimaryOk()
        {
            var negociosMAT = new List<AgenteCompra> { new AgenteCompra() { Id = 1, MaterialId = 3, OperadorId = 1, Posicion = "07.2023", Cantidad = 500, Precio = 1000, MonedaId = "USDM ", Operador = new Operador() { Id = 1 }, DolarExportador = true } };
            var negociosDA = new AgenteCompra() { Id = 1, MaterialId = 3, OperadorId = 1, Posicion = "07.2023", Cantidad = 500, Precio = 1000, MonedaId = "USDM ", Operador = new Operador() { Id = 1 }, DolarExportador = true };

            clientePrimaryAPIAgentMock.Setup(mock => mock.ObtenerNegocios(It.IsAny<DateTime>())).Returns(negociosMAT);
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AgenteCompra, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>())).Returns(new List<AgenteCompra> { negociosDA });
            contextoMock.Setup(x => x.ObtenerPathLogoMail()).Returns(TestContext.CurrentContext.TestDirectory + "\\Util\\MolinosAgro.png");
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>())).Returns(new List<Comercial> { new Comercial() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Negocio, int>>>(), It.IsAny<Expression<Func<Negocio, bool>>>(), 0, null, Entities.Helpers.DirOrden.Asc))
               .Returns(new List<int>() { 1 });

            target.MigrarContratosPrimary(DateTime.Now);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<AgenteCompra>()), Times.AtLeastOnce);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            mailManagerMock.Verify(x => x.EnviarMail(It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<AlternateView>(), null, null, null), Times.Once);
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
    }
}