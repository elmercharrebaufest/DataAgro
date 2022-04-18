using Autofac.Extras.NLog;
using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Business;
using Molinos.DataAgro.Business.Managers;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Interfaces.Criterios;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Reflection;
using System.Web;
using System.Web.Script.Serialization;


namespace Molinos.DataAgro.Test.Managers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class AdministracionCupoManagerTest
    {
        private AdministracionCupoManager target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ILogger> logger;
        private Mock<IMailManager> mailManagerMock;
        private Mock<IComercialManager> comercialManagerMock;
        private Mock<ICupoManager> cupoManagerMock;
        private Mock<IHttpContextManager> contextoMock;

        [SetUp]
        public void SetUp()
        {
            logger = new Mock<ILogger>();
            repositorioMock = new Mock<IRepositorio>();
            mailManagerMock = new Mock<IMailManager>();
            comercialManagerMock = new Mock<IComercialManager>();
            cupoManagerMock = new Mock<ICupoManager>();
            contextoMock = new Mock<IHttpContextManager>();

            target = new AdministracionCupoManager(logger.Object, repositorioMock.Object, cupoManagerMock.Object,
                mailManagerMock.Object, comercialManagerMock.Object, contextoMock.Object);
        }

        [Test]
        public void TraerTodaAdministracionCupoTest()
        {
            repositorioMock.Setup(x => x.ObtenerConsultaEscalar(It.IsAny<TraerAdministracionCupoExcedente>()))
             .Returns(new KendoGrid<AdministracionCupoDto>(new List<AdministracionCupoDto> { new AdministracionCupoDto { Id = 1, CentroId = 1, MaterialId = 1, ComercialId = 1, ProveedorId = 1 } }, 1));
            var resultado = target.TraerTodaAdministracionCupo(It.IsAny<KendoGridMvcRequest>(), null);
            Assert.IsNotNull(resultado);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }

        [Test]
        public void TraerAdministracionCupo()
        {
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<AdministracionCupo, bool>>>(), It.IsAny<Expression<Func<AdministracionCupo, AdministracionCupoDto>>>())).
                Returns(new AdministracionCupoDto());
            var resultado = target.TraerAdministracionCupo(It.IsAny<int>());
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }


        [Test]
        public void AceptarCupoExcedenteSinError()
        {
            repositorioMock.Setup(y => y.Obtener<AdministracionCupo>(It.IsAny<int>()))
                .Returns(new AdministracionCupo
                {
                    Id = 1,
                    Fecha = DateTime.Now,
                    CantidadCupo = 1,
                    CantidadFleteProcedencia = 0,
                    CentroId = 1,
                    ComercialId = 1,
                    Comercial = new Comercial { IdActiveDirectory = "bmelgarejo", ComercialId = 1 },
                    EstadoId = 3,
                    Excedente = true,
                    ProveedorId = 1,
                    ZonaId = 1,
                    Proveedor = new Proveedor { RazonSocial = "hernanbio" },
                    Material = new Material { Descripcion = "Trigo" },
                    Centro = new Centro { Acopio = false, CodigoSap = "1", Descripcion = "", Id = 1 }
                });
            cupoManagerMock.Setup(x => x.SugerenciasParaAceptar(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), null))
                .Returns(new List<SugerenciaCupo>() {
                    new SugerenciaCupo {
                        FechaSugerida = DateTime.Now,
                        MaterialId = 1,
                        Aceptado = false,
                        CantidadCupoOriginal = 10,
                        CantidadDeCupos = 10,
                        CDWarrant = false,
                        CentroId = 1,
                        ComercialId = 63,
                        ConfiguracionEspacioDinamicoId = 1,
                        MonedaId = "USDM",
                        Id = 1,
                        ProveedorId = 1,
                        TipoNegocioId = 1,
                        ZonaCupoId = 1,
                        Puntuaciones = "{\"CriterioRaiz\":0.4}",
                    }
                });

            cupoManagerMock.Setup(x => x.GrabarCupo(It.IsAny<Cupo>(), It.IsAny<List<DiaCupo>>())).
               Returns(new CupoResult() { ListaCupos = new List<string>() { "MOL/123223", "MOL/232323" } });

            repositorioMock.Setup(y => y.Obtener<Comercial>(It.IsAny<int>()))
              .Returns(new Comercial { ComercialId = 1, IdActiveDirectory = "bmelgarejo" });

            repositorioMock.Setup(y => y.Obtener<Comercial>(It.IsAny<Expression<Func<Comercial, bool>>>()))
          .Returns(new Comercial { ComercialId = 1, IdActiveDirectory = "bmelgarejo" });

            contextoMock.Setup(x => x.ObtenerPathLogoMail()).Returns(TestContext.CurrentContext.TestDirectory + "\\Util\\MolinosAgro.png");
            mailManagerMock.Setup(y => y.EnviarMail(It.IsAny<Comercial>(), It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>(),
                 It.IsAny<List<string>>(), It.IsAny<AlternateView>(), It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<string>())).Verifiable();
            repositorioMock.Setup(x => x.AgregarTodos(It.IsAny<List<Cupo>>(), null)).Verifiable();
            cupoManagerMock.Setup(x => x.SugerenciasParaAceptar(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<DateTime>()))
                .Returns(new List<SugerenciaCupo> { new SugerenciaCupo { CantidadCupoOriginal = 1, CantidadDeCupos = 1, CDWarrant = true, Aceptado = null, CentroId = 1, ComercialId = 1, ConfiguracionEspacioDinamicoId = 1, ContratoSAP = "", Destinatario = "", FechaSugerida = DateTime.Now.Date, Id = 1, MaterialId = 1, MonedaId = "ARP", MotivoRechazo = "", NegocioId = 1, Precio = 1, ProveedorId = 1, Puntuaciones = "", StandardDeCalidad = "", TipoNegocioId = 1, ZonaCupoId = 1, Puntuacion = 1 } });
            var resultado = target.AceptarCupoExcedente(It.IsAny<int>(), 1, 0, 1, 0, "bmelgarejo", "prueba");
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }
        [Test]
        public void CambiarEstadoRechazadoTest()
        {
            repositorioMock.Setup(y => y.Obtener<AdministracionCupo>(It.IsAny<int>()))
                .Returns(new AdministracionCupo
                {
                    Id = 1,
                    Fecha = DateTime.Now,
                    CantidadCupo = 5,
                    CantidadFleteProcedencia = 2,
                    CentroId = 1,
                    Centro = new Centro { CodigoSap = "1", Id = 1, Descripcion = "" },
                    ComercialId = 1,
                    Comercial = new Comercial { IdActiveDirectory = "bmelgarejo", ComercialId = 1 },
                    EstadoId = 1,
                    Excedente = true,
                    ProveedorId = 1,
                    ZonaId = 1,
                    Proveedor = new Proveedor { RazonSocial = "hernanbio" },
                    Material = new Material { Descripcion = "Trigo" }
                });
            repositorioMock.Setup(y => y.Obtener<Comercial>(It.IsAny<int>()))
             .Returns(new Comercial { ComercialId = 1, IdActiveDirectory = "bmelgarejo" });

            repositorioMock.Setup(y => y.Obtener<Comercial>(It.IsAny<Expression<Func<Comercial, bool>>>()))
          .Returns(new Comercial { ComercialId = 1, IdActiveDirectory = "bmelgarejo" });

            contextoMock.Setup(x => x.ObtenerPathLogoMail()).Returns(TestContext.CurrentContext.TestDirectory + "\\Util\\MolinosAgro.png");
            mailManagerMock.Setup(y => y.EnviarMail(It.IsAny<Comercial>(), It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>(),
                 It.IsAny<List<string>>(), It.IsAny<AlternateView>(), It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<string>())).Verifiable();
            cupoManagerMock.Setup(x => x.SugerenciasParaAceptar(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<DateTime?>()))
                            .Returns(new List<SugerenciaCupo> { new SugerenciaCupo { CantidadCupoOriginal = 1, CantidadDeCupos = 1, CDWarrant = true, Aceptado = null, CentroId = 1, ComercialId = 1, ConfiguracionEspacioDinamicoId = 1, ContratoSAP = "", Destinatario = "", FechaSugerida = DateTime.Now.Date, Id = 1, MaterialId = 1, MonedaId = "ARP", MotivoRechazo = "", NegocioId = 1, Precio = 1, ProveedorId = 1, Puntuaciones = "", StandardDeCalidad = "", TipoNegocioId = 1, ZonaCupoId = 1, Puntuacion = 1 } });
            var resultado = target.CambiarEstadoRechazado(It.IsAny<int>(), "", "bmelgarejo");
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }
        [Test]
        public void RechazarSolicitudesVencidasTest()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AdministracionCupo, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
             .Returns(new List<AdministracionCupo>());
            target.RechazarSolicitudesVencidas();
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }

    }
}