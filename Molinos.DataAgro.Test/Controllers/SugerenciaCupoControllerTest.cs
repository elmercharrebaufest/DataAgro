using Autofac.Extras.NLog;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Interfaces.Managers;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebDataAgro.Controllers;

namespace Molinos.DataAgro.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    class SugerenciaCupoControllerTest
    {
        private SugerenciaCupoController target;
        private Mock<ILogger> loggerMock;
        private Mock<ICupoManager> cupoManagerMock;
        private Mock<IMaterialManager> materialManagerMock;
        private Mock<ICentroManager> centroManagerMock;
        private Mock<IFormulaManager> formulaManagerMock;
        private Mock<IComercialManager> comercialManagerMock;

        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            serializer = new JavaScriptSerializer();
            loggerMock = new Mock<ILogger>();
            cupoManagerMock = new Mock<ICupoManager>();
            materialManagerMock = new Mock<IMaterialManager>();
            centroManagerMock = new Mock<ICentroManager>();
            formulaManagerMock = new Mock<IFormulaManager>();
            comercialManagerMock = new Mock<IComercialManager>();

            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            target = new SugerenciaCupoController(loggerMock.Object, centroManagerMock.Object, cupoManagerMock.Object, materialManagerMock.Object,
                formulaManagerMock.Object, comercialManagerMock.Object);
            HttpContext.Current.Session["perfil"] = 1;
            HttpContext.Current.Session["comercialId"] = 1;

        }

        [Test]
        public void IndexOk()
        {
            cupoManagerMock.Setup(x => x.ObtenerSugerenciaCupoAgrupadasPorProveedor(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .Returns(new List<SugerenciaCupoDto> { new SugerenciaCupoDto { Id = 1, CentroId = 1, MaterialId = 1, ComercialId = 1, ProveedorId = 1 } });

            cupoManagerMock.Setup(x => x.FechasComprendidas(null))
                .Returns(new List<DateTime> { new DateTime(2018, 10, 26), new DateTime(2018, 10, 27), new DateTime(2018, 10, 28) });

            materialManagerMock.Setup(x => x.TraerTodoMaterial())
               .Returns(new ResultIniMaterial { Material = new List<MaterialIni>() });
            centroManagerMock.Setup(x => x.TraerTodoCentro())
              .Returns(new ResultIniCentro { Centro = new List<CentroIni>() });
            cupoManagerMock.Setup(x => x.DevolverTodoCierreCupera()).Returns(new List<CierreCupera>());
            comercialManagerMock.Setup(x => x.ListarComercialesAsignanNegocios()).Returns(new List<ComercialQry>());

            var result = target.Index() as ViewResult;
            Assert.NotNull(result);
            Assert.IsEmpty(result.ViewName);
        }

        [Test]
        public void DatosConfiguracionTest()
        {
            cupoManagerMock.Setup(x => x.ObtenerSugerenciaCupo(It.IsAny<int>(), It.IsAny<int?>()))
                .Returns(new List<SugerenciaCupoDto> { new SugerenciaCupoDto { Id = 1, CentroId = 1, MaterialId = 1, ComercialId = 1, ProveedorId = 1 } });
            var result = target.DatosConfiguracion(new KendoGridMvcRequest(), It.IsAny<int?>());

            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            cupoManagerMock.Verify(x => x.ObtenerSugerenciaCupo(It.IsAny<int>(), It.IsAny<int?>()), Times.Once);
            Assert.AreEqual(
               "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Groups\":null,\"Data\":[{\"ComercialId\":1,\"Id\":1,\"MaterialId\":1,\"MaterialDesc\":null,\"Precio\":null,\"MonedaId\":null,\"ConfiguracionEspacioDinamicoId\":null,\"FechaDesde\":\"\\/Date(-62135586000000)\\/\",\"FechaHasta\":\"\\/Date(-62135586000000)\\/\",\"PrecioPizarra\":0,\"formula\":null,\"Puntuaciones\":{},\"PuntuacionesString\":null,\"PuntuacionTotal\":0,\"DestinoId\":0,\"CantidadDeCupos\":0,\"CantidadCupoOriginal\":0,\"CantidadDeCuposMaximo\":0,\"CantidadFleteProcedencia\":null,\"ZonaDescrip\":null,\"Priorizado\":false,\"FechaSugerida\":\"\\/Date(-62135586000000)\\/\",\"ProveedorId\":1,\"CentroId\":1,\"MonedaDesc\":null,\"ProveedorCUIT\":null,\"ProveedorDesc\":null,\"TipoNegocioDesc\":null,\"Aceptado\":null,\"ZonaCupoId\":null,\"Destinatario\":null,\"StandardDeCalidad\":null,\"TipoNegocioId\":0,\"ContratoSAP\":null,\"NegocioId\":null,\"CDWarrant\":false,\"KgNegocio\":0,\"KgPendienteAplicar\":0,\"ComercialDesc\":null,\"Fason\":null,\"CentroDesc\":null,\"Inhabilitado\":null,\"CuposPendientes\":0,\"SolicitudesPendientes\":0,\"CUITProveedor\":null,\"CUITCorredor\":null,\"Canje\":null,\"MercsDeposito\":null,\"CaratulaMAT\":null,\"Sustentable\":false,\"EPA\":false}],\"Aggregates\":null,\"Total\":1},\"JsonRequestBehavior\":0,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
               a);
        }
        [Test]
        public void AceptarTest()
        {
            cupoManagerMock.Setup(x => x.AceptarSugerenciaCupo(It.IsAny<List<SugerenciaCupoDto>>())).Returns(new List<CupoResult>());
            List<SugerenciaCupoDto> list = new List<SugerenciaCupoDto>();
            list.Add(new SugerenciaCupoDto { Id = 1, CantidadDeCupos = 1 });
            var result = target.Aceptar(list);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            cupoManagerMock.Verify(x => x.AceptarSugerenciaCupo(It.IsAny<List<SugerenciaCupoDto>>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[],\"JsonRequestBehavior\":1,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void RechazarTest()
        {
            List<int> ids = new List<int> { 1 };
            var result = target.Rechazar(ids, "");
            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            cupoManagerMock.Verify(x => x.RechazarSugerenciaCupo(It.IsAny<List<int>>(), It.IsAny<String>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[null],\"JsonRequestBehavior\":1,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void DatosConfirmarTest()
        {
            cupoManagerMock.Setup(x => x.ConfirmarSugerencia(It.IsAny<List<ConfirmacionSugerenciaCupoDto>>(), It.IsAny<List<DiaCupo>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>())).Returns(new CupoResult());
            List<ConfirmacionSugerenciaCupoDto> lista = new List<ConfirmacionSugerenciaCupoDto>();
            var result = target.DatosConfirmar(lista, It.IsAny<List<DiaCupo>>(), 1, "", 1);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            cupoManagerMock.Verify(x => x.ConfirmarSugerencia(It.IsAny<List<ConfirmacionSugerenciaCupoDto>>(), It.IsAny<List<DiaCupo>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":\"Ok\",\"JsonRequestBehavior\":0,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void GenerarSolicitudExtraordinariaTest()
        {
            cupoManagerMock.Setup(x => x.GenerarSolicitudExtraordinaria(It.IsAny<AdministracionCupoDto>())).Returns(new CupoResult());
            var result = target.GenerarSolicitudExtraordinaria(It.IsAny<AdministracionCupoDto>());
            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            cupoManagerMock.Verify(x => x.GenerarSolicitudExtraordinaria(It.IsAny<AdministracionCupoDto>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"ListaCupos\":[],\"CuposNormales\":0,\"CuposFlete\":0,\"Estados\":null,\"Codigo\":null,\"CupoNoPropios\":null,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":0,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void PartialPanelTest()
        {
            cupoManagerMock.Setup(x => x.ObtenerSugerenciaCupoAgrupadasPorProveedor(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .Returns(new List<SugerenciaCupoDto> { new SugerenciaCupoDto { Id = 1, CentroId = 1, MaterialId = 1, ComercialId = 1, ProveedorId = 1 } });

            cupoManagerMock.Setup(x => x.FechasComprendidas(null))
                .Returns(new List<DateTime> { new DateTime(2018, 10, 26), new DateTime(2018, 10, 27), new DateTime(2018, 10, 28) });

            materialManagerMock.Setup(x => x.TraerTodoMaterial())
               .Returns(new ResultIniMaterial { Material = new List<MaterialIni>() });
            centroManagerMock.Setup(x => x.TraerTodoCentro())
              .Returns(new ResultIniCentro { Centro = new List<CentroIni>() });
            cupoManagerMock.Setup(x => x.DevolverTodoCierreCupera()).Returns(new List<CierreCupera>());
            comercialManagerMock.Setup(x => x.ListarComercialesAsignanNegocios()).Returns(new List<ComercialQry>());

            var result = target.PartialTabla(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int?>()) as PartialViewResult;

            Assert.NotNull(result);
        }

        [Test]
        public void DevolverSugerenciasMasivoTest()
        {
            cupoManagerMock.Setup(x => x.DevolverSugerenciasMasivo(It.IsAny<List<DevolucionSugerenciaCupoDto>>())).Returns(new CupoResult());
            List<DevolucionSugerenciaCupoDto> sugerenciasADevolver = new List<DevolucionSugerenciaCupoDto> { new DevolucionSugerenciaCupoDto { IdSugerencia = 1111, Cantidad = 2 } };
            var result = target.DevolverSugerenciasMasivo(sugerenciasADevolver);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            cupoManagerMock.Verify(x => x.DevolverSugerenciasMasivo(It.IsAny<List<DevolucionSugerenciaCupoDto>>()), Times.Once);
            var model = serializer.Deserialize<CupoResult>(serializer.Serialize(result.Data));
            Assert.AreEqual(false, model.HayErrores);
        }

    }
}