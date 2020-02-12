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
    class SugerenciaCupoControllerTest
    {
        private SugerenciaCupoController target;
        private Mock<ILogger> loggerMock;
        private Mock<ICupoManager> cupoManagerMock;

        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            loggerMock = new Mock<ILogger>();
            cupoManagerMock = new Mock<ICupoManager>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            target = new SugerenciaCupoController(loggerMock.Object, cupoManagerMock.Object);

            HttpContext.Current.Session["perfil"] = 1;
            HttpContext.Current.Session["comercialId"] = 1;

        }

        [Test]
        public void IndexOk()
        {
            var result = target.Index() as ViewResult;

            Assert.NotNull(result);
            Assert.IsEmpty(result.ViewName);
        }

        [Test]
        public void DatosConfiguracionTest()
        {
            cupoManagerMock.Setup(x => x.ObtenerSugerenciaCupo(It.IsAny<int>()))
                .Returns(new List<SugerenciaCupoDto> { new SugerenciaCupoDto { Id = 1, CentroId = 1, MaterialId = 1, ComercialId = 1, ProveedorId = 1 } });
            var result = target.DatosConfiguracion(new KendoGridMvcRequest());

            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            cupoManagerMock.Verify(x => x.ObtenerSugerenciaCupo(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(
               "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Groups\":null,\"Data\":[{\"ComercialId\":1,\"Id\":1,\"MaterialId\":1,\"MaterialDesc\":null,\"Precio\":null,\"MonedaId\":null,\"AgenteCompraId\":null,\"FijacionDePrecioContratoId\":null,\"FasonId\":null,\"ContratoId\":null,\"ConfiguracionEspacioDinamicoId\":null,\"FechaDesde\":\"\\/Date(-62135586000000)\\/\",\"FechaHasta\":\"\\/Date(-62135586000000)\\/\",\"PrecioPizarra\":0,\"formula\":null,\"Puntuaciones\":{},\"PuntuacionesString\":null,\"PuntuacionTotal\":0,\"DestinoId\":0,\"CantidadDeCupos\":0,\"CantidadDeCuposMaximo\":0,\"ZonaDescrip\":null,\"Priorizado\":false,\"FechaSugerida\":\"\\/Date(-62135586000000)\\/\",\"ProveedorId\":1,\"CentroId\":1,\"MonedaDesc\":null,\"ProveedorCUIT\":null,\"ProveedorDesc\":null,\"TipoNegocioDesc\":null,\"Aceptado\":null,\"ZonaCupoId\":null,\"Destinatario\":null,\"StandardDeCalidad\":null,\"TipoNegocioId\":0,\"ContratoSAP\":null}],\"Aggregates\":null,\"Total\":1},\"JsonRequestBehavior\":0,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
               a);
        }

        //[Test]
        //public void EliminarTest()
        //{
        //    int id = 1;
        //    cupoManagerMock.Setup(x => x.EliminarSugerenciaCupo(id)).Returns(new Resultado { Errores = new List<ErrorMessage>() });
        //    var result = target.Eliminar(id);
        //    Assert.NotNull(result);
        //    var a = serializer.Serialize(result);

        //    cupoManagerMock.Verify(x => x.EliminarSugerenciaCupo(It.IsAny<int>()), Times.Once);
        //    Assert.AreEqual(
        //        "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":0,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
        //        a);
        //}

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
            //cupoManagerMock.Setup(x => x.RechazarSugerenciaCupo(It.IsAny<List<int>>(),It.IsAny<String>()));
            List<int> ids = new List<int>();
            ids.Add(1);
            var result = target.Rechazar(ids,"");
            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            cupoManagerMock.Verify(x => x.RechazarSugerenciaCupo(It.IsAny<List<int>>(), It.IsAny<String>()), Times.Once);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[],\"JsonRequestBehavior\":1,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
                a);
        }
    }
}
