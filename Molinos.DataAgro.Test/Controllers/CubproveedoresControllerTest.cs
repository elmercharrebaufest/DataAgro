using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebDataAgro.Controllers;
using WebDataAgro.Models;
using static WebDataAgro.MvcApplication;

namespace Molinos.DataAgro.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class CubProveedoresControllerTest
    {
        private CubProveedoresController target;
        private Mock<ICubProveedoresManager> cubProveedoresManagerMock;
        private Mock<IHomeManager> homeManagerMock;
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();            
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            
            cubProveedoresManagerMock = new Mock<ICubProveedoresManager>();
            homeManagerMock = new Mock<IHomeManager>();
            target = new CubProveedoresController(cubProveedoresManagerMock.Object, homeManagerMock.Object);
        }

        [Test]
        public void IndexOk()
        {
            HttpContext.Current.Session["perfil"] = 1;
            var result = target.Index() as ViewResult;
            Assert.NotNull(result);
            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }

        [Test]
        public void InicializarTest()
        {
            HttpContext.Current.Session["equipo"] = new List<int> { 1, 2 };
            cubProveedoresManagerMock.Setup(x => x.TraerDatosIniciales(GlobalVariables.Equipo)).Returns(new DatosIniCubProveedores());
            cubProveedoresManagerMock.Setup(x => x.TraerParam()).Returns(new ParamCubProveedores { ComercialId=1, EstadoId=1,LocalidadId=1, proveedorId=1,provinciaId=1});
            var result = target.Inicializar();

            Assert.NotNull(result);
            cubProveedoresManagerMock.Verify(x => x.TraerDatosIniciales(It.IsAny<List<int>>()), Times.Once);
            cubProveedoresManagerMock.Verify(x => x.TraerParam(), Times.Once);
            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Datos\":{\"Proveedor\":null,\"Provincia\":null,\"Localidad\":null,\"Estado\":null,\"Segmentacion\":null,\"Material\":null,\"Comercial\":null},\"Param\":{\"proveedorId\":1,\"provinciaId\":1,\"LocalidadId\":1,\"EstadoId\":1,\"SegmentacionId\":null,\"material\":null,\"ComercialId\":1},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void ValidarTest()
        {
            var result = target.Validar(new ParamCubProveedores());

            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Proveedores\":[],\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void ListarConProveedoresTest()
        {
            var cub = new ParamCubProveedores { proveedorId = 1, provinciaId = 1, LocalidadId = 1, EstadoId = 1, SegmentacionId = 1, material = 1 };
            homeManagerMock.Setup(x => x.TraerIdComercial(GlobalVariables.IdActiveDirectory)).Returns(1);
            cubProveedoresManagerMock.Setup(x => x.TraerDatos(cub)).Returns(new ResultCubProveedores { Proveedores = new List<ProveedoresCub>() { new ProveedoresCub { RazonSocial = "A", CUIT = "201", Estado = "B" } }, Errores = new List<ErrorMessage>() });
            var result = target.Listar(cub);

            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            var model = serializer.Deserialize<ResultCubProveedores>(serializer.Serialize(result)); ;
            Assert.AreEqual(1, cub.ComercialId);
            Assert.IsTrue(model.Errores.Count == 0);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Proveedores\":[{\"CUIT\":\"201\",\"RazonSocial\":\"A\",\"Sergmentacion\":null,\"Estado\":\"B\",\"ContactoApellido\":null,\"AreaDeInfluencia\":null,\"MaterialCampaña\":null,\"Campaña\":null,\"ToneladasCompradas\":null,\"ToneladasObjetivo\":null,\"CampañaCampo\":null,\"MaterialCampo\":null,\"HectCampo\":null,\"TonCampo\":null,\"LocalidadCampo\":null,\"ProvinciaCampo\":null,\"CampañaAcopio\":null,\"PorcentajeAcopio\":null,\"TonAcopio\":null,\"LocalidadAcopio\":null,\"ProvinciaAcopio\":null}],\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void ListarSinProveedoresTest()
        {
            var cub = new ParamCubProveedores { proveedorId = 1, provinciaId = 1, LocalidadId = 1, EstadoId = 1, SegmentacionId = 1, material = 1 };
            homeManagerMock.Setup(x => x.TraerIdComercial(GlobalVariables.IdActiveDirectory)).Returns(1);
            cubProveedoresManagerMock.Setup(x => x.TraerDatos(cub)).Returns(new ResultCubProveedores { Proveedores = new List<ProveedoresCub>(), Errores = new List<ErrorMessage>() { new ErrorMessage { Message="A", ErrorCode=12} } });
            var result = target.Listar(cub);
            var data = result as JsonResult;
            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            var model = serializer.Deserialize<CubProveedoresModel>(serializer.Serialize(data.Data)); ;
            Assert.AreEqual(1, cub.ComercialId);
            Assert.IsTrue(model.Errores.Count > 1);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Proveedores\":[],\"Errores\":[{\"Item\":0,\"ErrorCode\":12,\"LogId\":0,\"Message\":\"A\",\"Source\":\"\",\"LargeDescription\":\"\",\"Translate\":false,\"Format\":\"\",\"Args\":[]},{\"Item\":0,\"ErrorCode\":0,\"LogId\":0,\"Message\":\"No hay datos para listar\",\"Source\":\"aviso\",\"LargeDescription\":\"\",\"Translate\":false,\"Format\":\"\",\"Args\":[]}],\"ListaErrores\":[{\"Item\":0,\"ErrorCode\":12,\"LogId\":0,\"Message\":\"A\",\"Source\":\"\",\"LargeDescription\":\"\",\"Translate\":false,\"Format\":\"\",\"Args\":[]},{\"Item\":0,\"ErrorCode\":0,\"LogId\":0,\"Message\":\"No hay datos para listar\",\"Source\":\"aviso\",\"LargeDescription\":\"\",\"Translate\":false,\"Format\":\"\",\"Args\":[]}],\"HayError\":true,\"HayErrores\":true},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
    }
}
