using Autofac.Extras.NLog;
using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
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
    public class CupoManagerTest
    {
        private CupoManager target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ILogger> logger;
        private Mock<ICrearCupoAgent> crearCupoAgentMock;
        private Mock<IEliminarCupoAgent> eliminarCupoAgentMock;
        private Mock<IClienteStopAgent> clienteStopMock;
        private Mock<IModificarCupoAgent> modificarCupoAgentMock;
        private Mock<IProveedorManager> proveedorManagerMock;
        [SetUp]
        public void SetUp()
        {
            logger = new Mock<ILogger>();
            repositorioMock = new Mock<IRepositorio>();
            crearCupoAgentMock = new Mock<ICrearCupoAgent>();
            eliminarCupoAgentMock = new Mock<IEliminarCupoAgent>();
            clienteStopMock = new Mock<IClienteStopAgent>();
            modificarCupoAgentMock = new Mock<IModificarCupoAgent>();
            proveedorManagerMock = new Mock<IProveedorManager>();
            target = new CupoManager(repositorioMock.Object, logger.Object, crearCupoAgentMock.Object,
                eliminarCupoAgentMock.Object, clienteStopMock.Object, modificarCupoAgentMock.Object, proveedorManagerMock.Object);
            repositorioMock.Setup(x => x.Obtener<Configuracion>(1)).Returns(new Configuracion { ConexionABMStop = true });
        }

        [Test]
        public void GrabarCupoTestOk()
        {
            var cupo = new Cupo
            {
                Id=0,
                ProveedorId = 1,
                MaterialId = 1,
                CentroId = 1,
                ZonaCupoId = 1,
                FechaIngreso = new DateTime(2099,11,19)
            };
            
            repositorioMock.Setup(y => y.Obtener<Proveedor>(It.IsAny<int>()))
                .Returns(new Proveedor { ProveedorId = 1 });
            repositorioMock.Setup(y => y.Obtener<Material>(It.IsAny<int>()))
                .Returns(new Material { MaterialId = 1 });
            repositorioMock.Setup(y => y.Obtener<Centro>(It.IsAny<int>()))
                .Returns(new Centro { Id = 1 });
            repositorioMock.Setup(y => y.Obtener<ZonaCupo>(It.IsAny<int>()))
                .Returns(new ZonaCupo { Id = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<LimiteCupo, bool>>>())).Returns(new LimiteCupo {ConfiguracionCupoId=1,ZonaCupoId=1,CantidadCupo=1 });
            repositorioMock.Setup(y => y.Contar(It.IsAny<Expression<Func<Cupo, bool>>>())).Returns(0);

            crearCupoAgentMock.Setup(x => x.Crear(It.IsAny<Cupo>(), It.IsAny<int>()))
                .Returns(new List<string>() { "a" });
            repositorioMock.Setup(x => x.Agregar(It.IsAny<Cupo>()));
            var result = target.GrabarCupo(cupo, new List<DiaCupo>() {
                new DiaCupo { Cantidad=1, Fecha = new DateTime(2099,11,10)},
                new DiaCupo { Fecha= new DateTime(2099,11,10),Cantidad=0},
            new DiaCupo { Fecha= new DateTime(2099,11,10),Cantidad=1}});
           
            repositorioMock.Verify(x => x.Obtener<Proveedor>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<Material>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<Centro>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<ZonaCupo>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.AgregarTodos(It.IsAny<List<Cupo>>(),null), Times.Exactly(2));
            crearCupoAgentMock.Verify(x => x.Crear(It.IsAny<Cupo>(), It.IsAny<int>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(2));
            Assert.NotNull(result);
            Assert.IsFalse(result.HayError);
        }

        [Test]
        public void ModificarCupoTestOk()
        {
            var cupo = new Cupo
            {
                Id = 1,
                ProveedorId = 1,
                MaterialId = 1,
                CentroId = 1,
                Centro = new Centro { Acopio = false},
                ZonaCupoId = 1,
                CupoStop = 1,
                CupoSap="aa",
                FechaIngreso = new DateTime(2019, 11, 19)
            };

            repositorioMock.Setup(y => y.Obtener<Proveedor>(It.IsAny<int>()))
                .Returns(new Proveedor { ProveedorId = 1 });
            repositorioMock.Setup(y => y.Obtener<Material>(It.IsAny<int>()))
                .Returns(new Material { MaterialId = 1 });
            repositorioMock.Setup(y => y.Obtener<Centro>(It.IsAny<int>()))
                .Returns(new Centro { Id = 1 });
            repositorioMock.Setup(y => y.Obtener<ZonaCupo>(It.IsAny<int>()))
                .Returns(new ZonaCupo { Id = 1 });
            repositorioMock.Setup(y => y.Obtener<Configuracion>(It.IsAny<int>())).Returns(new Configuracion{ ConexionABMStop = true});
            repositorioMock.Setup(y => y.Obtener<Cupo>(It.IsAny<int>())).Returns(cupo);

            modificarCupoAgentMock.Setup(x => x.Modificar(It.IsAny<Cupo>()))
                .Returns("Ok");

            repositorioMock.Setup(x => x.Agregar(It.IsAny<Cupo>()));
            var result = target.GrabarCupo(cupo, new List<DiaCupo>());

            repositorioMock.Verify(x => x.Obtener<Proveedor>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<Material>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<Centro>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<ZonaCupo>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<Configuracion>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<Cupo>(It.IsAny<int>()), Times.Once);
            modificarCupoAgentMock.Verify(x => x.Modificar(It.IsAny<Cupo>()), Times.Once);
            clienteStopMock.Verify(x => x.ModificarCupo(It.IsAny<Cupo>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            Assert.NotNull(result);
            Assert.IsFalse(result.HayError);
        }

        [Test]
        public void EliminarCupoTest()
        {
            repositorioMock.Setup(x => x.Obtener<Cupo>(It.IsAny<int>())).Returns(new Cupo {CupoStop=1, CupoSap = "a", Centro = new Centro { Acopio= false} });
            eliminarCupoAgentMock.Setup(x => x.Eliminar(It.IsAny<string>(), It.IsAny<string>())).Returns("OK");
            clienteStopMock.Setup(x => x.EliminarCupo(It.IsAny<Cupo>())).Returns(new Resultado { Errores = new List<ErrorMessage>() });
            repositorioMock.Setup(x => x.GuardarCambios());

            var result = target.EliminarCupo(1, "a");
            repositorioMock.Verify(x => x.Obtener<Cupo>(It.IsAny<int>()), Times.Once);
            clienteStopMock.Verify(x => x.EliminarCupo(It.IsAny<Cupo>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            eliminarCupoAgentMock.Verify(x => x.Eliminar(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            Assert.IsFalse(result.HayError);
        }

        [Test]
        public void ValidarCupoTestError()
        {
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor,bool>>>(), It.IsAny<Expression<Func<Proveedor, string>>>()))
                .Returns("a");
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<SISA, bool>>>()))
                .Returns(new SISA());
            var result = target.Validar(new Cupo() { Fason = true, MaterialId = 3, ProveedorId = 1 }, 0,null);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>(), It.IsAny<Expression<Func<Proveedor, string>>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<SISA, bool>>>()), Times.Once);
            
            Assert.NotNull(result);
            Assert.IsTrue(result.HayError);
            Assert.AreEqual(6, result.Errores.Count);
        }
        [Test]
        public void TraerCuposTablaTestOk()
        {
            repositorioMock.Setup(x => x.ObtenerConsultaEscalar(It.IsAny<TraerTodosCupos>())).Returns(
                new KendoGrid<CupoDto>(new List<CupoDto>() { new CupoDto { Acopio= false,Calidad="a",Centro="a",CentroId=1,Comercial="a",ComercialId=1,CupoSap="a",CupoStop="a",Destinatario="a",EstadoCupo="a", EstadoCupoId=1,Fason=false,
                FechaGeneracion = new DateTime(2019,1,1),FechaIngreso=new DateTime(2019,1,1),FleteProcedencia=false,HoraIngreso=new DateTime(2019,1,1),Id=1,
                Material="a",MaterialId=1,MensajeError="",Observaciones="a",Proveedor="a",ProveedorId=1,ZonaCupo="a",ZonaCupoId=1} }, 1));

            var result = target.TraerCuposTabla(new KendoGridMvcRequest(), new List<int>() { 1,2});

            Assert.NotNull(result);
            repositorioMock.Verify(x => x.ObtenerConsultaEscalar(It.IsAny<TraerTodosCupos>()), Times.Once);
        }
        [Test]
        public void EliminarCupoTestOk()
        {
            repositorioMock.Setup(y => y.Obtener<Cupo>(It.IsAny<int>()))
                .Returns(new Cupo() { CupoStop=1, CupoSap="a",Centro=new Centro { Acopio = false } });
            repositorioMock.Setup(y => y.Obtener<Configuracion>(It.IsAny<int>())).Returns(new Configuracion{ ConexionABMStop = true});
            eliminarCupoAgentMock.Setup(y => y.Eliminar(It.IsAny<string>(), It.IsAny<string>())).Returns("OK");
            clienteStopMock.Setup(y => y.EliminarCupo(It.IsAny<Cupo>())).Returns(new Resultado() { Errores = new List<ErrorMessage>()});
            var result = target.EliminarCupo(1,"a");

            repositorioMock.Verify(x => x.Obtener<Cupo>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<Configuracion>(It.IsAny<int>()), Times.Once);
            eliminarCupoAgentMock.Verify(y => y.Eliminar(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            clienteStopMock.Verify(y => y.EliminarCupo(It.IsAny<Cupo>()), Times.Once);

            Assert.NotNull(result);
            Assert.IsFalse(result.HayError);
            
        }
        [Test]
        public void TransmitirCuposTest()
        {
            repositorioMock.Setup(y => y.Obtener<Configuracion>(It.IsAny<int>())).Returns(new Configuracion{ ConexionABMStop = true});
            eliminarCupoAgentMock.Setup(x => x.Eliminar(It.IsAny<string>(), It.IsAny<string>())).Returns("OK");

            var result = target.TransmitirCupos(new List<string>());
            repositorioMock.Verify(x => x.Obtener<Configuracion>(It.IsAny<int>()), Times.Once);
            clienteStopMock.Verify(x => x.CrearCupo(It.IsAny<List<string>>()), Times.Once);
            Assert.IsFalse(result.HayError);
        }

        [Test]
        public void TransmitirJobCuposTest()
        {
            target.TransmitirCupos();
           
            clienteStopMock.Verify(x => x.TransmitirJobCupos(), Times.Once);
        }
        [Test]
        public void ConsultarCuposDiariosTest()
        {
            clienteStopMock.Setup(x => x.ConsultarCuposDiarios()).Returns(new List<RespuestaCupoStop>() { new RespuestaCupoStop { token="a",
            codGrano=1,codLocalidadDestino=1,
             creado="a",cuitCorredorC="a",cuitDestinatario="a",desvio="n",
            cuitDestino="a",estado="a",fecha="a",idCupo=1,idCupoEstado=1,idCupoTerminal="a",idTerminal=1 } });
            var result = target.ConsultarCuposDiarios();

            clienteStopMock.Verify(x => x.ConsultarCuposDiarios(), Times.Once);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }
        [Test]
        public void ObtenerCupoTest()
        {
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<Cupo,bool>>>(), It.IsAny<Expression<Func<Cupo, CupoDto>>>())).Returns(new CupoDto {
                Id = 1,
                Calidad = "a",
                Centro = "a",
                CentroId = 1,
                ComercialId = 1,
                Destinatario = "a",
                Fason = true,
                FleteProcedencia = true,
                FechaIngreso = DateTime.Now,
                MaterialId = 1,
                Material = "a",
                Observaciones = "a",
                ProveedorId = 1,
                Proveedor = "a",
                ZonaCupoId = 2,
                ZonaCupo = "a"
            });
            var result = target.ObtenerCupo(1);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Cupo, bool>>>(), It.IsAny<Expression<Func<Cupo, CupoDto>>>()), Times.Once);
            Assert.IsNotNull(result);
        }
        [Test]
        public void EliminarVariosCupoTestOk()
        {
            repositorioMock.Setup(y => y.Obtener<Cupo>(It.IsAny<int>()))
                .Returns(new Cupo() { CupoStop = 1, CupoSap = "a", Centro = new Centro { Acopio = false } });
            repositorioMock.Setup(y => y.Obtener<Configuracion>(It.IsAny<int>())).Returns(new Configuracion { ConexionABMStop = true });
            eliminarCupoAgentMock.Setup(y => y.Eliminar(It.IsAny<string>(), It.IsAny<string>())).Returns("OK");
            clienteStopMock.Setup(y => y.EliminarCupo(It.IsAny<Cupo>())).Returns(new Resultado() { Errores = new List<ErrorMessage>() });
            var result = target.EliminarVarios(new List<int>() { 1,2}, "a");

            repositorioMock.Verify(x => x.Obtener<Cupo>(It.IsAny<int>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.Obtener<Configuracion>(It.IsAny<int>()), Times.Exactly(2));
            eliminarCupoAgentMock.Verify(y => y.Eliminar(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
            clienteStopMock.Verify(y => y.EliminarCupo(It.IsAny<Cupo>()), Times.Exactly(2));

            Assert.NotNull(result);
            Assert.IsFalse(result.HayError);
        }
        [Test]
        public void ObtenerCodigoSapTestOk()
        {
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Cupo,bool>>>(), It.IsAny<Expression<Func<Cupo, string>>>()))
                .Returns("a");
            var result = target.ObtenerCodigoSap(1);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Cupo, bool>>>(), It.IsAny<Expression<Func<Cupo, string>>>()), Times.Once);

            Assert.NotNull(result);
            Assert.AreEqual("a", result);
        }
    }
}