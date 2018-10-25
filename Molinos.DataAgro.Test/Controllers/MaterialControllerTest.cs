using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Web;
using System.Web.Script.Serialization;
using WebDataAgro.Controllers;
using WebDataAgro.Models;

namespace Molinos.DataAgro.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class MaterialControllerTest
    {
        private MaterialController target;
        private Mock<IMaterialManager>materialManagerMock;
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();            
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            HttpContext.Current.Session["perfil"] = 1;
            materialManagerMock = new Mock<IMaterialManager>();
            target = new MaterialController(materialManagerMock.Object);
        }

        [Test]
        public void FiltrarMaterialTest()
        {
            var materialFiltro = new ParamAbmMaterial() { Codigo = "A", Descripcion = "A" };
            materialManagerMock.Setup(x => x.TraerFiltroMaterial(materialFiltro)).Returns(new ResultIniMaterial
            {
                Material = new List<MaterialIni>()
                {
                    new MaterialIni()
                    {
                        MaterialId = 1,
                        Codigo= "A",
                        Descripcion="A",
                        CampañaIdActual = 1
                    }
                }
            });
            var result = target.Filtrar(materialFiltro);

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Datos\":[{\"MaterialId\":1,\"Codigo\":\"A\",\"Descripcion\":\"A\",\"CampañaIdActual\":1}],\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void AplicarCondicionTest()
        {
            materialManagerMock.Setup(x => x.TraerMaterial(1)).Returns(new MaterialDto
            {
                MaterialId = 1,
                Codigo = "A",
                Descripcion = "A",
                CampañaId = 1
            }
            );
            var result = target.Aplicar(new AbmMaterialParam { MaterialId = 1 });

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Material\":{\"MaterialId\":1,\"Codigo\":\"A\",\"Descripcion\":\"A\",\"CampañaId\":1},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void GrabarCondicionTest()
        {
            var material = new Material
            {
                MaterialId = 1,
                Codigo = "A",
                Descripcion = "A",
                CampañaId = 1
            };
            materialManagerMock.Setup(x => x.GrabarMaterial(material)).Returns(new Resultado { Errores = new List<ErrorMessage>() });
            var result = target.Grabar(material);

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Material\":{\"MaterialId\":0,\"Codigo\":\"\",\"Descripcion\":\"\",\"CampañaId\":null,\"Campaña\":null},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void EliminarCondicionTest()
        {
            materialManagerMock.Setup(x => x.EliminarMaterial(1)).Returns(new Resultado { Errores = new List<ErrorMessage>() });
            var result = target.Eliminar(new AbmMaterialParam { MaterialId = 1});

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void CancelarCondicionTest()
        {
            var result = target.Cancelar();

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Material\":{\"MaterialId\":0,\"Codigo\":\"\",\"Descripcion\":\"\",\"CampañaId\":null,\"Campaña\":null},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
    }
}
