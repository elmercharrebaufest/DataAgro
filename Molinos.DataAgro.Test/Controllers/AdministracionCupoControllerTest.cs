using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebDataAgro.Controllers;

namespace Molinos.DataAgro.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class AdministracionCupoControllerTest
    {
        private AdministracionCupoController target;
        private Mock<ICupoManager> cupoManagerMock;
        private Mock<ICentroManager> centroManagerMock;
        private Mock<IAdministracionCupoManager> administracionManagerMock;

        [SetUp]
        public void SetUp()
        {
            cupoManagerMock = new Mock<ICupoManager>();
            centroManagerMock = new Mock<ICentroManager>();
            administracionManagerMock = new Mock<IAdministracionCupoManager>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            target = new AdministracionCupoController(administracionManagerMock.Object, cupoManagerMock.Object, centroManagerMock.Object);

            HttpContext.Current.Session["perfil"] = 1;
            HttpContext.Current.Session["comercialId"] = 1;
        }

        // ------------------------------------------------------------------ //
        //  Index
        // ------------------------------------------------------------------ //

        [Test]
        public void IndexOk()
        {
            cupoManagerMock.Setup(x => x.Panel()).Returns(new List<DiaCupo>());
            cupoManagerMock.Setup(x => x.FechasComprendidas(null))
                .Returns(new List<DateTime> { new DateTime(2018, 10, 26), new DateTime(2018, 10, 27), new DateTime(2018, 10, 28) });
            cupoManagerMock.Setup(x => x.SugerenciasNoAceptadas()).Returns(new List<SugerenciaNoAceptada>());
            centroManagerMock.Setup(x => x.TraerTodoCentro())
                .Returns(new ResultIniCentro { Centro = new List<CentroIni> { new CentroIni { Descripcion = "San Lorenzo", CargaCupos = true } } });

            var result = target.Index() as ViewResult;

            Assert.NotNull(result);
            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }

        // ------------------------------------------------------------------ //
        //  DatosAdministracion
        // ------------------------------------------------------------------ //

        [Test]
        public void DatosAdministracion_RetornaDatosDelManager()
        {
            var dto = new AdministracionCupoDto { Id = 1, CentroId = 1, MaterialId = 1, ComercialId = 1, ProveedorId = 1 };
            administracionManagerMock.Setup(x => x.TraerTodaAdministracionCupo(It.IsAny<KendoGridMvcRequest>(), null))
                .Returns(new KendoGrid<AdministracionCupoDto>(new List<AdministracionCupoDto> { dto }, 1));

            var result = target.DatosAdministracion(new KendoGridMvcRequest()) as JsonResult;

            Assert.NotNull(result);
            var grid = result.Data as KendoGrid<AdministracionCupoDto>;
            Assert.NotNull(grid);
            Assert.AreEqual(1, grid.Total);
            Assert.AreEqual(1, grid.Data.Count());
            Assert.AreEqual(1, grid.Data.First().Id);
            administracionManagerMock.Verify(x => x.TraerTodaAdministracionCupo(It.IsAny<KendoGridMvcRequest>(), null), Times.Once);
        }

        // ------------------------------------------------------------------ //
        //  PartialPanel
        // ------------------------------------------------------------------ //

        [Test]
        public void PartialPanelTest()
        {
            cupoManagerMock.Setup(x => x.Panel()).Returns(new List<DiaCupo>());
            cupoManagerMock.Setup(x => x.FechasComprendidas(It.IsAny<int?>())).Returns(new List<DateTime>());
            cupoManagerMock.Setup(x => x.SugerenciasNoAceptadas()).Returns(new List<SugerenciaNoAceptada>());

            var result = target.PartialPanel() as PartialViewResult;

            Assert.NotNull(result);
            Assert.AreEqual("PartialPanel", result.ViewName);
            cupoManagerMock.Verify(x => x.Panel(), Times.Once);
            cupoManagerMock.Verify(x => x.FechasComprendidas(It.IsAny<int?>()), Times.Once);
            cupoManagerMock.Verify(x => x.SugerenciasNoAceptadas(), Times.Once);
        }

        // ------------------------------------------------------------------ //
        //  Aceptar
        // ------------------------------------------------------------------ //

        [Test]
        public void AceptarTest_RetornaCupoResultSinErrores()
        {
            administracionManagerMock.Setup(x => x.AceptarCupoExcedente(
                    It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
                    It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new CupoResult { Errores = new List<ErrorMessage>() });

            var result = target.Aceptar(1, 1, 1, 1, 1, "") as JsonResult;

            Assert.NotNull(result);
            var data = result.Data as CupoResult;
            Assert.NotNull(data);
            Assert.IsFalse(data.HayError);
            administracionManagerMock.Verify(x => x.AceptarCupoExcedente(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void AceptarTest_IdInvalido_RetornaErrorSinLlamarManager()
        {
            var result = target.Aceptar(0, 1, 1, 1, 1, "") as JsonResult;

            Assert.NotNull(result);
            var data = result.Data as CupoResult;
            Assert.NotNull(data);
            Assert.IsTrue(data.HayError);
            administracionManagerMock.Verify(x => x.AceptarCupoExcedente(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        // ------------------------------------------------------------------ //
        //  Rechazar
        // ------------------------------------------------------------------ //

        [Test]
        public void RechazarTest_RetornaResultadoSinErrores()
        {
            administracionManagerMock.Setup(x => x.CambiarEstadoRechazado(
                    It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new CupoResult { Errores = new List<ErrorMessage>() });

            var result = target.Rechazar(1, "") as JsonResult;

            Assert.NotNull(result);
            var data = result.Data as CupoResult;
            Assert.NotNull(data);
            Assert.IsFalse(data.HayError);
            administracionManagerMock.Verify(x => x.CambiarEstadoRechazado(
                It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        // ------------------------------------------------------------------ //
        //  AceptarMasivo
        // ------------------------------------------------------------------ //

        [Test]
        public void AceptarMasivoTest_RetornaCupoResultSinErrores()
        {
            var adm = new List<AdministracionCupoDto>
            {
                new AdministracionCupoDto { Id = 234, ComercialId = 63, CentroId = 1, MaterialId = 1, ZonaId = 1,
                    CantidadDeCupo = 10, CantidadFleteProcedencia = 1, EstadoId = 1, TipoNegocioId = 1,
                    ConfiguracionEspacioDinamicoId = 1, Fecha = DateTime.Now }
            };
            administracionManagerMock.Setup(x => x.AceptarCupoExcedenteMasivo(
                    It.IsAny<List<AdministracionCupoDto>>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new CupoResult { Errores = new List<ErrorMessage>() });

            var result = target.AceptarMasivo(adm, "") as JsonResult;

            Assert.NotNull(result);
            var data = result.Data as CupoResult;
            Assert.NotNull(data);
            Assert.IsFalse(data.HayError);
            administracionManagerMock.Verify(x => x.AceptarCupoExcedenteMasivo(
                It.IsAny<List<AdministracionCupoDto>>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void AceptarMasivoTest_ListaVacia_RetornaErrorSinLlamarManager()
        {
            var result = target.AceptarMasivo(new List<AdministracionCupoDto>(), "") as JsonResult;

            Assert.NotNull(result);
            var data = result.Data as CupoResult;
            Assert.NotNull(data);
            Assert.IsTrue(data.HayError);
            administracionManagerMock.Verify(x => x.AceptarCupoExcedenteMasivo(
                It.IsAny<List<AdministracionCupoDto>>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        // ------------------------------------------------------------------ //
        //  RechazarMasivo
        // ------------------------------------------------------------------ //

        [Test]
        public void RechazarMasivoTest_RetornaListaDeResultados()
        {
            var adm = new List<AdministracionCupoDto>
            {
                new AdministracionCupoDto { Id = 234, ComercialId = 63, CentroId = 1, MaterialId = 1, ZonaId = 1,
                    CantidadDeCupo = 10, CantidadFleteProcedencia = 1, EstadoId = 1, TipoNegocioId = 1,
                    ConfiguracionEspacioDinamicoId = 1, Fecha = DateTime.Now }
            };
            administracionManagerMock.Setup(x => x.CambiarEstadoRechazado(
                    It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new CupoResult { Errores = new List<ErrorMessage>() });

            var result = target.RechazarMasivo(adm, "") as JsonResult;

            Assert.NotNull(result);
            var data = result.Data as List<Resultado>;
            Assert.NotNull(data);
            Assert.AreEqual(1, data.Count);
            Assert.IsFalse(data[0].HayError);
            administracionManagerMock.Verify(x => x.CambiarEstadoRechazado(
                It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void RechazarMasivoTest_ListaVacia_RetornaErrorSinLlamarManager()
        {
            var result = target.RechazarMasivo(new List<AdministracionCupoDto>(), "") as JsonResult;

            Assert.NotNull(result);
            var data = result.Data as Resultado;
            Assert.NotNull(data);
            Assert.IsTrue(data.HayError);
            administracionManagerMock.Verify(x => x.CambiarEstadoRechazado(
                It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        // ------------------------------------------------------------------ //
        //  BuscarDatosSolicitudCupo
        // ------------------------------------------------------------------ //

        [Test]
        public void BuscarDatosSolicitudCupoTest_RetornaGridConDatos()
        {
            var dto = new AdministracionCupoDto { Id = 1, CentroId = 1, MaterialId = 1, ComercialId = 1, ProveedorId = 1 };
            administracionManagerMock.Setup(x => x.TraerTodaAdministracionCupo(It.IsAny<KendoGridMvcRequest>(), It.IsAny<int?>()))
                .Returns(new KendoGrid<AdministracionCupoDto>(new List<AdministracionCupoDto> { dto }, 1));

            var result = target.BuscarDatosSolicitudCupo(new KendoGridMvcRequest(), null) as JsonResult;

            Assert.NotNull(result);
            var grid = result.Data as KendoGrid<AdministracionCupoDto>;
            Assert.NotNull(grid);
            Assert.AreEqual(1, grid.Total);
            Assert.AreEqual(1, grid.Data.First().Id);
            administracionManagerMock.Verify(x => x.TraerTodaAdministracionCupo(
                It.IsAny<KendoGridMvcRequest>(), It.IsAny<int?>()), Times.Once);
        }

        // ------------------------------------------------------------------ //
        //  ActualizarSolicitud
        // ------------------------------------------------------------------ //

        [Test]
        public void ActualizarSolicitudTest_RetornaOk()
        {
            administracionManagerMock.Setup(x => x.ActualizarSolicitud(
                    It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Returns("Ok");

            var result = target.ActualizarSolicitud(1, 10, 5, true) as JsonResult;

            Assert.NotNull(result);
            Assert.AreEqual("Ok", result.Data);
            administracionManagerMock.Verify(x => x.ActualizarSolicitud(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()), Times.Once);
        }
    }
}
