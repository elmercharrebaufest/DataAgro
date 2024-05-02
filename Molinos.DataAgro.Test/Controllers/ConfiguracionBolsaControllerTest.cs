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
    class ConfiguracionBolsaControllerTest
    {
        private ConfiguracionBolsaController target;
        private Mock<IContratoManager> contratoManagerMock;
        private Mock<IConfiguracionBolsaManager> bolsaManagerMock;


        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            contratoManagerMock = new Mock<IContratoManager>();
            bolsaManagerMock = new Mock<IConfiguracionBolsaManager>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            target = new ConfiguracionBolsaController(contratoManagerMock.Object, bolsaManagerMock.Object);
            HttpContext.Current.Session["perfil"] = 1;
            HttpContext.Current.Session["comercialId"] = 1;
        }

        [Test]
        public void IndexOk()
        {
            contratoManagerMock.Setup(x => x.TraerDatosCombo(null))
                .Returns(new DatosIniContrato()
                {
                    Bolsa = new List<BolsaCompraNetQry>()
                    {
                        new BolsaCompraNetQry { Descripcion = "Buenos Aires", Id = 1 }                   
                    },
                    Destino = new List<CentroQry>()
                    {
                        new CentroQry { Descripcion = "San Lorenzo", CodigoSap = "1029", Id = 1}
                    }
                });
            var result = target.Index() as ViewResult;
            Assert.NotNull(result);
            Assert.IsEmpty(result.ViewName);
        }

        [Test]
        public void TablaBolsaPartialTest()
        {
            var result = target.TablaBolsaPartial() as PartialViewResult;
            Assert.NotNull(result);
            Assert.AreEqual("_ListaBolsa", result.ViewName);
            Assert.IsInstanceOf<ConfiguracionBolsaModel>(result.Model);
        }
        [Test]
        public void GrabarConfiguracionBolsaTest()
        {
             contratoManagerMock.Setup(x => x.TraerDatosCombo(null))
                .Returns(new DatosIniContrato()
                {
                    Bolsa = new List<BolsaCompraNetQry>()
                    {
                        new BolsaCompraNetQry { Descripcion = "Buenos Aires", Id = 1 }                   
                    },
                    Destino = new List<CentroQry>()
                    {
                        new CentroQry { Descripcion = "San Lorenzo", CodigoSap = "1029", Id = 1}
                    }
                });
            bolsaManagerMock.Setup(x => x.GrabarConfiguracionBolsa(It.IsAny<ConfiguracionBolsa>()))
                .Returns(new Resultado());
            var result = target.GrabarConfiguracionBolsa(new ConfiguracionBolsaModel { Id = 1, DestinoId = 1, ProvinciaId = 1, BolsaId = 1 }) as RedirectToRouteResult;
            Assert.NotNull(result);          
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }
        [Test]
        public void DatosConfiguracionTest()
        {
            bolsaManagerMock.Setup(x => x.TraerTodaConfiguracionBolsa(It.IsAny<KendoGridMvcRequest>()))
                .Returns(new KendoGrid<ConfiguracionBolsaDto>(new List<ConfiguracionBolsaDto>() {
                    new ConfiguracionBolsaDto { Id = 1, DestinoId = 1, ProvinciaId = 1, BolsaId = 1 } }, 20));
            var result = target.DatosConfiguracion(new KendoGridMvcRequest());

            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            bolsaManagerMock.Verify(x => x.TraerTodaConfiguracionBolsa(It.IsAny<KendoGridMvcRequest>()), Times.Once);
            Assert.AreEqual(
               "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Groups\":null,\"Data\":[{\"Id\":1,\"DestinoId\":1,\"Destino\":null,\"ProvinciaId\":1,\"Provincia\":null,\"BolsaId\":1,\"Bolsa\":null}],\"Aggregates\":null,\"Total\":20},\"JsonRequestBehavior\":0,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
               a);
        }

        [Test]
        public void EditarConfiguracionBolsaTest()
        {
            bolsaManagerMock.Setup(x => x.TraerConfiguracionBolsa(It.IsAny<int>()))
                .Returns(new ConfiguracionBolsaDto { Id = 1, DestinoId = 1, ProvinciaId = 1, BolsaId = 1 });
            var result = target.EditarConfiguracionBolsa(1);

            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            bolsaManagerMock.Verify(x => x.TraerConfiguracionBolsa(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(
               "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Id\":1,\"DestinoId\":1,\"Destino\":null,\"ProvinciaId\":1,\"Provincia\":null,\"BolsaId\":1,\"Bolsa\":null},\"JsonRequestBehavior\":0,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
               a);
        }


        [Test]
        public void EliminarConfiguracionBolsaTest()
        {
            bolsaManagerMock.Setup(x => x.EliminarConfiguracionBolsa(It.IsAny<int>()))
                .Returns(new Resultado());
            var result = target.EliminarConfiguracionBolsa(1);

            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            bolsaManagerMock.Verify(x => x.EliminarConfiguracionBolsa(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(
               "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":0,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
               a);
        }

        [Test]
        public void TraerConfiguracionBolsaConDestinoYProcedenciaTest()
        {
            bolsaManagerMock.Setup(x => x.TraerConfiguracionBolsaConDestinoYProcedencia(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new ConfiguracionBolsa());
            var result = target.TraerConfiguracionBolsaConDestinoYProcedencia(1, 1);

            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            bolsaManagerMock.Verify(x => x.TraerConfiguracionBolsaConDestinoYProcedencia(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            Assert.AreEqual(
               "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Id\":0,\"DestinoId\":0,\"BolsaId\":0,\"ProvinciaId\":0,\"Destino\":null,\"Bolsa\":null,\"Provincia\":null},\"JsonRequestBehavior\":0,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
               a);
        }
    }
}
