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
    public class InformeComercialControllerTest
    {
        private InformeComercialController target;
        private Mock<ICondicionManager> condicionManagerMock;
        private Mock<IInformeComercialManager> informeComercialManagerMock;
        private Mock<IHomeManager> homeManagerMock;
        private Mock<IReportesManager> reportesManagerMock;
        private Mock<IComercialManager> mobjComercialManagerMock;
        private Mock<IMaterialManager> mobjMaterialManagerMock;
        private Mock<ICampañaManager> mobjCampaniaManagerMock;
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            reportesManagerMock = new Mock<IReportesManager>();
            condicionManagerMock = new Mock<ICondicionManager>();
            informeComercialManagerMock = new Mock<IInformeComercialManager>();
            mobjComercialManagerMock = new Mock<IComercialManager>();
            mobjMaterialManagerMock = new Mock<IMaterialManager>();
            mobjCampaniaManagerMock = new Mock<ICampañaManager>();
            homeManagerMock = new Mock<IHomeManager>();


            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            HttpContext.Current.Session["perfil"] = 1;
            target = new InformeComercialController(condicionManagerMock.Object, informeComercialManagerMock.Object, homeManagerMock.Object, reportesManagerMock.Object,
                  mobjComercialManagerMock.Object, mobjMaterialManagerMock.Object, mobjCampaniaManagerMock.Object);
        }

        [Test]
        public void IndexOk()
        {
            var result = target.Index() as ViewResult;

            Assert.NotNull(result);
            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }

        [Test]
        public void InformeAdministrativoOkTest()
        {
            mobjMaterialManagerMock.Setup(x => x.TraerTodoMaterial()).Returns(new ResultIniMaterial { Material = new List<MaterialIni>() });
            mobjCampaniaManagerMock.Setup(x => x.TraerTodoCampania()).Returns(new List<CampañaDto> { new CampañaDto { CampañaId = 2, Descripcion = "" } });
            mobjComercialManagerMock.Setup(x => x.TraerTodoComercial()).Returns(new ResultIniComercial
            {
                Comercial = new List<ComercialIni>()
                {
                    new ComercialIni()
                    {
                        ComercialId = 1,
                        Apellido = "A",
                        Nombres = "A",
                        PerDescripcion = "Mesa",
                        Rol = ""
                    }
                }
            });

            var result = target.InformeAdministrativo() as ViewResult;

            Assert.NotNull(result);
            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }
        [Test]
        public void ReporteOkTest()
        {

            var result = target.Reporte() as ViewResult;

            Assert.NotNull(result);
            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }

        [Test]
        public void BuscarTest()
        {

            condicionManagerMock.Setup(x => x.TraerTodoCondicion()).Returns(new ResultIniCondicion()
            {
                Condicion = new List<CondicionIni>()
                {
                    new CondicionIni()
                    {
                        CondicionId= 1,
                        Descripcion="A",
                    }
                }
            });
            var result = target.Buscar();
            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Datos\":[{\"CondicionId\":1,\"Descripcion\":\"A\",\"Inhabilitado\":null}],\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void GrabarInformeComercialTest()
        {
            var condicion = new Condicion() { CondicionId = 1, Descripcion = "A" };

            condicionManagerMock.Setup(x => x.GrabarCondicion(condicion)).Returns(new AbmCondicionResult()
            {
                Condicion = condicion,
                Errores = new List<ErrorMessage>()
            });
            var result = target.GrabarInformeComercial(condicion);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Condicion\":{\"CondicionId\":0,\"Descripcion\":\"\",\"Inhabilitado\":null},\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void ListarTest()
        {
            var informeComercial = new ParamInformeComercial()
            {

                ProveedorId = 1,
                Materiales = new List<ParamInformeComercialMaterial>()
                {
                    new ParamInformeComercialMaterial()
                    {
                        MaterialId=1,
                        Toneladas=1
                    }
                },
                Campaña = "18-19"
            };
            var rtaInforme = new RptInformeComercialInfo()
            {
                RazonSocial = "A",
                CUIT = "111",
                Campaña = "18-19"
            };
            homeManagerMock.Setup(x => x.TraerIdComercial(GlobalVariables.IdActiveDirectory)).Returns(10);
            informeComercialManagerMock.Setup(x => x.GrabarInformeComercial(informeComercial, 10, null, null, null, null, null, null)).Returns(new InformeResult()
            {
                InformeId = 1,
                Errores = new List<ErrorMessage>() { }
            });
            informeComercialManagerMock.Setup(x => x.GenerarInformeComercial(informeComercial, 1)).Returns(rtaInforme);
            informeComercialManagerMock.Setup(x => x.EnviarMailInformeComercial("downloadKey"));

            var result = target.Listar(informeComercial, null, null, null, null, null, null, null);

            homeManagerMock.Verify(x => x.TraerIdComercial(It.IsAny<string>()), Times.Once);
            informeComercialManagerMock.Verify(x => x.GrabarInformeComercial(It.IsAny<ParamInformeComercial>(), It.IsAny<int>(), It.IsAny<List<NuevoProduccion>>(), It.IsAny<List<NuevoAcopio>>(), It.IsAny<ContactoComercial>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int?>()), Times.Once);
            informeComercialManagerMock.Verify(x => x.GenerarInformeComercial(It.IsAny<ParamInformeComercial>(), It.IsAny<int>()), Times.Once);
            informeComercialManagerMock.Verify(x => x.EnviarMailInformeComercial(It.IsAny<string>()), Times.Once);

            var model = result.Result as JsonResult;
            Assert.NotNull(result);
            Assert.IsTrue(((ReportesModel)model.Data).DownloadKey != "");
        }

        [Test]
        public void ListarMaterialesTest()
        {
            informeComercialManagerMock.Setup(x => x.TraerInformeComercial(1)).Returns(new List<InformeComercialMaterialDisponible>()
            {
                new InformeComercialMaterialDisponible()
                {
                    CampañaId=1,
                    MaterialId=1,
                    ProveedorId =1
                }
            });
            informeComercialManagerMock.Setup(x => x.TraerInformeComercialGenerado(1)).Returns(new List<InformeGeneradoList>()
            {
                new InformeGeneradoList()
                {
                    Campaña="18-19",
                    Cuit="aaa",
                    InformeComercialId=1
                }
            });
            var result = target.ListarMateriales(new oParamInforme { filtro = 1 });

            informeComercialManagerMock.Verify(x => x.TraerInformeComercial(It.IsAny<int>()), Times.Once);
            informeComercialManagerMock.Verify(x => x.TraerInformeComercialGenerado(It.IsAny<int>()), Times.Once);

            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            Assert.AreEqual(
                //"{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"materiales\":[{\"ProveedorId\":1,\"MaterialId\":1,\"CampañaId\":1,\"Campaña\":null,\"Material\":null}],\"InformeGenerado\":[{\"InformeComercialId\":1,\"Cuit\":\"aaa\",\"RazonSocial\":null,\"Campaña\":\"18-19\",\"Materiales\":null,\"Comercial\":null,\"EstadoId\":0,\"EstadoInforme\":null}]},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"materiales\":[{\"ProveedorId\":1,\"MaterialId\":1,\"CampañaId\":1,\"Campaña\":null,\"Material\":null}],\"InformeGenerado\":[{\"InformeComercialId\":1,\"Cuit\":\"aaa\",\"RazonSocial\":null,\"Campaña\":\"18-19\",\"Materiales\":null,\"Comercial\":null,\"EstadoId\":0,\"EstadoInforme\":null,\"FechaAlta\":\"\\/Date(-62135586000000)\\/\"}]},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void ListarInformesTest()
        {
            informeComercialManagerMock.Setup(x => x.TraerInformesGenerados()).Returns(new List<InformeList>()
            {
                new InformeList()
                {
                    InformeComercialId=1,
                    Campaña="18-19",
                    Cuit="12",
                    Comercial="a",
                    RazonSocial="C",
                    Seleccionado = true,
                    MaterialesList = new List<string>{ ""},
                    MaterialesIdList = new List<int?>{ 1}
                }
            });

            var result = target.ListarInformes();

            informeComercialManagerMock.Verify(x => x.TraerInformesGenerados(), Times.Once);

            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"InformeComercialId\":1,\"Cuit\":\"12\",\"RazonSocial\":\"C\",\"Campaña\":\"18-19\",\"MaterialesList\":[\"\"],\"Comercial\":\"a\",\"Seleccionado\":true,\"ProveedorId\":0,\"CampanaId\":null,\"ComercialId\":null,\"MaterialesIdList\":[1],\"Materiales\":\"\",\"MaterialId\":\"1\",\"FechaAlta\":null,\"FechaDescarga\":null,\"OrigenDA\":null}],\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }

        [Test]
        public void GenerarExcelTest()
        {
            informeComercialManagerMock.Setup(x => x.TraerCapacidadProductiva("A")).Returns(new List<ResultCapacidadProductiva>()
            {
                new ResultCapacidadProductiva()
                {
                    Maiz=1,
                    Proveedor="A",
                    Soja=2,
                    Trigo=3
                }
            });
            informeComercialManagerMock.Setup(x => x.GrabarCapacidadProductiva("A")).Returns(1);
            var result = target.GenerarExcel(new oParamExcel { Informes = "A" });

            informeComercialManagerMock.Verify(x => x.TraerCapacidadProductiva(It.IsAny<string>()), Times.Once);
            informeComercialManagerMock.Verify(x => x.GrabarCapacidadProductiva(It.IsAny<string>()), Times.Once);

            var model = result.Result as JsonResult;
            Assert.NotNull(result);
            Assert.IsTrue(((ReportesModel)model.Data).DownloadKey != "");
        }
        [Test]
        public void GenerarExcelIATest()
        {
            var repo = new ParamReportesIC { ComercialID = 1, ComercialIDGenerador = 2, Cuit = "A", EstadoId = 1, MaterialID = 2 };
            homeManagerMock.Setup(x => x.TraerIdComercial(GlobalVariables.IdActiveDirectory)).Returns(10);
            informeComercialManagerMock.Setup(x => x.ListarReportes(repo, It.IsAny<List<int>>())).Returns(new List<ReportesList>()
            {
                new ReportesList{Cuit="1",Comercial="B",Estado="C",InformeComercialId=2,Material="D",RazonSocial="E"}
            });

            var result = target.GenerarExcelIA(repo);
            homeManagerMock.Verify(x => x.TraerIdComercial(It.IsAny<string>()), Times.Once);
            informeComercialManagerMock.Verify(x => x.ListarReportes(It.IsAny<ParamReportesIC>(), It.IsAny<List<int>>()), Times.Once);

            var model = result.Result as JsonResult;
            Assert.NotNull(result);
            Assert.IsTrue(((ReportesModel)model.Data).DownloadKey != "");
        }
        [Test]
        public void ReImprimirPDFTest()
        {
            var informeComercial = new ParamInformeComercial()
            {
                ProveedorId = 1,
                Materiales = new List<ParamInformeComercialMaterial>()
                {
                    new ParamInformeComercialMaterial()
                    {
                        MaterialId=1,
                        Toneladas=1
                    }
                },
                Campaña = "18-19"
            };
            var rtaInforme = new RptInformeComercialInfo()
            {
                RazonSocial = "A",
                CUIT = "111",
                Campaña = "18-19"
            };
            informeComercialManagerMock.Setup(x => x.ReimprimirInformeComercial(1)).Returns(informeComercial);

            informeComercialManagerMock.Setup(x => x.GenerarInformeComercial(informeComercial, 1)).Returns(rtaInforme);
            var result = target.ReImprimirPDF(1);

            informeComercialManagerMock.Verify(x => x.ReimprimirInformeComercial(It.IsAny<int>()), Times.Once);
            informeComercialManagerMock.Verify(x => x.GenerarInformeComercial(It.IsAny<ParamInformeComercial>(), It.IsAny<int>()), Times.Once);

            var model = result.Result as JsonResult;
            Assert.NotNull(result);
            Assert.IsTrue(((ReportesModel)model.Data).DownloadKey != "");
        }
        [Test]
        public void EliminarInformeComercialTest()
        {
            informeComercialManagerMock.Setup(x => x.EliminarInformes(1)).Returns(new Resultado());

            var result = target.EliminarInformeComercial(1);

            informeComercialManagerMock.Verify(x => x.EliminarInformes(It.IsAny<int>()), Times.Once);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void ModificarInformeComercialTest()
        {
            var informeComercial = new ParamInformeComercial()
            {
                ProveedorId = 1,
                Materiales = new List<ParamInformeComercialMaterial>()
                {
                    new ParamInformeComercialMaterial()
                    {
                        MaterialId=1,
                        Toneladas=1
                    }
                },
                Campaña = "18-19"
            };
            informeComercialManagerMock.Setup(x => x.ReimprimirInformeComercial(1)).Returns(informeComercial);
            informeComercialManagerMock.Setup(x => x.TraerInformeMateriales(1)).Returns(new List<MaterialesModificacionInforme>() { new MaterialesModificacionInforme { Material = "A", MaterialId = 1, Seleccionado = true } });

            var result = target.ModificarInformeComercial(1);

            informeComercialManagerMock.Verify(x => x.ReimprimirInformeComercial(It.IsAny<int>()), Times.Once);
            informeComercialManagerMock.Verify(x => x.TraerInformeMateriales(It.IsAny<int>()), Times.Once);
            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"parametros\":{\"ProveedorId\":1,\"Materiales\":[{\"MaterialId\":1,\"Toneladas\":1}],\"CampañaId\":0,\"Campaña\":\"18-19\",\"EmplRelDep\":false,\"EmplRelDepCant\":null,\"Rodados\":null,\"RodadosOtros\":null,\"Chacra\":null,\"ChacraOtros\":null,\"AntigActividad\":null,\"ActuacionProd\":null,\"ClienteAnt\":null,\"Comentarios\":null,\"Domicilio\":null,\"InformeComercialId\":null,\"FechaDescarga\":null,\"OrigenDA\":null},\"materiales\":[{\"MaterialId\":1,\"Material\":\"A\",\"Seleccionado\":true}],\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void ListarReportesConIdGeneradorTest()
        {
            var repo = new ParamReportesIC { ComercialID = 1, ComercialIDGenerador = 2, Cuit = "A", EstadoId = 1, MaterialID = 2 };
            informeComercialManagerMock.Setup(x => x.ListarReportes(repo, It.IsAny<List<int>>())).Returns(new List<ReportesList>()
            {
                new ReportesList{Cuit="1",Comercial="B",Estado="C",InformeComercialId=2,Material="D",RazonSocial="E"}
            });

            var result = target.ListarReportes(repo);

            informeComercialManagerMock.Verify(x => x.ListarReportes(It.IsAny<ParamReportesIC>(), It.IsAny<List<int>>()), Times.Once);

            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"Cuit\":\"1\",\"RazonSocial\":\"E\",\"FechaDeGeneracion\":null,\"Comercial\":\"B\",\"Estado\":\"C\",\"Material\":\"D\",\"Observaciones\":null,\"InformeComercialId\":2}],\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
        [Test]
        public void ListarReportesSinIdGeneradorTest()
        {
            var repo = new ParamReportesIC { ComercialID = 1, Cuit = "A", EstadoId = 1, MaterialID = 2 };
            informeComercialManagerMock.Setup(x => x.ListarReportes(repo, It.IsAny<List<int>>())).Returns(new List<ReportesList>()
            {
                new ReportesList{Cuit="1",Comercial="B",Estado="C",InformeComercialId=2,Material="D",RazonSocial="E"}
            });

            homeManagerMock.Setup(x => x.TraerIdComercial(GlobalVariables.IdActiveDirectory)).Returns(10);
            var result = target.ListarReportes(repo);

            informeComercialManagerMock.Verify(x => x.ListarReportes(It.IsAny<ParamReportesIC>(), It.IsAny<List<int>>()), Times.Once);

            Assert.NotNull(result);
            var a = serializer.Serialize(result);
            Assert.AreEqual(
                "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":[{\"Cuit\":\"1\",\"RazonSocial\":\"E\",\"FechaDeGeneracion\":null,\"Comercial\":\"B\",\"Estado\":\"C\",\"Material\":\"D\",\"Observaciones\":null,\"InformeComercialId\":2}],\"JsonRequestBehavior\":1,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}",
                a);
        }
    }
}
