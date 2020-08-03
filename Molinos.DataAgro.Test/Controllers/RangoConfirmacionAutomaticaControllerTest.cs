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
    public class RangoConfirmacionAutomaticaControllerTest
    {
        private RangoConfirmacionAutomaticaController target;
        private Mock<IComercialManager> comercialManagerMock;
        private Mock<IRangoConfirmacionAutomaticaManager> rangoManagerMock;
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            comercialManagerMock = new Mock<IComercialManager>();
            rangoManagerMock = new Mock<IRangoConfirmacionAutomaticaManager>();
            target = new RangoConfirmacionAutomaticaController(comercialManagerMock.Object, rangoManagerMock.Object);

            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            HttpContext.Current.Session["comercialId"] = 1;
        }

        [Test]
        public void InicializarRangoPrecioTest()
        {
            rangoManagerMock.Setup(x => x.TraerDatosIniciales()).Returns(new DatosIniAbmRangoConfirmacionAutomatica
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
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Datos\":{\"Material\":[{\"MaterialId\":1,\"Descripcion\":\"A\",\"Codigo\":null,\"Campaña\":null}],\"Moneda\":[{\"MonedaId\":\"A\",\"Descripcion\":\"A\"}],\"Zona\":null,\"TipoNegocio\":null},\"RangoConfirmacion\":{\"Id\":0,\"PrecioMinimo\":0,\"PrecioMaximo\":0,\"MaterialId\":0,\"MonedaId\":null,\"FechaDesde\":\"\\/Date(-62135586000000)\\/\",\"FechaHasta\":\"\\/Date(-62135586000000)\\/\",\"ZonaId\":null,\"Cantidad\":0,\"DesdeMes\":0,\"DesdeAnio\":0,\"HastaMes\":0,\"HastaAnio\":0,\"TipoNegocioId\":0,\"UsuarioCreadorId\":null,\"FechaCreacion\":null,\"TipoNegocio\":null,\"Material\":null,\"Moneda\":null,\"Zona\":null,\"Comercial\":null},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void BuscarRangoTest()
        {
            rangoManagerMock.Setup(x => x.TraerTodoRangoDisponible()).Returns(new ResultIniRangoConfirmacionAutomatica
            {
                Rango = new List<RangoConfirmacionAutomaticaIni>()
                {
                    new RangoConfirmacionAutomaticaIni()
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
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Datos\":[{\"Id\":1,\"PrecioMinimo\":1,\"PrecioMaximo\":1,\"Material\":\"A\",\"Moneda\":\"A\",\"FechaDesde\":\"\\/Date(-62135586000000)\\/\",\"FechaHasta\":\"\\/Date(-62135586000000)\\/\",\"ZonaId\":0,\"Zona\":null,\"Cantidad\":0,\"EntregaDesde\":null,\"EntregaHasta\":null,\"TipoNegocio\":null,\"TipoNegocioId\":0}],\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void RangoComboTest()
        {
            var rangoPrecio = new RangoConfirmacionAutomaticaDto()
            {
                Id = 1,
                MaterialId = 1,
                MonedaId = "A",
                PrecioMaximo = 1,
                PrecioMinimo = 1
            };
            rangoManagerMock.Setup(x => x.TraerRango(1)).Returns(rangoPrecio);
            var result = target.RangoCombo(new AbmRangoConfirmacionAutomaticaParam() { Id = 1});

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Rango\":{\"Id\":1,\"PrecioMinimo\":1,\"PrecioMaximo\":1,\"Material\":null,\"MaterialId\":1,\"Moneda\":null,\"MonedaId\":\"A\",\"FechaDesde\":\"\\/Date(-62135586000000)\\/\",\"FechaHasta\":\"\\/Date(-62135586000000)\\/\",\"ZonaId\":0,\"Zona\":null,\"Cantidad\":null,\"DesdeMes\":null,\"DesdeAnio\":null,\"HastaMes\":null,\"HastaAnio\":null,\"TipoNegocioId\":0,\"TipoNegocio\":null,\"UsuarioCreadorId\":null,\"FechaCreacion\":null,\"UsuarioCreador\":null},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void AplicarRangoTest()
        {
            var rangoPrecio = new RangoConfirmacionAutomaticaDto()
            {
                Id = 1,
                MaterialId = 1,
                MonedaId = "A",
                PrecioMaximo = 1,
                PrecioMinimo = 1
            };
            rangoManagerMock.Setup(x => x.TraerRango(1)).Returns(rangoPrecio);
            var result = target.RangoCombo(new AbmRangoConfirmacionAutomaticaParam() { Id = 1 });

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Rango\":{\"Id\":1,\"PrecioMinimo\":1,\"PrecioMaximo\":1,\"Material\":null,\"MaterialId\":1,\"Moneda\":null,\"MonedaId\":\"A\",\"FechaDesde\":\"\\/Date(-62135586000000)\\/\",\"FechaHasta\":\"\\/Date(-62135586000000)\\/\",\"ZonaId\":0,\"Zona\":null,\"Cantidad\":null,\"DesdeMes\":null,\"DesdeAnio\":null,\"HastaMes\":null,\"HastaAnio\":null,\"TipoNegocioId\":0,\"TipoNegocio\":null,\"UsuarioCreadorId\":null,\"FechaCreacion\":null,\"UsuarioCreador\":null},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void GrabarRangoTest()
        {
            var rangoPrecio = new RangoConfirmacionAutomatica
            {
                Id = 1,
                MaterialId = 1,
                MonedaId = "A",
                PrecioMaximo = 1,
                PrecioMinimo = 1
            };
            rangoManagerMock.Setup(x => x.GrabarRangoConfirmacionAutomatica(rangoPrecio, It.IsAny<int>())).Returns(new Resultado { Errores = new List<ErrorMessage>() });
            var result = target.Grabar(rangoPrecio);

            Assert.NotNull(result);

            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Rango\":{\"Id\":1,\"PrecioMinimo\":1,\"PrecioMaximo\":1,\"Material\":null,\"MaterialId\":1,\"Moneda\":null,\"MonedaId\":\"A\",\"FechaDesde\":\"\\/Date(-62135586000000)\\/\",\"FechaHasta\":\"\\/Date(-62135586000000)\\/\",\"ZonaId\":0,\"Zona\":null,\"Cantidad\":null,\"DesdeMes\":null,\"DesdeAnio\":null,\"HastaMes\":null,\"HastaAnio\":null,\"TipoNegocioId\":0,\"TipoNegocio\":null,\"UsuarioCreadorId\":null,\"FechaCreacion\":null,\"UsuarioCreador\":null},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void EliminarRangoTest()
        {
            rangoManagerMock.Setup(x => x.EliminarRangoConfirmacionAutomatica(1)).Returns(new Resultado { Errores = new List<ErrorMessage>() });
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
