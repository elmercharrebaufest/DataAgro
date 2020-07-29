using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Report.Clases;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    public class HomeControllerTest
    {
        private HomeController target;
        private Mock<IHomeManager> homeManagerMock;
        private Mock<IComercialManager> comercialManagerMock;
        private Mock<IReportesManager> reportesManagerMock;
        private Mock<IObjetivoManager> objetivoManagerMock;
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            reportesManagerMock = new Mock<IReportesManager>();
            comercialManagerMock = new Mock<IComercialManager>();
            homeManagerMock = new Mock<IHomeManager>();
            objetivoManagerMock = new Mock<IObjetivoManager>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            HttpContext.Current.Session["comercialId"] = 1;
            target = new HomeController(comercialManagerMock.Object, reportesManagerMock.Object, homeManagerMock.Object, objetivoManagerMock.Object);
        }

        [Test]
        public void IndexOk()
        {
            HttpContext.Current.Session["perfil"] = 1;
            comercialManagerMock.Setup(x => x.ComercialExiste(GlobalVariables.IdActiveDirectory)).Returns(true);
            var result = target.Index() as ViewResult;

            Assert.NotNull(result);
            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }
        [Test]
        public void ErrorOk()
        {            
            var result = target.Error() as ViewResult;

            Assert.NotNull(result);
            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }
        [Test]
        public void ErrorDePermisosTest()
        {
            var result = target.ErrorDePermisos() as ViewResult;

            Assert.NotNull(result);
            Assert.That(result.ViewName, Is.Not.Null.Or.Empty);
        }
        [Test]
        public void ErrorUsuarioSinDerechosTest()
        {
            var result = target.ErrorUsuarioSinDerechos() as ViewResult;

            Assert.NotNull(result);
            Assert.That(result.ViewName, Is.Not.Null.Or.Empty);
        }
        [Test]
        public void InicializarTest()
        {
            HttpContext.Current.Session["perfil"] = 7;
            HttpContext.Current.Session["equipoReal"] = new List<int>();
            var filtro = new oParamBusqueda
            {
                ComercialId = GlobalVariables.ComercialId,
                Equipo = GlobalVariables.EquipoReal
            };
            homeManagerMock.Setup(x => x.TraerBusquedaContacto(filtro, 1, new List<int>())).Returns(new ResultIniContacto
            {
                Contactos = new List<ContactoIni>(),
                TotalBajaContactos = 0,
                TotalContactos = 0,
                TotalNoOperandoContactos = 0,
                TotalOperandoContactos = 0,
                TotalPotencialContactos = 0,
                TotalSinInteresContactos = 0
            });
            homeManagerMock.Setup(x => x.TraerInfoCampaña(GlobalVariables.ComercialId, GlobalVariables.EquipoReal)).Returns(new CampañaHome
            { Materiales = new List<MaterialCampaña> { new MaterialCampaña { Campaña = "17-18", Nombre = "a", Toneladas = 12 }},
            Nombre = "A"});
            homeManagerMock.Setup(x => x.TraerInfoIniciales(GlobalVariables.EquipoReal)).Returns(new DatosIniciales
            {
                mat = new List<MaterialesQry>(),
                camp = new List<CampañaQry>(),
                segm = new List<SegmentacionQry>(),
                tipoact = new List<TipoActividadQry>(),
                est = new List<EstadoQry>(),
                cond = new List<CondicionPreferenteQry>(),
                come = new List<ComercialQry>(),
                zona = new List<ZonaQry>()
            });
            homeManagerMock.Setup(x => x.TraerTodoCompraDetalle(GlobalVariables.EquipoReal)).Returns(new List<CompraDto>());
            var result = target.Inicializar();
            Assert.NotNull(result);
            homeManagerMock.Verify(x => x.TraerBusquedaContacto(It.IsAny<oParamBusqueda>(), It.IsAny<int>(), It.IsAny<List<int>>()), Times.Once);
            homeManagerMock.Verify(x => x.TraerInfoCampaña(It.IsAny<int>(),It.IsAny<List<int>>()), Times.Once);
            homeManagerMock.Verify(x => x.TraerInfoIniciales(It.IsAny<List<int>>()), Times.Once);
            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Contactos\":{\"Contactos\":null,\"TotalContactos\":0,\"TotalPotencialContactos\":0,\"TotalOperandoContactos\":0,\"TotalNoOperandoContactos\":0,\"TotalBajaContactos\":0,\"TotalSinInteresContactos\":0},\"Campaña\":null,\"Objetivo\":null,\"Datos\":null,\"Detalle\":null,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);        
        }

        [Test]
        public void BusquedaHomeTest()
        {
            HttpContext.Current.Session["equipo"] = new List<int>();
            HttpContext.Current.Session["perfil"] = EnumPerfil.CorredoresComercial;
            homeManagerMock.Setup(x => x.BusquedaHome("a", GlobalVariables.ComercialId,GlobalVariables.Equipo, GlobalVariables.CorredoresComercial)).Returns(new List<BusquedaHome>()
            {
                new BusquedaHome
                {
                    Cuit="1",
                    Filtro="a",
                    Id=1,
                    RazonSocial="ab"
                }
            });            
            var result = target.BusquedaHome("a");
            homeManagerMock.Verify(x => x.BusquedaHome(It.IsAny<string>(), It.IsAny<int>(),It.IsAny<List<int>>(), It.IsAny<List<int>>()), Times.Once);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"Id\":1,\"RazonSocial\":\"ab\",\"Cuit\":\"1\",\"Corredor\":null,\"Filtro\":\"a\"}],\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void TraerActividadesPorComercialIdTest()
        {
            homeManagerMock.Setup(x => x.TraerActividadesPorComercialId(GlobalVariables.ComercialId))
                .Returns(new List<ActividadRecordatorio>(){new ActividadRecordatorio()});
            var result = target.TraerActividadesPorComercialId();

            homeManagerMock.Verify(x => x.TraerActividadesPorComercialId(It.IsAny<int>()), Times.Once);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Actividades\":[{\"ActividadId\":0,\"Tema\":null,\"Comentarios\":null,\"Dia\":null,\"Hora\":null,\"Contacto\":null,\"ProveedorId\":0}],\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void ExportarContactosPDFTest()
        {
            var busqueda = new oParamBusqueda();

            homeManagerMock.Setup(x => x.ExportarContactos(busqueda, GlobalVariables.IdActiveDirectory, It.IsAny<List<int>>()))
                .Returns(new List<ContactoIni>(){new ContactoIni(){
                    Calificacion = 1,
                    ComercialCargo = "",
                    Corredor = true,
                    Cuit="",
                    Estado= "A",
                    GrupoDeCompras = "",
                    FechaAlta = DateTime.Now,
                    Mail="",
                    NoOperable= false,
                    Operando = true,
                    ProveedorId=1,
                    RazonSocial="a",
                    RptOpera="",
                    Telefono="",
                    TooltipNoOperable="",
                    UltimoContacto=""
                }});

            var result = target.ExportarContactosPDF(busqueda) as JsonResult;
            homeManagerMock.Verify(x => x.ExportarContactos(It.IsAny<oParamBusqueda>(), It.IsAny<string>(), It.IsAny<List<int>>()), Times.Once);

            Assert.NotNull(result);
            Assert.IsTrue(((ReportesModel)result.Data).DownloadKey != "");
        }
        [Test]
        public void ExportarContactosExcelTest()
        {
            var busqueda = new oParamBusqueda();

            homeManagerMock.Setup(x => x.ExportarContactos(busqueda, GlobalVariables.IdActiveDirectory, It.IsAny<List<int>>()))
                .Returns(new List<ContactoIni>() { new ContactoIni
                {
                    Calificacion = 1,
                    ComercialCargo = "",
                    Corredor = true,
                    Cuit="",
                    Estado= "A",
                    GrupoDeCompras = "",
                    FechaAlta = DateTime.Now,
                    Mail="",
                    NoOperable= false,
                    Operando = true,
                    ProveedorId=1,
                    RazonSocial="a",
                    RptOpera="",
                    Telefono="",
                    TooltipNoOperable="",
                    UltimoContacto=""
                }});

            var result = target.ExportarContactosPDF(busqueda) as JsonResult;
            homeManagerMock.Verify(x => x.ExportarContactos(It.IsAny<oParamBusqueda>(), It.IsAny<string>(), It.IsAny<List<int>>()), Times.Once);

            Assert.NotNull(result);
            Assert.IsTrue(((ReportesModel)result.Data).DownloadKey != "");
        }
        [Test]
        public void ExportarAllTest()
        {
            HttpContext.Current.Session["perfil"] = 1;
            var busqueda = new oParamBusqueda();

            homeManagerMock.Setup(x => x.ExportarAll(busqueda, GlobalVariables.IdActiveDirectory, GlobalVariables.Equipo))
                .Returns(new ExportAll
                {
                    agenda= new List<AgendaAll>() { new AgendaAll { } },
                    almacenamiento =new List<AlmacenamientoAll>(),
                    compras= new List<ComprasAll>(),
                    contacto= new List<ContactoAll>(),
                    ContactosPrincipales = new List<ContactosPrincipalesAll>(),
                    objetivo= new List<ObjetivoAll>(),
                    produccion = new List<ProduccionAll>()
                });

            var result = target.ExportarAll(busqueda) as JsonResult;
            homeManagerMock.Verify(x => x.ExportarAll(It.IsAny<oParamBusqueda>(), It.IsAny<string>(), It.IsAny<List<int>>()), Times.Once);

            Assert.NotNull(result);
            Assert.IsTrue(((ReportesModel)result.Data).DownloadKey != "");
        }

        [Test]
        public void TraerPostItTest()
        {
            homeManagerMock.Setup(x => x.TraerTexto(GlobalVariables.ComercialId)).Returns(new PostItDto());
            
            var result = target.TraerPostIt();
            
            homeManagerMock.Verify(x => x.TraerTexto(It.IsAny<int>()), Times.Once);

            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Texto\":null,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void GuardarPostItTest()
        {
            var postit = new PostIt { ComercialId = 1, Texto = "a" };
            homeManagerMock.Setup(x => x.GuardarPostIt(postit)).Returns(new GrabarPostItResult { ComercialId=1, Errores= new List<ErrorMessage>()});
            
            var result = target.GuardarPostIt(postit);

            homeManagerMock.Verify(x => x.GuardarPostIt(It.IsAny<PostIt>()), Times.Once);

            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"ComercialId\":1,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
    }
}
