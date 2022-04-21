using Autofac.Extras.NLog;
using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Interfaces.Managers;
using Molinos.DataAgro.Repository;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebDataAgro.Controllers;
using WebDataAgro.Models;

namespace Molinos.DataAgro.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    class AdministracionCupoControllerTest
    {
        private AdministracionCupoController target;
        private Mock<ICupoManager> cupoManagerMock;
        private Mock<IAdministracionCupoManager> administracionManagerMock;


        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            cupoManagerMock = new Mock<ICupoManager>();
            administracionManagerMock = new Mock<IAdministracionCupoManager>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            target = new AdministracionCupoController(administracionManagerMock.Object, cupoManagerMock.Object);

            HttpContext.Current.Session["perfil"] = 1;
            HttpContext.Current.Session["comercialId"] = 1;

        }

        [Test]
        public void IndexOk()
        {
            cupoManagerMock.Setup(x => x.Panel())
                .Returns(new List<DiaCupo>());

            cupoManagerMock.Setup(x => x.FechasComprendidas(null))
                .Returns(new List<DateTime> { new DateTime(2018, 10, 26), new DateTime(2018, 10, 27), new DateTime(2018, 10, 28) });

            cupoManagerMock.Setup(x => x.SugerenciasNoAceptadas())
                .Returns(new List<SugerenciaNoAceptada>());

            var result = target.Index() as ViewResult;
            Assert.NotNull(result);
            Assert.IsEmpty(result.ViewName);
        }

        [Test]
        public void DatosAdministracion()
        {
            administracionManagerMock.Setup(x => x.TraerTodaAdministracionCupo(It.IsAny<KendoGridMvcRequest>(), null))
              .Returns(new KendoGrid<AdministracionCupoDto>(new List<AdministracionCupoDto> { new AdministracionCupoDto { Id = 1, CentroId = 1, MaterialId = 1, ComercialId = 1, ProveedorId = 1 } }, 1));
            var result = target.DatosAdministracion(new KendoGridMvcRequest());

            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            administracionManagerMock.Verify(x => x.TraerTodaAdministracionCupo(It.IsAny<KendoGridMvcRequest>(), null), Times.Once);
            Assert.AreEqual(
               "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Groups\":null,\"Data\":[{\"Id\":1,\"ProveedorId\":1,\"ComercialId\":1,\"Fecha\":\"\\/Date(-62135586000000)\\/\",\"CantidadCupo\":0,\"CantidadDeCupo\":0,\"CantidadFleteProcedencia\":0,\"CantidadDeCupoMax\":0,\"CantidadFleteProcedenciaMax\":0,\"EstadoId\":0,\"CentroId\":1,\"ZonaId\":0,\"MaterialId\":1,\"Comercial\":null,\"Proveedor\":null,\"Centro\":null,\"Zona\":null,\"Material\":null,\"StandardDeCalidad\":null,\"TipoNegocioId\":0,\"Destinatario\":null,\"NegocioId\":null,\"ConfiguracionEspacioDinamicoId\":null,\"Excedente\":false,\"Fason\":null,\"Estado\":null,\"TipoAdministracionCupo\":null,\"TipoAdministracionCupoId\":0,\"Observacion\":null,\"ComercialCreadorId\":null,\"FechaCreacion\":null,\"FechaDecision\":null,\"SugerenciaCupoId\":null,\"Hora\":null,\"ConDescarga\":null,\"FechaCreacionConHora\":null,\"Calidad\":null,\"Dias\":null,\"CantidadFleteProcedenciaOriginal\":0,\"CantidadDeCupoOriginal\":0}],\"Aggregates\":null,\"Total\":1},\"JsonRequestBehavior\":0,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
               a);
        }

        [Test]
        public void AceptarTest()
        {
            administracionManagerMock.Setup(x => x.AceptarCupoExcedente(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new CupoResult());
            var result = target.Aceptar(1, 1, 1,1,1, "");
            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            administracionManagerMock.Verify(x => x.AceptarCupoExcedente(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"ListaCupos\":[],\"CuposNormales\":0,\"CuposFlete\":0,\"Estados\":null,\"Codigo\":null,\"CupoNoPropios\":null,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void RechazarTest()
        {
            administracionManagerMock.Setup(x => x.CambiarEstadoRechazado(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new CupoResult());
            var result = target.Rechazar(1, "");
            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            administracionManagerMock.Setup(x => x.CambiarEstadoRechazado(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new CupoResult());
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"ListaCupos\":[],\"CuposNormales\":0,\"CuposFlete\":0,\"Estados\":null,\"Codigo\":null,\"CupoNoPropios\":null,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void AceptarMasivoTest()
        {
            var adm = new List<AdministracionCupoDto>() {
                new AdministracionCupoDto
                {
                    CantidadFleteProcedencia = 1,
                    CantidadDeCupo = 10,
                    CentroId = 1,
                    ComercialId = 63,
                    ConfiguracionEspacioDinamicoId = 1,
                    EstadoId = 1,
                    Id = 234,
                    MaterialId = 1,
                    Fecha = DateTime.Now,
                    Excedente = false,
                    TipoNegocioId = 1,
                    ZonaId = 1,
                }
            };
            administracionManagerMock.Setup(x => x.AceptarCupoExcedente(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new CupoResult());
            var result = target.AceptarMasivo(adm, "");
            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"ListaCupos\":[],\"CuposNormales\":0,\"CuposFlete\":0,\"Estados\":null,\"Codigo\":null,\"CupoNoPropios\":null,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void RechazarMasivoTest()
        {
            var adm = new List<AdministracionCupoDto>() {
                new AdministracionCupoDto
                {
                    CantidadFleteProcedencia = 1,
                    CantidadDeCupo = 10,
                    CentroId = 1,
                    ComercialId = 63,
                    ConfiguracionEspacioDinamicoId = 1,
                    EstadoId = 1,
                    Id = 234,
                    MaterialId = 1,
                    Fecha = DateTime.Now,
                    Excedente = false,
                    TipoNegocioId = 1,
                    ZonaId = 1,
                }
            };
            administracionManagerMock.Setup(x => x.CambiarEstadoRechazado(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new CupoResult());
            var result = target.RechazarMasivo(adm, "");
            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            administracionManagerMock.Setup(x => x.CambiarEstadoRechazado(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new CupoResult());
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"ListaCupos\":[],\"CuposNormales\":0,\"CuposFlete\":0,\"Estados\":null,\"Codigo\":null,\"CupoNoPropios\":null,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void PartialPanelTest()
        {
            cupoManagerMock.Setup(x => x.Panel()).Returns(new List<DiaCupo> { });
            cupoManagerMock.Setup(x => x.FechasComprendidas(It.IsAny<int?>())).Returns(new List<DateTime> { });
            cupoManagerMock.Setup(x => x.SugerenciasNoAceptadas()).Returns(new List<SugerenciaNoAceptada> { });

            var result = target.PartialPanel() as PartialViewResult;

            Assert.NotNull(result);

            cupoManagerMock.Verify(x => x.Panel(), Times.Once);
            cupoManagerMock.Verify(x => x.FechasComprendidas(It.IsAny<int?>()), Times.Once);
            cupoManagerMock.Verify(x => x.SugerenciasNoAceptadas(), Times.Once);

            Assert.AreEqual("PartialPanel", result.ViewName);
        }

        [Test]
        public void BuscarDatosSolicitudCupoTest()
        {
            administracionManagerMock.Setup(x => x.TraerTodaAdministracionCupo(It.IsAny<KendoGridMvcRequest>(), It.IsAny<int?>()))
              .Returns(new KendoGrid<AdministracionCupoDto>(new List<AdministracionCupoDto> { new AdministracionCupoDto { Id = 1, CentroId = 1, MaterialId = 1, ComercialId = 1, ProveedorId = 1 } }, 1));
            var result = target.BuscarDatosSolicitudCupo(new KendoGridMvcRequest(),It.IsAny<int?>());

            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            administracionManagerMock.Verify(x => x.TraerTodaAdministracionCupo(It.IsAny<KendoGridMvcRequest>(), It.IsAny<int?>()), Times.Once);
            Assert.AreEqual(
               "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Groups\":null,\"Data\":[{\"Id\":1,\"ProveedorId\":1,\"ComercialId\":1,\"Fecha\":\"\\/Date(-62135586000000)\\/\",\"CantidadCupo\":0,\"CantidadDeCupo\":0,\"CantidadFleteProcedencia\":0,\"CantidadDeCupoMax\":0,\"CantidadFleteProcedenciaMax\":0,\"EstadoId\":0,\"CentroId\":1,\"ZonaId\":0,\"MaterialId\":1,\"Comercial\":null,\"Proveedor\":null,\"Centro\":null,\"Zona\":null,\"Material\":null,\"StandardDeCalidad\":null,\"TipoNegocioId\":0,\"Destinatario\":null,\"NegocioId\":null,\"ConfiguracionEspacioDinamicoId\":null,\"Excedente\":false,\"Fason\":null,\"Estado\":null,\"TipoAdministracionCupo\":null,\"TipoAdministracionCupoId\":0,\"Observacion\":null,\"ComercialCreadorId\":null,\"FechaCreacion\":null,\"FechaDecision\":null,\"SugerenciaCupoId\":null,\"Hora\":null,\"ConDescarga\":null,\"FechaCreacionConHora\":null,\"Calidad\":null,\"Dias\":null,\"CantidadFleteProcedenciaOriginal\":0,\"CantidadDeCupoOriginal\":0}],\"Aggregates\":null,\"Total\":1},\"JsonRequestBehavior\":0,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
               a);
        }

        [Test]
        public void ActualizarSolicitudTest()
        {
            administracionManagerMock.Setup(x => x.ActualizarSolicitud(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
              .Returns("Ok");
            var result = target.ActualizarSolicitud(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>());

            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            administracionManagerMock.Verify(x => x.ActualizarSolicitud(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()), Times.Once);
            Assert.AreEqual(
               "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":\"Ok\",\"JsonRequestBehavior\":0,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
               a);
        }



    }
}
