using Kendo.DynamicLinq;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using KendoFilter = Kendo.DynamicLinq.Filter;
using JsonResult = System.Web.Mvc.JsonResult;
using WebDataAgro.Controllers;

namespace Molinos.DataAgro.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class CupoNoPropioControllerTest
    {
        private CupoNoPropioController target;
        private Mock<ICentroManager> centroManagerMock;
        private Mock<IMaterialManager> materialManagerMock;
        private Mock<ICupoNoPropioManager> cupoNoPropioManagerMock;
        private Mock<IComercialManager> comercialManagerMock;
        private Mock<IHabilitacionCupoManager> habilitacionManagerMock;
        private JavaScriptSerializer serializer;

        [SetUp]
        public void Setup()
        {
            this.serializer = new JavaScriptSerializer();
            centroManagerMock = new Mock<ICentroManager>();
            materialManagerMock = new Mock<IMaterialManager>();
            cupoNoPropioManagerMock = new Mock<ICupoNoPropioManager>();
            comercialManagerMock = new Mock<IComercialManager>();
            habilitacionManagerMock = new Mock<IHabilitacionCupoManager>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();

            target = new CupoNoPropioController(centroManagerMock.Object, materialManagerMock.Object, cupoNoPropioManagerMock.Object,
                comercialManagerMock.Object, habilitacionManagerMock.Object);
            centroManagerMock.Setup(y => y.TraerTodoCentro())
                .Returns(new ResultIniCentro { Centro = new List<CentroIni>() { new CentroIni { Id = 1, Descripcion = "a", CodigoSap = "1600" } } });
            materialManagerMock.Setup(y => y.TraerTodoMaterial())
                .Returns(new ResultIniMaterial { Material = new List<MaterialIni>() { new MaterialIni { MaterialId = 1, Descripcion = "a" } } });
            HttpContext.Current.Session["equipo"] = new List<int>() { 1, 2 };
        }

        [Test]
        public void CrearCupoTest()
        {
            var cupoDto = new CupoDto
            {
                MaterialId = 1,
                Centro = "1029",
                FechaIngreso = DateTime.Now,
                Codigo = "MOA/2022"
            };
            cupoNoPropioManagerMock.Setup(x => x.GrabarCupoNoPropio(cupoDto)).Returns(new CupoResult());
            var result = target.CrearCupo(cupoDto);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            Assert.AreEqual("{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"ListaCupos\":[],\"CuposNormales\":0,\"CuposFlete\":0,\"Estados\":null,\"Codigo\":null,\"CupoNoPropios\":null,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":null,\"RecursionLimit\":null}"
                , a);
        }


        /*
        [Test]
        public void BuscaDatosTablaTest()
        {
            DataSourceRequest dsRequest = new DataSourceRequest()
            {
                Filter = new KendoFilter
                {
                    Logic = "and",
                    Filters = new List<KendoFilter>
                    {
                        new KendoFilter
                        {
                            Logic ="gte",
                            Value = "05/04/2022",
                           Field = "FechaIngreso"
                        }
                    }
                }
            };
            cupoNoPropioManagerMock.Setup(x => x.TraerCuposNoPropioTabla(dsRequest, It.IsAny<List<int>>()))
                .Returns(new DataSourceResult { Total = 1, Data = new List<CupoNoPropioDto>() { new CupoNoPropioDto { Id = 1, MaterialId = 1, CentroId = 13 } } });
            var result = target.BuscaDatosTabla(dsRequest) as JsonResult;
            Assert.NotNull(result);
            cupoNoPropioManagerMock.Verify(x => x.TraerCuposNoPropioTabla(It.IsAny<DataSourceRequest>(), It.IsAny<List<int>>()), Times.Once);
            var dsResult = result.Data as DataSourceResult;
            Assert.NotNull(dsResult);
            Assert.AreEqual(1, dsResult.Total);
            var items = dsResult.Data as List<CupoNoPropioDto>;
            Assert.NotNull(items);
            Assert.AreEqual(1, items[0].Id);
            Assert.AreEqual(13, items[0].CentroId);
            Assert.AreEqual(1, items[0].MaterialId);
        }
        */
        [Test]
        public void ModificarDisponibleTest()
        {
            Resultado res = new Resultado();
            cupoNoPropioManagerMock.Setup(x => x.GrabarDisponibilidadCupoNoPropio(It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(res);
            var result = target.ModificarDisponible(It.IsAny<int>(), It.IsAny<bool>());
            var a = serializer.Serialize(result);
            Assert.AreEqual("{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":0,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}"
                , a);
        }


        [Test]
        public void ModificacionMasivaDisponibleTest()
        {
            Resultado res = new Resultado();
            cupoNoPropioManagerMock.Setup(x => x.ModificacionMasivaDisponible(It.IsAny<List<int>>(), It.IsAny<bool>()))
                .Returns(res);
            var result = target.ModificacionMasivaDisponible(It.IsAny<List<int>>(), It.IsAny<bool>());
            var a = serializer.Serialize(result);
            Assert.AreEqual("{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":0,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}"
                , a);
        }
    }
}
