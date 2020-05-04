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
using WebDataAgro.Models;

namespace Molinos.DataAgro.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class CupoControllerTest
    {
        private CupoController target;
        private Mock<ICentroManager> centroManagerMock;
        private Mock<IMaterialManager> materialManagerMock;
        private Mock<IZonaCupoManager> zonaCupoManagerMock;
        private Mock<IProveedorManager> proveedorManagerMock;
        private Mock<ICupoManager> cupoManagerMock;
        private Mock<IComercialManager> comercialManagerMock;
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            centroManagerMock = new Mock<ICentroManager>();
            materialManagerMock = new Mock<IMaterialManager>();
            zonaCupoManagerMock = new Mock<IZonaCupoManager>();
            proveedorManagerMock = new Mock<IProveedorManager>();
            cupoManagerMock = new Mock<ICupoManager>();
            comercialManagerMock = new Mock<IComercialManager>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();

            target = new CupoController(centroManagerMock.Object,
                materialManagerMock.Object, zonaCupoManagerMock.Object,
                proveedorManagerMock.Object, cupoManagerMock.Object,
                comercialManagerMock.Object);
            centroManagerMock.Setup(y => y.TraerTodoCentro())
                .Returns(new ResultIniCentro { Centro = new List<CentroIni>() { new CentroIni { Id = 1, Descripcion = "a" } } });
            materialManagerMock.Setup(y => y.TraerTodoMaterial())
                .Returns(new ResultIniMaterial { Material = new List<MaterialIni>() { new MaterialIni { MaterialId = 1, Descripcion = "a" } } });
            zonaCupoManagerMock.Setup(y => y.TraerTodoZonaCupo())
                .Returns(new ResultIniZonaCupo { ZonaCupo = new List<ZonaCupoIni>() { new ZonaCupoIni { Id = 1, Descripcion = "a" } } });
            comercialManagerMock.Setup(y => y.TraerComercial(It.IsAny<int>()))
                .Returns(new ComercialDto { Nombres = "a", Apellido = "a", ComercialId = 1 });
            HttpContext.Current.Session["equipo"] = new List<int>() { 1, 2 };
            HttpContext.Current.Session["equipoReal"] = new List<int>() { 1, 2 };
            HttpContext.Current.Session["comercialId"] = 1;
        }

        [Test]
        public void IndexTest()
        {
            var result = target.Index() as ViewResult;

            Assert.NotNull(result);

            Assert.NotNull(result);
            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }
        [Test]
        public void CrearCupoNuevoTest()
        {
            var result = target.CrearCupo(null, "") as ViewResult;

            Assert.NotNull(result);
            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }
        [Test]
        public void CrearCupoTest()
        {
            var cupoModel = new CupoModel
            {
                Id = 1,
                CalidadId = 1,
                CantidadCupos = null,
                FasonId = false,
                FechaEntrega = new DateTime(2020, 1, 20),
                FechaHastaEntrega = new DateTime(2020, 1, 20),
                FleteAcarreo = false,
                MaterialId = 1,
                ProveedorDescripcion = "a",
                Proveedor = 1,
                Observacion = "",
                ZonaId = 1,
                PlantaId = 1,
                CuitId = "A",
                Siguientes = ""
            };
            cupoManagerMock.Setup(x => x.ObtenerCupo(It.IsAny<int>())).Returns(new CupoDto {
                Id = 1,
                Calidad = "Camara",
                Fason = false,
                FechaIngreso = new DateTime(2020, 1, 20),
                FleteProcedencia = false,
                MaterialId = 1,
                Proveedor = "a",
                ProveedorId = 1,
                Observaciones = "",
                ZonaCupoId = 1,
                CentroId = 1,
                Destinatario = "A",
                CupoSap = "a"
            });
            var result = target.CrearCupo(1, "") as ViewResult;

            Assert.NotNull(result);
            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }
        [Test]
        public void CrearCupoOkTest()
        {
            var cupoModel = new CupoModel
            {
                Id = 0,
                CalidadId = 1,
                CantidadCupos = null,
                FasonId = false,
                FechaEntrega = new DateTime(2020, 1, 20),
                FechaHastaEntrega = new DateTime(2020, 1, 20),
                FleteAcarreo = false,
                MaterialId = 1,
                ProveedorDescripcion = "a",
                Proveedor = 1,
                Observacion = "",
                ZonaId = 1,
                PlantaId = 1,
                CuitId = "A",
                Siguientes = null
            };
            cupoManagerMock.Setup(x => x.Validar(It.IsAny<Cupo>(), It.IsAny<int>(), It.IsAny<DateTime>()))
                .Returns(new Resultado { Errores = new List<ErrorMessage>() });
            cupoManagerMock.Setup(x => x.GrabarCupo(It.IsAny<Cupo>(), It.IsAny<List<DiaCupo>>()))
                .Returns(new CupoResult { Errores = new List<ErrorMessage>() });
            var result = target.CrearCupo(cupoModel) as ViewResult;

            Assert.NotNull(result);
            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }
        [Test]
        public void ModificarCupoOkTest()
        {
            var cupoModel = new CupoModel
            {
                Id = 1,
                CalidadId = 1,
                CantidadCupos = null,
                FasonId = false,
                FechaEntrega = new DateTime(2020, 1, 20),
                FechaHastaEntrega = new DateTime(2020, 1, 20),
                FleteAcarreo = false,
                MaterialId = 1,
                ProveedorDescripcion = "a",
                Proveedor = 1,
                Observacion = "",
                ZonaId = 1,
                PlantaId = 1,
                CuitId = "A",
                Siguientes = null
            };
            cupoManagerMock.Setup(x => x.Validar(It.IsAny<Cupo>(), It.IsAny<int>(), It.IsAny<DateTime>()))
                .Returns(new Resultado { Errores = new List<ErrorMessage>() });
            cupoManagerMock.Setup(x => x.GrabarCupo(It.IsAny<Cupo>(), It.IsAny<List<DiaCupo>>()))
                .Returns(new CupoResult { Errores = new List<ErrorMessage>() });
            var result = (RedirectToRouteResult)target.CrearCupo(cupoModel);

            result.RouteValues["action"].Equals("Index");
            Assert.AreEqual("Index", result.RouteValues["action"]);          
        }
        [Test]
        public void BuscarProveedorTest()
        {
            proveedorManagerMock.Setup(x => x.DevolverProveedoresCorredores(It.IsAny<string>())).Returns(new List<BusquedaHome>() { new BusquedaHome {Id=1,Cuit="1",RazonSocial="a" } });
            var result = target.BuscarProveedor("a");

            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"Id\":1,\"RazonSocial\":\"a\",\"Cuit\":\"1\",\"Corredor\":null,\"Filtro\":null}],\"JsonRequestBehavior\":0,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
                a);
        }
        //[Test]
        //public void BuscaDatosTablaTest()
        //{
        //    cupoManagerMock.Setup(x => x.TraerCuposTabla(It.IsAny<KendoGridMvcRequest>(), It.IsAny<List<int>>()))
        //        .Returns(new KendoGridBinder.KendoGrid<CupoDto>(new List<CupoDto>() { new CupoDto
        //        {
        //            Id = 1,
        //            Calidad = "Camara",
        //            Fason = false,
        //            FechaIngreso = new DateTime(2020, 1, 20),
        //            FleteProcedencia = false,
        //            MaterialId = 1,
        //            Proveedor = "a",
        //            ProveedorId = 1,
        //            Observaciones = "",
        //            ZonaCupoId = 1,
        //            CentroId = 1,
        //            Destinatario = "A",
        //            CupoSap = "a",
        //            Acopio= true,
        //            Centro="a",
        //            Comercial="a",
        //            ComercialId=1,
        //            CupoStop="1",
        //            EstadoCupo="a",
        //            EstadoCupoId=1,
        //            Fecha= "1/1/2020",
        //            FechaGeneracion =new DateTime(2020,1,1),
        //            Hora="1:1",
        //            HoraIngreso=new DateTime(2020,1,1),
        //            Material="a",
        //            MensajeError="",
        //            ZonaCupo="a"
        //        } }, 20));
        //    //var result = target.BuscaDatosTabla(new KendoGridMvcRequest());

        //    //Assert.NotNull(result);
        //    //var a = serializer.Serialize(result);
        //    //Assert.AreEqual(
        //    //    "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Groups\":null,\"Data\":[{\"Id\":1,\"ProveedorId\":1,\"Proveedor\":\"a\",\"CentroId\":1,\"Centro\":\"a\",\"MaterialId\":1,\"Material\":\"a\",\"FechaIngreso\":\"\\/Date(1579489200000)\\/\",\"HoraIngreso\":\"\\/Date(1577847600000)\\/\",\"CupoSap\":\"a\",\"CupoStop\":\"1\",\"ZonaCupoId\":1,\"ZonaCupo\":\"a\",\"ComercialId\":1,\"Comercial\":\"a\",\"FleteProcedencia\":false,\"Calidad\":\"Camara\",\"Observaciones\":\"\",\"Fason\":false,\"Destinatario\":\"A\",\"FechaGeneracion\":\"\\/Date(1577847600000)\\/\",\"EstadoCupoId\":1,\"EstadoCupo\":\"a\",\"MensajeError\":\"\",\"Acopio\":true,\"Fecha\":\"1/1/2020\",\"Hora\":\"1:1\"}],\"Aggregates\":null,\"Total\":20},\"JsonRequestBehavior\":1,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
        //    //    a);
        //}
        [Test]
        public void EliminarCupoTest()
        {
            cupoManagerMock.Setup(x => x.EliminarCupo(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(new Resultado { Errores= new List<ErrorMessage>()});
            var result = target.EliminarCupo(1);

            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void ListarProveedorTest()
        {
            proveedorManagerMock.Setup(x => x.ListarProveedor(It.IsAny<string>()))
                .Returns(new List<ProveedorDto> { new ProveedorDto { ProveedorId = 1 } });
            var result = target.ListarProveedor("");

            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"ProveedorId\":1,\"Proveedor\":null}],\"JsonRequestBehavior\":0,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void ListarComercialTest()
        {
            comercialManagerMock.Setup(x => x.ListarComercial(It.IsAny<string>(), It.IsAny<List<int>>()))
                .Returns(new List<ComercialDto>() { new ComercialDto { ComercialId = 1 } });
            var result = target.ListarComercial("");

            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"ComercialId\":1,\"Comercial\":\" \"}],\"JsonRequestBehavior\":0,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void TransmitirCuposTest()
        {
            cupoManagerMock.Setup(x => x.TransmitirCupos(It.IsAny<List<string>>()))
                .Returns(new Resultado { Errores= new List<ErrorMessage>()});
            var result = target.TransmitirCupos(new List<string>() { "a"});

            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":0,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void EliminarVariosTest()
        {
            cupoManagerMock.Setup(x => x.EliminarVarios(It.IsAny<List<int>>(), It.IsAny<string>()))
                .Returns(new Resultado { Errores = new List<ErrorMessage>() });
            var result = target.EliminarVarios(new List<int>() { 1 });

            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":0,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void DisponibilidadTest()
        {
            var result = target.Disponibilidad() as ViewResult;

            Assert.NotNull(result);

            Assert.NotNull(result);
            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }

        [Test]
        public void BuscaDatosTablaDisponibilidadTest() {
            cupoManagerMock.Setup(x => x.TraerCupoDisponibilidad(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<string>())).Returns(new List<DisponibilidadCuposDto>() { new DisponibilidadCuposDto { Consumidos = "1", Fecha = new DateTime(2020,4,28).Date, Disponibles = "9", Limite = "10", MaterialCodigo = "000000000019908017", MaterialId = 3, MaterialNombre = "Soja", ZonaId = "CBA" } } );
            var result = target.BuscaDatosTablaDisponibilidad("","","",new List<string>(),"");

            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"Fecha\":\"\\/Date(1588042800000)\\/\",\"MaterialNombre\":\"Soja\",\"ZonaId\":\"CBA\",\"Disponibles\":\"9\",\"Consumidos\":\"1\",\"Limite\":\"10\",\"MaterialCodigo\":\"000000000019908017\",\"MaterialId\":3,\"ZonaNombre\":null}],\"JsonRequestBehavior\":0,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
    }
}
