using Kendo.DynamicLinq;
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
        private Mock<IHabilitacionCupoManager> habilitacionManagerMock;
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
            habilitacionManagerMock = new Mock<IHabilitacionCupoManager>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();

            target = new CupoController(centroManagerMock.Object,
                materialManagerMock.Object, zonaCupoManagerMock.Object,
                proveedorManagerMock.Object, cupoManagerMock.Object,
                comercialManagerMock.Object, habilitacionManagerMock.Object);
            centroManagerMock.Setup(y => y.TraerTodoCentro())
                .Returns(new ResultIniCentro { Centro = new List<CentroIni>() { new CentroIni { Id = 1, Descripcion = "a", CodigoSap = "1600" } } });
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
                PlantaId = "1600",
                CuitId = "A",
                Siguientes = ""
            };
            cupoManagerMock.Setup(x => x.ObtenerCupo(It.IsAny<int>(), null)).Returns(new CupoDto {
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
                CupoSap = "a",
                CentroCodigo = "1600"
            });

            centroManagerMock.Setup(x => x.ObtenerCentroPorCodigoSap(It.IsAny<string>())).Returns(new CentroDto { CodigoSap = "1600" });
            centroManagerMock.Setup(x => x.TraerCentro(It.IsAny<int>())).Returns(new CentroDto { NoPropio = false });
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
                PlantaId = "1600",
                CuitId = "A",
                Siguientes = null
                
            };
            cupoManagerMock.Setup(x => x.Validar(It.IsAny<Cupo>(), It.IsAny<int>(), It.IsAny<DateTime>()))
                .Returns(new CupoResult { Errores = new List<ErrorMessage>() });
            cupoManagerMock.Setup(x => x.GrabarCupo(It.IsAny<Cupo>(), It.IsAny<List<DiaCupo>>()))
                .Returns(new CupoResult { Errores = new List<ErrorMessage>() });
            centroManagerMock.Setup(x => x.ObtenerCentroPorCodigoSap(It.IsAny<string>())).Returns(new CentroDto { CodigoSap = "1600" });
            var result = target.CrearCupo(cupoModel) as ViewResult;

            Assert.NotNull(result);
            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }

        [Test]
        public void GuardarCupoTest()
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
                PlantaId = "1600",
                CuitId = "A",
                Siguientes = null

            };
            cupoManagerMock.Setup(x => x.Validar(It.IsAny<Cupo>(), It.IsAny<int>(), It.IsAny<DateTime>()))
                .Returns(new CupoResult { Errores = new List<ErrorMessage>() });
            cupoManagerMock.Setup(x => x.GrabarCupo(It.IsAny<Cupo>(), It.IsAny<List<DiaCupo>>()))
                .Returns(new CupoResult { Errores = new List<ErrorMessage>() });
            centroManagerMock.Setup(x => x.ObtenerCentroPorCodigoSap(It.IsAny<string>())).Returns(new CentroDto { CodigoSap = "1600" });
            var result = target.GuardarCupo(cupoModel) as JsonResult;

            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Result\":{\"Id\":0,\"Siguientes\":null,\"ProveedorDescripcion\":\"a\",\"Proveedor\":1,\"PlantaId\":\"1600\",\"MaterialId\":1,\"FechaEntrega\":\"\\/Date(1579489200000)\\/\",\"FechaHastaEntrega\":\"\\/Date(1579489200000)\\/\",\"CantidadCupos\":0,\"ZonaId\":1,\"FleteAcarreo\":false,\"CalidadId\":1,\"Observacion\":\"\",\"FasonId\":false,\"CuitId\":\"A\",\"ConDescarga\":null,\"Dias\":null,\"Resultado\":{\"ListaCupos\":[],\"CuposNormales\":0,\"CuposFlete\":0,\"Estados\":null,\"Codigo\":null,\"CupoNoPropios\":null,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"Negocio\":null,\"NegocioId\":null,\"NoPropio\":false,\"FechaIngreso\":\"\\/Date(-62135586000000)\\/\",\"CuposNoPropios\":null,\"Sustentable\":false,\"EPA\":false},\"Error\":{\"ListaCupos\":[],\"CuposNormales\":0,\"CuposFlete\":0,\"Estados\":null,\"Codigo\":null,\"CupoNoPropios\":null,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"irA\":\"\"},\"JsonRequestBehavior\":1,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
                a);
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
                PlantaId = "1600",
                CuitId = "A",
                Siguientes = null
            };
            cupoManagerMock.Setup(x => x.Validar(It.IsAny<Cupo>(), It.IsAny<int>(), It.IsAny<DateTime>()))
                .Returns(new CupoResult { Errores = new List<ErrorMessage>() });
            cupoManagerMock.Setup(x => x.GrabarCupo(It.IsAny<Cupo>(), It.IsAny<List<DiaCupo>>()))
                .Returns(new CupoResult { Errores = new List<ErrorMessage>() });
            centroManagerMock.Setup(x => x.ObtenerCentroPorCodigoSap(It.IsAny<string>())).Returns(new CentroDto { CodigoSap = "1600" });

            var result = (RedirectToRouteResult)target.CrearCupo(cupoModel);

            result.RouteValues["action"].Equals("Index");
            Assert.AreEqual("Index", result.RouteValues["action"]);          
        }
        [Test]
        public void BuscarProveedorTest()
        {
            proveedorManagerMock.Setup(x => x.DevolverProveedoresCorredores(It.IsAny<string>(), false, "",false)).Returns(new List<BusquedaHome>() { new BusquedaHome {Id=1,Cuit="1",RazonSocial="a" } });
            var result = target.BuscarProveedor("a");

            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":null,\"JsonRequestBehavior\":0,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
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
            cupoManagerMock.Setup(x => x.EliminarCupo(It.IsAny<int>(), It.IsAny<string>(), true))
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
            cupoManagerMock.Setup(x => x.TraerCupoDisponibilidad(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<string>())).Returns(new List<DisponibilidadCuposDto>() { new DisponibilidadCuposDto { Consumidos = 1, Fecha = new DateTime(2020,4,28).Date, Disponibles = 9, Limite = 10, MaterialCodigo = "000000000019908017", MaterialId = 3, MaterialNombre = "Soja", ZonaId = "CBA" } } );
            var result = target.BuscaDatosTablaDisponibilidad("","","",new List<string>(),"");

            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"Fecha\":\"\\/Date(1588042800000)\\/\",\"MaterialNombre\":\"Soja\",\"ZonaId\":\"CBA\",\"Disponibles\":9,\"Consumidos\":1,\"Limite\":10,\"MaterialCodigo\":\"000000000019908017\",\"MaterialId\":3,\"ZonaNombre\":null,\"CentroNombre\":null,\"CentroCodigo\":null}],\"JsonRequestBehavior\":0,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void RechazarCupoTest()
        {
            cupoManagerMock.Setup(x => x.RechazarCupo(It.IsAny<Cupo>(), It.IsAny<string>()))
                .Returns(new CupoResult { Errores = new List<ErrorMessage>() });
            var result = target.RechazarCupo(It.IsAny<Cupo>());

            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"ListaCupos\":[],\"CuposNormales\":0,\"CuposFlete\":0,\"Estados\":null,\"Codigo\":null,\"CupoNoPropios\":null,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void AceptarCupoTest()
        {
            cupoManagerMock.Setup(x => x.AceptarCupo(It.IsAny<Cupo>()))
                .Returns(new CupoResult { Errores = new List<ErrorMessage>() });
            var result = target.AceptarCupo(It.IsAny<Cupo>());

            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"ListaCupos\":[],\"CuposNormales\":0,\"CuposFlete\":0,\"Estados\":null,\"Codigo\":null,\"CupoNoPropios\":null,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void TraerNegocioConCupoDisponibleTest()
        {
            cupoManagerMock.Setup(x => x.TraerNegocioConCupoDisponible(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(new List<BasicoContrato>() { new BasicoContrato { MaterialId = 3 } });
            var result = target.TraerNegocioConCupoDisponible("", It.IsAny<int>(), It.IsAny<int>(), "", It.IsAny<DateTime>(), It.IsAny<DateTime>());

            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            //Assert.AreEqual(
            //    "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"ChequeElectronicoValor\":null,\"Id\":0,\"Cuit\":null,\"ContratoId\":0,\"MaterialId\":3,\"NivelTarifaId\":null,\"TipoNegocioId\":0,\"Cantidad\":0,\"Precio\":0,\"PrecioPlazo\":null,\"FechaEntrega\":null,\"CampanaId\":null,\"FechaDesde\":null,\"FechaDesdeFormateado\":null,\"FechaHasta\":null,\"FechaHastaFormateado\":null,\"ProveedorId\":0,\"MonedaId\":null,\"Moneda\":null,\"Fecha\":null,\"FechaFormateado\":null,\"Hora\":null,\"NivelTarifa\":null,\"TarifaFlete\":null,\"GrupoCompra\":0,\"GrupoCompraDescripcion\":null,\"ComercialId\":null,\"ComercialCreadorId\":null,\"ProvinciaId\":null,\"LocalidadId\":null,\"Base\":null,\"Importe_Sustentable\":null,\"MonedaId_Sustentable\":null,\"Moneda_Sustentable\":null,\"Fecha_Dolarizado\":null,\"Fecha_DolarizadoFormateado\":null,\"Dias_Pesificado\":null,\"NoInformaSIO\":null,\"TrigoEspecial\":null,\"Estado\":null,\"UsuarioId\":null,\"ContratoSAP\":null,\"Ampliaciones\":null,\"TipoNegocio\":null,\"Proveedor\":null,\"Corredor\":null,\"CUITCorredor\":null,\"Comercial\":null,\"ComercialCreador\":null,\"Material\":null,\"Campania\":null,\"Provincia\":null,\"Localidad\":null,\"Estado_Contrato\":null,\"Fecha_Order\":\"\\/Date(-62135586000000)\\/\",\"Estado_Order\":0,\"Observacion\":null,\"FijacionDePrecioContratoId\":null,\"Sustentable\":null,\"Dolarizado\":null,\"Pesificado\":null,\"Negocio\":null,\"ClasificacionId\":null,\"ClasificacionDescripcion\":null,\"DestinoId\":null,\"DestinoDescripcion\":null,\"CantidadCamiones\":null,\"Consignatario\":null,\"PlanCanje\":null,\"CondicionFijacion\":null,\"CD\":null,\"Warrant\":null,\"PagoDirectoVendedor\":null,\"CalidadDescripcion\":null,\"EstablecimientoPropio\":null,\"BoletoId\":null,\"BolsaId\":null,\"BoletoDescripcion\":null,\"BolsaDescripcion\":null,\"DesdeFijacion\":null,\"DesdeFijacionFormateado\":null,\"HastaFijacion\":null,\"HastaFijacionFormateado\":null,\"CondicionFijacionDescripcion\":null,\"Descuentos\":null,\"Calidades\":null,\"MercsDeposito\":null,\"CorredorId\":0,\"DatosFijacion\":null,\"PorcentajeComision\":null,\"ContratoCorredor\":null,\"ContratoVendedor\":null,\"SelCargoMOA\":null,\"SelCargoVendedor\":null,\"Madre\":null,\"EsFason\":null,\"ContratoMadre\":null,\"Posicion\":null,\"TipoFason\":null,\"TipoFasonId\":0,\"FasonId\":0,\"Operador\":null,\"OperadorId\":0,\"AgenteId\":0,\"AperturaPrecios\":null,\"PreciosPactados\":null,\"PrecioNeto\":null,\"Pizarra\":null,\"StandardCalidadId\":null,\"StandardDeCalidadDescripcion\":null,\"PagoDiferido\":null,\"ZonaId\":null,\"ZonaDescripcion\":null,\"AcuerdoId\":null,\"ImporteFinanciero\":null,\"ImporteRedespacho\":null,\"ImporteComision\":null,\"ImporteBonificacion\":null,\"PorcentajeBonificacion\":null,\"Compensacion\":null,\"Acuerdo\":null,\"Rechazo\":null,\"ComercialZonaId\":null,\"ComercialZonaDescripcion\":null,\"OcultarEnTablero\":false,\"FechaCiertaFormateado\":null,\"FechaCierta\":null,\"MonedaBonificacion\":null,\"MesPosicion\":null,\"CampanaMaterialId\":null,\"ContratoAcuerdoId\":null,\"PorcentajeDePago\":null,\"TipoAgenteCompraId\":null,\"CaratulaExtension\":null,\"CaratulaMAT\":null,\"PrecioAjusteComision\":null,\"MonedaAjusteComisionId\":null,\"FechaOperacion\":null,\"FechaOperacionFormateado\":null,\"MotivoOperacionAnterior\":null,\"UsuarioConfirmador\":null,\"FechaConfirmacion\":null,\"CantidadMaximaCupo\":0,\"ChequeElectronico\":null,\"DolarizadoExpress\":null,\"DolarizadoExpressValor\":null,\"PagoCBU\":null,\"DolarizadoValor\":null}],\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
            //    a);
            var data = (List<BasicoContrato>)((JsonResult)result).Data;
            Assert.AreEqual(data.Count, 1);
        }

        [Test]
        public void TraerEstablecimientosTest()
        {
            cupoManagerMock.Setup(x => x.TraerEstablecimientos(It.IsAny<string>(), It.IsAny<bool>())).Returns(new List<EstablecimientoStockDto>() { new EstablecimientoStockDto { Cantidad = 300, Cosecha = "20-21", Establecimiento = "Guard" } });
            var result = target.TraerEstablecimientos(It.IsAny<string>(), It.IsAny<bool>());

            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"Establecimiento\":\"Guard\",\"Cantidad\":300,\"Proveedor\":null,\"Cosecha\":\"20-21\",\"Localidad\":null,\"Provincia\":null,\"CodigoEstablecimiento\":null}],\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void ListarProveedorTodosTest()
        {
            proveedorManagerMock.Setup(x => x.ListarProveedorTodos(It.IsAny<string>()))
                .Returns(new List<ProveedorDto> { new ProveedorDto { ProveedorId = 1 } });
            var result = target.ListarProveedorTodos("");

            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"ProveedorId\":1,\"Proveedor\":null}],\"JsonRequestBehavior\":0,\"MaxJsonLength\":null,\"RecursionLimit\":null}",
                a);
        }
    }
}
