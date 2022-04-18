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
using System.Net.Mail;
using System.Web;
using System.Web.Script.Serialization;

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
        private Mock<IClientePrimariAPIAgent> clientePrimariAPIAgentMock;
        private JavaScriptSerializer serializer;
        private Mock<IHttpContextManager> contextoMock;


        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            logger = new Mock<ILogger>();
            repositorioMock = new Mock<IRepositorio>();
            mailManagerMock = new Mock<IMailManager>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            clientePrimariAPIAgentMock = new Mock<IClientePrimariAPIAgent>();
            contextoMock = new Mock<IHttpContextManager>();

            target = new NegocioManager(logger.Object, repositorioMock.Object, mailManagerMock.Object, clientePrimariAPIAgentMock.Object, contextoMock.Object);
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
                Material = new Material { Descripcion = "" },

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
            mailManagerMock.Verify(x => x.EnviarMail(It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<AlternateView>(), null,null, null), Times.Once);

        }
    }

}
