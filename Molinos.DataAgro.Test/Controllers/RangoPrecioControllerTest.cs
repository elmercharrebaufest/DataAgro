using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using WebDataAgro.Controllers;
using WebDataAgro.Models;

namespace Molinos.DataAgro.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class RangoPrecioControllerTest
    {
        private RangoPrecioController target;
        private Mock<IComercialManager> comercialManagerMock;
        private Mock<IRangoManager> rangoManagerMock;
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            comercialManagerMock = new Mock<IComercialManager>();
            rangoManagerMock = new Mock<IRangoManager>();
            target = new RangoPrecioController(comercialManagerMock.Object, rangoManagerMock.Object);
        }

        [Test]
        public void InicializarRangoPrecioTest()
        {
            rangoManagerMock.Setup(x => x.TraerDatosIniciales()).Returns(new DatosIniAbmRango
            {
                Material = new List<MaterialCombo>()
                {
                    new MaterialCombo()
                    {
                        MaterialId = 1,
                        Descripcion = "A"
                    }
                },
                Moneda = new List<MonedaQry>()
                {
                    new MonedaQry()
                    {
                        MonedaId= "A",
                        Descripcion="A"
                    }
                }
            });
            var result = target.Inicializar();

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Datos\":{\"Material\":[{\"MaterialId\":1,\"Descripcion\":\"A\",\"Codigo\":null,\"Campaña\":null,\"IVA\":null}],\"Moneda\":[{\"MonedaId\":\"A\",\"Descripcion\":\"A\"}]},\"RangoPrecio\":{\"Id\":0,\"PrecioMinimo\":0,\"PrecioMaximo\":0,\"MaterialId\":0,\"MonedaId\":null,\"Material\":null,\"Moneda\":null},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void BuscarRangoTest()
        {
            rangoManagerMock.Setup(x => x.TraerTodoRango()).Returns(new ResultIniRango
            {
                Rango = new List<RangoIni>()
                {
                    new RangoIni()
                    {
                        Id = 1,
                        Material = "A",
                        Moneda="A",
                        PrecioMaximo=1,
                        PrecioMinimo=1
                    }
                }
            }
            );
            var result = target.Buscar();

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Datos\":[{\"Id\":1,\"PrecioMinimo\":1,\"PrecioMaximo\":1,\"Material\":\"A\",\"Moneda\":\"A\"}],\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void RangoComboTest()
        {
            var rangoPrecio = new RangoPrecioDto()
            {
                Id = 1,
                MaterialId = 1,
                MonedaId = "A",
                PrecioMaximo = 1,
                PrecioMinimo = 1
            };
            rangoManagerMock.Setup(x => x.TraerRango(1)).Returns(rangoPrecio);
            var result = target.RangoCombo(new AbmRangoParam() { Id = 1});

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Rango\":{\"Id\":1,\"PrecioMinimo\":1,\"PrecioMaximo\":1,\"Material\":null,\"MaterialId\":1,\"Moneda\":null,\"MonedaId\":\"A\"},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void AplicarRangoTest()
        {
            var rangoPrecio = new RangoPrecioDto()
            {
                Id = 1,
                MaterialId = 1,
                MonedaId = "A",
                PrecioMaximo = 1,
                PrecioMinimo = 1
            };
            rangoManagerMock.Setup(x => x.TraerRango(1)).Returns(rangoPrecio);
            var result = target.RangoCombo(new AbmRangoParam() { Id = 1 });

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Rango\":{\"Id\":1,\"PrecioMinimo\":1,\"PrecioMaximo\":1,\"Material\":null,\"MaterialId\":1,\"Moneda\":null,\"MonedaId\":\"A\"},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void GrabarRangoTest()
        {
            var rangoPrecio = new RangoPrecio
            {
                Id = 1,
                MaterialId = 1,
                MonedaId = "A",
                PrecioMaximo = 1,
                PrecioMinimo = 1
            };
            rangoManagerMock.Setup(x => x.GrabarRango(rangoPrecio)).Returns(new Resultado { Errores = new List<ErrorMessage>() });
            var result = target.Grabar(rangoPrecio);

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Rango\":{\"Id\":1,\"PrecioMinimo\":1,\"PrecioMaximo\":1,\"Material\":null,\"MaterialId\":1,\"Moneda\":null,\"MonedaId\":\"A\"},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void EliminarRangoTest()
        {
            rangoManagerMock.Setup(x => x.EliminarRango(1)).Returns(new Resultado { Errores = new List<ErrorMessage>() });
            var result = target.Eliminar(new AbmRangoParam { Id = 1});

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void CancelarCanalOperacionTest()
        {
            var result = target.Cancelar();

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Rango\":{\"Id\":0,\"PrecioMinimo\":0,\"PrecioMaximo\":0,\"Material\":null,\"MaterialId\":0,\"Moneda\":null,\"MonedaId\":null},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
    }
}
