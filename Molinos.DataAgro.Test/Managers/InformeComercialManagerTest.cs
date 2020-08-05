using Autofac.Extras.NLog;
using Molinos.DataAgro.Business;
using Molinos.DataAgro.Business.Managers;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq.Expressions;
using System.Web;
using System.Web.Script.Serialization;

namespace Molinos.DataAgro.Test.Managers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class InformeComercialManagerTest
    {
        private InformeComercialManager target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ILogger> logger;
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            logger = new Mock<ILogger>();
            repositorioMock = new Mock<IRepositorio>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();

            target = new InformeComercialManager(logger.Object, repositorioMock.Object);
        }

        [Test]
        public void TraerInformeComercialTestOk()
        {
            repositorioMock.Setup(y => y.SelStore<InformeComercialMaterialDisponible>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                            .Returns(new List<InformeComercialMaterialDisponible>() { new InformeComercialMaterialDisponible { ProveedorId = 1, CampañaId = 1, MaterialId = 1 } });
            var result = target.TraerInformeComercial(1);
            repositorioMock.Verify(x => x.SelStore<InformeComercialMaterialDisponible>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            Assert.NotNull(result);
            Assert.AreEqual(1, result.Count);
        }
        [Test]
        public void TraerInformeComercialGeneradoTestOk()
        {
            repositorioMock.Setup(y => y.SelStore<InformeGeneradoList>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                            .Returns(new List<InformeGeneradoList>() { new InformeGeneradoList { Materiales = "a|a" } });
            var result = target.TraerInformeComercialGenerado(1);
            repositorioMock.Verify(x => x.SelStore<InformeGeneradoList>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            Assert.NotNull(result);
            Assert.AreEqual(1, result.Count);
        }
        [Test]
        public void GrabarInformeComercialTestOk()
        {
            var param = new ParamInformeComercial
            {
                InformeComercialId = 1,
                CampañaId = 1,
                Chacra = 1,
                ProveedorId = 1,
                ActuacionProd = "a",
                EmplRelDep = true,
                Materiales = new List<ParamInformeComercialMaterial>() { new ParamInformeComercialMaterial { MaterialId = 1, Toneladas = 100 } }
            };
            var nuevoCampo = new List<NuevoProduccion> { new NuevoProduccion { ArrendaPropia = false, CampañaId = 1, Hectareas = 1, LocalidadId = 1, MaterialId = 1, Toneladas = 1 } };
            var nuevoAcopio = new List<NuevoAcopio> { new NuevoAcopio { ArrendaPropia = false, CampañaId = 1, LocalidadId = 1, Toneladas = 1 } };

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<InformeComercialProduccion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<InformeComercialProduccion>() { new InformeComercialProduccion { MaterialId = 1 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<InformeComercialAlmacenamiento, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<InformeComercialAlmacenamiento>() { new InformeComercialAlmacenamiento() });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<InformeComercial, bool>>>()))
                            .Returns(new InformeComercial() { Campaña = new Campaña() });
            repositorioMock.Setup(y => y.Obtener<Comercial>(It.IsAny<int>()))
                            .Returns(new Comercial());
            repositorioMock.Setup(y => y.Agregar(It.IsAny<InformeComercial>()))
                            .Returns(new InformeComercial());

            repositorioMock.Setup(y => y.ListarConsulta(It.IsAny<TraerInformeComercialProduccion>()))
                            .Returns(new List<InformeComercialProduccionDto>() { new InformeComercialProduccionDto() });
            repositorioMock.Setup(y => y.ListarConsulta(It.IsAny<TraerInformeComercialAlmacenamiento>()))
                            .Returns(new List<InformeComercialAlmacenamientoDto>() { new InformeComercialAlmacenamientoDto() });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Material, int>>>(),It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<int>() { 1 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Campaña, int>>>(), It.IsAny<Expression<Func<Campaña, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<int>() { 1 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Localidad, int>>>(), It.IsAny<Expression<Func<Localidad, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<int>() { 1 });
           
            var result = target.GrabarInformeComercial(param, 1, nuevoCampo, nuevoAcopio);

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<InformeComercialProduccion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Remover(It.IsAny<List<InformeComercialProduccion>>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<InformeComercialAlmacenamiento, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Remover(It.IsAny<List<InformeComercialAlmacenamiento>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<InformeComercial, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<Comercial>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<InformeComercial>()), Times.Once);
            repositorioMock.Verify(x => x.ListarConsulta(It.IsAny<TraerInformeComercialProduccion>()), Times.Once);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<InformeComercialProduccion>()), Times.Once);
            repositorioMock.Verify(x => x.ListarConsulta(It.IsAny<TraerInformeComercialAlmacenamiento>()), Times.Once);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<InformeComercialAlmacenamiento>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(3));
            Assert.NotNull(result);

            Assert.IsFalse(result.HayError);
        }
        [Test]
        public void GenerarInformeComercialTestOk()
        {
            var param = new ParamInformeComercial
            {
                InformeComercialId = 1,
                CampañaId = 1,
                Chacra = 1,
                ProveedorId = 1,
                ActuacionProd = "a",
                EmplRelDep = true,
                Materiales = new List<ParamInformeComercialMaterial>() { new ParamInformeComercialMaterial { MaterialId = 1, Toneladas = 100 } }
            };
            repositorioMock.Setup(y => y.Obtener<Proveedor>(It.IsAny<int>()))
                            .Returns(new Proveedor() { Segmentacion = new Segmentacion { Grupo = "Acopiadores" } });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<ProveedorComercial, bool>>>(), It.IsAny<Expression<Func<ProveedorComercial, int>>>()))
                            .Returns(1);
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ContactoComercial, RptContactosInfo>>>(), It.IsAny<Expression<Func<ContactoComercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<RptContactosInfo>() { new RptContactosInfo() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<InformeComercialProduccion, InformeComercialAcopiadores>>>(), It.IsAny<Expression<Func<InformeComercialProduccion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<InformeComercialAcopiadores>() { new InformeComercialAcopiadores() });

            repositorioMock.Setup(y => y.ObtenerConsultaEscalar(It.IsAny<ObtenerToneladasPorMaterial>()))
                            .Returns(1000);
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<InformeComercialAlmacenamiento, InformeComercialAcopiadores>>>(), It.IsAny<Expression<Func<InformeComercialAlmacenamiento, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<InformeComercialAcopiadores>() { new InformeComercialAcopiadores() });

            var result = target.GenerarInformeComercial(param, 1);

            repositorioMock.Verify(x => x.Obtener<Proveedor>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<ProveedorComercial, bool>>>(), It.IsAny<Expression<Func<ProveedorComercial, int>>>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<ContactoComercial, RptContactosInfo>>>(), It.IsAny<Expression<Func<ContactoComercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<InformeComercialProduccion, InformeComercialAcopiadores>>>(), It.IsAny<Expression<Func<InformeComercialProduccion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.ObtenerConsultaEscalar(It.IsAny<ObtenerToneladasPorMaterial>()), Times.Exactly(5));
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<InformeComercialAlmacenamiento, InformeComercialAcopiadores>>>(), It.IsAny<Expression<Func<InformeComercialAlmacenamiento, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);

            Assert.NotNull(result);
        }
        [Test]
        public void ReimprimirInformeComercialTestOk()
        {
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<InformeComercial, bool>>>()))
                            .Returns(new InformeComercial()
                            {
                                ActuacionProd = "a",
                                AntigActividad = "a",
                                Campaña = new Campaña(),
                                CampañaId = 1,
                                Chacra = 1,
                                ChacraOtros = "a",
                                ClienteAnt = "a",
                                Comentarios = "a",
                                Comercial = new Comercial(),
                                ComercialId = 1,
                                DomicilioReal = "a",
                                EmplRelDep = true,
                                EmplRelDepCant = "a",
                                Estado = new InformeComercialEstado { Descripcion = "a", EstadoInformeId = 1 },
                                EstadoId = 1,
                                FechaAlta = DateTime.Now,
                                InformeComercialId = 1,
                                Proveedor = new Proveedor(),
                                ProveedorId = 1,
                                RespuestaSap = "a",
                                Rodados = 1,
                                RodadosOtros = "a"
                            });

            var result = target.ReimprimirInformeComercial(1);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<InformeComercial, bool>>>()), Times.Once);

            Assert.NotNull(result);
        }
        [Test]
        public void TraerInformeMaterialesTestOk()
        {
            repositorioMock.Setup(y => y.SelStore<MaterialesModificacionInforme>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                            .Returns(new List<MaterialesModificacionInforme>() { new MaterialesModificacionInforme { Seleccionado = true, Material = "a", MaterialId = 1 } });

            var result = target.TraerInformeMateriales(1);

            repositorioMock.Verify(x => x.SelStore<MaterialesModificacionInforme>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);

            Assert.NotNull(result);
        }
        [Test]
        public void TraerInformesGeneradosTestOk()
        {
            repositorioMock.Setup(y => y.SelStore<InformeList>(It.IsAny<string>(), It.IsAny<int>()))
                            .Returns(new List<InformeList>() { new InformeList { Cuit = "1", Campaña = "a", Comercial = "a", InformeComercialId = 1, Materiales = "a", RazonSocial = "a", Seleccionado = true } });

            var result = target.TraerInformesGenerados();

            repositorioMock.Verify(x => x.SelStore<InformeList>(It.IsAny<string>(), It.IsAny<int>()), Times.Once);

            Assert.NotNull(result);
            Assert.AreEqual(1, result.Count);
        }
        [Test]
        public void TraerCapacidadProductivaTestOk()
        {
            repositorioMock.Setup(y => y.SelStore<ResultCapacidadProductiva>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
                            .Returns(new List<ResultCapacidadProductiva>() { new ResultCapacidadProductiva
                            {Proveedor="a",Maiz=124,Soja=123,Trigo=1322}});

            var result = target.TraerCapacidadProductiva("a");

            repositorioMock.Verify(x => x.SelStore<ResultCapacidadProductiva>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()), Times.Once);

            Assert.NotNull(result);
            Assert.AreEqual(1, result.Count);
        }
        [Test]
        public void GrabarCapacidadProductivaTestOk()
        {
            repositorioMock.Setup(y => y.SelStore<Result>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
                            .Returns(new List<Result>() { new Result { Res = 1 } });

            var result = target.GrabarCapacidadProductiva("a");

            repositorioMock.Verify(x => x.SelStore<Result>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()), Times.Once);

            Assert.NotNull(result);
            Assert.AreEqual(1, result);
        }
        [Test]
        public void EliminarInformesTestOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<InformeComercialProduccion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<InformeComercialProduccion>() { new InformeComercialProduccion() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<InformeComercialAlmacenamiento, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                           .Returns(new List<InformeComercialAlmacenamiento>() { new InformeComercialAlmacenamiento() });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<InformeComercial, bool>>>()))
                           .Returns(new InformeComercial
                           {
                               ActuacionProd = "a",
                               AntigActividad = "a",
                               CampañaId = 1,
                               Proveedor = new Proveedor(),
                               Campaña = new Campaña(),
                               Chacra = 1,
                               ChacraOtros = "a",
                               ClienteAnt = "a",
                               Comentarios = "a",
                               Comercial = new Comercial(),
                               ComercialId = 1,
                               DomicilioReal = "a",
                               EmplRelDep = true,
                               EmplRelDepCant = "a",
                               Estado = new InformeComercialEstado(),
                               EstadoId = 1,
                               FechaAlta = DateTime.Now,
                               InformeComercialId = 1,
                               ProveedorId = 1,
                               RespuestaSap = "a",
                               Rodados = 1,
                               RodadosOtros = "a"
                           });
            var result = target.EliminarInformes(1);

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<InformeComercialProduccion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.RemoverTodos(It.IsAny<List<InformeComercialProduccion>>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<InformeComercialAlmacenamiento, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.RemoverTodos(It.IsAny<List<InformeComercialAlmacenamiento>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<InformeComercial, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Remover(It.IsAny<InformeComercial>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.NotNull(result);
            Assert.IsFalse(result.HayError);
        }
        [Test]
        public void RespuestaDeSapCapacidadProductivaTestOk()
        {
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, int>>>()))
                .Returns(1);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<Expression<Func<Material, CampaniaMaterialSAP>>>()))
                .Returns(new CampaniaMaterialSAP { MaterialId = 1, CampaniaId = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<InformeComercialProduccion, bool>>>()))
                .Returns(new InformeComercialProduccion
                {
                    MaterialId = 1,
                    Material = new Material(),
                    Alquilado = true,
                    Hectareas = 1000,
                    InformeComercial = new InformeComercial(),
                    InformeComercialId = 1,
                    InformeComerciaProduccionId = 1,
                    Localidad = new Localidad(),
                    LocalidadId = 1,
                    MensajeSap = "a",
                    Propio = true,
                    RtaOkSap = true,
                    Toneladas = 100
                });

            var result = target.RespuestaDeSapCapacidadProductiva("a", "a", "a");

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, int>>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<Expression<Func<Material, CampaniaMaterialSAP>>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<InformeComercialProduccion, bool>>>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.NotNull(result);
            Assert.IsFalse(result.HayError);
        }
    }
}
