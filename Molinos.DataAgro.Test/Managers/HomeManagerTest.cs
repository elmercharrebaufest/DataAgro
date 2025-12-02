using NLog;
using Molinos.DataAgro.Business.Managers;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using Molinos.DataAgro.Test.Mock;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading;

namespace Molinos.DataAgro.Test.Managers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class HomeManagerTest
    {
        private HomeManager target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ICampañaManager> campanaMock;
        private Mock<IObjetivoManager> objetivoMock;
        private Mock<ILogger> logMock;
        private Mock<IProveedorManager> proveedorMock;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            logMock = new Mock<ILogger>();
            campanaMock = new Mock<ICampañaManager>();
            objetivoMock = new Mock<IObjetivoManager>();
            proveedorMock = new Mock<IProveedorManager>();
            target = new HomeManager(logMock.Object, repositorioMock.Object, campanaMock.Object, objetivoMock.Object);
        }

        [Test]
        public void TraerBusquedaContactoSinPermiso()
        {
            var param = new oParamBusqueda()
            {
                Material = "",
                Campaña = "",
                Actividad = "",
                Segmentacion = "",
                Calificacion = "",
                Equipo = new List<int> { 1, 2, 3},
                Hectareas = "",
                Estado = "",
                Comercial = "",
                Zona = "",
                Condicion = "",
                Toneladas = "",
                ComercialId = 1
            };
            var contacto = new List<Contactos>() { new Contactos
            {
               Calificacion = 1,
               Email1 = "bmelgarejo",
               ComercialAcargo = "",
               CUIT = "2344",
               ProveedorId = 1,
               RazonSocial = "Parisi",
               FechaUltimoContacto = DateTime.Now,
               Estado = "3",
               EstadoCuit = 1,
               FechaAlta = DateTime.Now,
               GrupoDeCompras = "",
               Segmentacion = 5,      
               Condicion = "",
               RiesgoComercialSap = "No",
               Situacion = ""
            }};
            Thread.CurrentPrincipal = new TestPrincipal(new Claim[] {
            new Claim(ClaimTypes.Role, PermisosDataAgro.IngresoExterno.ToString()) });
            repositorioMock.Setup(x => x.SelStorePaginado<Contactos>(It.IsAny<String>(), It.IsAny<int>(), It.IsAny<int>(),
              It.IsAny<String>(), It.IsAny<String>(), It.IsAny<String>(), It.IsAny<String>(), It.IsAny<String>(), It.IsAny<String>(),
              It.IsAny<String>(), It.IsAny<String>(), It.IsAny<String>(), It.IsAny<String>(), It.IsAny<String>(), It.IsAny<String>())).Returns(contacto);

            repositorioMock.Setup(x => x.SelStore<Contactos>(It.IsAny<String>(), It.IsAny<int>(),
             It.IsAny<String>(), It.IsAny<String>(), It.IsAny<String>(), It.IsAny<String>(), It.IsAny<String>(), It.IsAny<String>(),
             It.IsAny<String>(), It.IsAny<String>(), It.IsAny<String>(), It.IsAny<String>(), It.IsAny<String>(), It.IsAny<String>())).Returns(contacto);

            var resultado = target.TraerBusquedaContacto(param, It.IsAny<int>(), param.Equipo);            
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }


        [Test]
        public void TraerBusquedaContactoConPermiso()
        {
            var param = new oParamBusqueda()
            {
                Material = "",
                Campaña = "",
                Actividad = "",
                Segmentacion = "",
                Calificacion = "",
                Equipo = new List<int> { 1, 2, 3 },
                Hectareas = "",
                Estado = "",
                Comercial = "",
                Zona = "",
                Condicion = "",
                Toneladas = "",
                ComercialId = 1
            };
            var contacto = new List<Contactos>() { new Contactos
            {
               Calificacion = 1,
               Email1 = "bmelgarejo",
               ComercialAcargo = "",
               CUIT = "2344",
               ProveedorId = 1,
               RazonSocial = "Parisi",
               FechaUltimoContacto = DateTime.Now,
               Estado = "3",
               EstadoCuit = 1,
               FechaAlta = DateTime.Now,
               GrupoDeCompras = "",
               Segmentacion = 5,
               Condicion = "",
               RiesgoComercialSap = "No",
               Situacion = ""
            }};
            Thread.CurrentPrincipal = new TestPrincipal(new Claim[] {
            new Claim(ClaimTypes.Role, PermisosDataAgro.VerCorredorComercial.ToString())
            });

            repositorioMock.Setup(y => y.ListarConsulta(It.IsAny<TraerCorredoresComercial>())).Returns(contacto);            
            var resultado = target.TraerBusquedaContacto(param, It.IsAny<int>(), param.Equipo);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }

        [Test]
        public void TraerInfoCampañaTest()
        {
           
            campanaMock.Setup(x => x.TraerCampañaHome(It.IsAny<int>(), new List<int>())).Returns(new CampañaHome());
            var resultado = target.TraerInfoCampaña(It.IsAny<int>(), new List<int>());
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }

        [Test]
        public void TraerInfoObjetivoTest()
        {

            objetivoMock.Setup(x => x.TraerObjetivoHome(It.IsAny<int>(), new List<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(new ObjetivoHome());
            var resultado = target.TraerInfoObjetivo(It.IsAny<int>(), new List<int>(), It.IsAny<int>(), It.IsAny<int>());
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }
        [Test]
        public void TraerIdComercialTest()
        {

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<Expression<Func<Comercial, int>>>())).Returns(1);
            var resultado = target.TraerIdComercial(It.IsAny<string>());
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }

        [Test]
        public void BusquedaHomeTest()
        {
            repositorioMock.Setup(y => y.ListarConsulta(It.IsAny<ConsultaBusquedaHome>())).Returns(new List<BusquedaHome>());
            var resultado = target.BusquedaHome(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<List<int>>(), It.IsAny<List<int>>());
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }
        [Test]
        public void TraerActividadesPorComercialIdTest()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Actividad, ActividadRecordatorioGrid>>>(), It.IsAny<Expression<Func<Actividad, bool>>>(),
                It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
                .Returns(new List<ActividadRecordatorioGrid>() { new ActividadRecordatorioGrid {
                    ActividadId = 1,
                    Comentarios = "",
                    Contacto = "",
                    FechaRecordatorio = DateTime.Now,
                    ProveedorId = 1,
                    Tema = "",
                }
             });
            repositorioMock.Setup(y => y.ListarConsulta(It.IsAny<ConsultarActividadResearch>())).Returns(new List<ActividadRecordatorio>());
            var resultado = target.TraerActividadesPorComercialId(It.IsAny<int>());
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }
        [Test]
        public void ListarTodosLosComercialesConMismaZonaTest()
        {
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<Expression<Func<Comercial, int>>>())).Returns(1);
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Comercial, int>>>(), It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), 
                It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<int>());
            var resultado = target.ListarTodosLosComercialesConMismaZona(It.IsAny<int>());
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);

        }
        [Test]
        public void TraerTodoCompraDetalleTest()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<int>(), It.IsAny<string>(),
                  It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<Material>() { new Material { MaterialId = 1, CampañaId = 1,Campaña = new Campaña {Descripcion="20-21" } } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<int>(), It.IsAny<string>(),
               It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<Proveedor>() { new Proveedor { ProveedorId = 1 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampanaMaterialDetallePorMes, bool>>>(), It.IsAny<int>(), It.IsAny<string>(),
                 It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<CampanaMaterialDetallePorMes>() {
                     new CampanaMaterialDetallePorMes { ComercialId = 1,
                         
                         Material = new Material{ Descripcion = "Soja"},
                             Proveedor = new Proveedor { RazonSocial = "Parisi", CUIT = "1234"},
                             Campana = new Campaña {Descripcion = "20-21"},
                             MaterialId = 1,
                             CampanaId = 1,
                             ProveedorId = 1,
                           ClaseDoc = "ZPAF",
                           Clasificacion = "ACOPIADOR",
                           Contrato = "2345",
                           Fecha =  DateTime.Now,
                           PendienteAFijar = 10,
                           PendienteAplicar = 1,
                           ToneladaAmpliada = 1,
                           ToneladaAnulada = 2,
                           ToneladaAplicada = 0,
                           ToneladaContrato = 1000,
                           CorredorCuit = "23443"
                     }
                 });
            var equipo = new List<int> { 1, 2, 3 };
            var resultado = target.TraerTodoCompraDetalle(It.IsAny<List<int>>(), It.IsAny<int>(), It.IsAny<int>());
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }
        [Test]
        public void TraerTodoCompraDetalleExcelTest()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<int>(), It.IsAny<string>(),
                 It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<Proveedor>() { new Proveedor { ProveedorId = 1} });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(),
                 It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<Comercial>() { new Comercial { ComercialId = 1 } });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampanaMaterialDetallePorMes, CampanaMaterialDetallePorMesDto>>>(), It.IsAny<Expression<Func<CampanaMaterialDetallePorMes, bool>>>(),
               It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
               .Returns(new List<CampanaMaterialDetallePorMesDto>() { new CampanaMaterialDetallePorMesDto {
                   ClaseDoc = "ZPAF",
                   Clasificacion = "ACOPIADOR",
                   ComercialId = 1,
                   Contrato = "2345",
                   Fecha =  DateTime.Now,
                   MaterialId = 1,
                   Material = "Soja",
                   PendienteAFijar = 10,
                   PendienteAplicar = 1,
                   ToneladaAmpliada = 1,
                   ToneladaAnulada = 2,
                   ToneladaAplicada = 0,
                   ToneladaContrato = 1000,
                   CorredorCuit = "23443",
                   ProveedorId = 1,
                   ToneladaFijada = 0,
                   CampanaId = 1,
                   Campana = "20-21",
                   Proveedor = "Parisi",
                   CUIT = "234533"
               }
             });

            var resultado = target.TraerTodoCompraDetalleExcel(It.IsAny<List<int>>());
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }

        [Test]
        public void TraerTodoCompraCampanaActualTest()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<int>(), It.IsAny<string>(),
                 It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<Proveedor>() { new Proveedor { ProveedorId = 1 } });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Comercial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(),
                 It.IsAny<Entities.Helpers.DirOrden>())).Returns(new List<Comercial>() { new Comercial { ComercialId = 1 } });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CampanaMaterialDetallePorMes, CompraCampanaActualDto>>>(), It.IsAny<Expression<Func<CampanaMaterialDetallePorMes, bool>>>(),
               It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Entities.Helpers.DirOrden>()))
               .Returns(new List<CompraCampanaActualDto>(){ new CompraCampanaActualDto {
                   ClaseDoc = "ZPAF",
                   Clasificacion = "ACOPIADOR",
                   Contrato = "2345",
                   Fecha =  DateTime.Now,
                   Material = "Soja",
                   PendienteAFijar = 10,
                   ToneladaAmpliada = 1,
                   ToneladaAnulada = 2,
                   ToneladaAplicada = 0,
                   ToneladaContrato = 1000,
                   CorredorCuit = "23443",
                   ToneladaFijada = 0,
                   Campana = "20-21",
                  }

               });
            var resultado = target.TraerTodoCompraCampanaActual(It.IsAny<List<int>>());
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }

    }    
}