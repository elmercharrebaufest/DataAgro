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
    public class CartaDePresentacionManagerTest
    {
        private CartaDePresentacionManager target;
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

            target = new CartaDePresentacionManager(logger.Object, repositorioMock.Object);
        }

        [Test]
        public void GenerarCartaDePresentacionTestOk()
        {
            RptCartaDePresentacionInfo oParam = new RptCartaDePresentacionInfo();
            List<NuevoProduccion> nuevosCampos = new List<NuevoProduccion>();
            List<NuevoAcopio> nuevosAcopios = new List<NuevoAcopio>();
            oParam.corredorBolsa = "corre bolsa";
            oParam.corredorCuit = "c2030905640";
            oParam.corredorNroRegistro = "nroregistros";
            oParam.corredorRazonSocial = " corredor razon social s.a. algo mas larga";
            oParam.vendedorActividad = "productor";
            oParam.vendedorAntecedentesComerciales = "sin antecedentes penales XD";
            oParam.vendedorAntiguedadEnActividad = "30 años";
            oParam.vendedorCosecha = "10-11";
            oParam.vendedorCuit = "v30905640";
            oParam.vendedorDomicilioFiscal = "vendedor domiciio fiscal 43434";
            oParam.vendedorDomicilioReal = "vendedor domicilio real 44232111";
            oParam.vendedorMailContacto = "martin__eugenio__adrian@hotmail.com.ar";
            oParam.vendedorRazonSocial = "vendedor razon social sa pepeito";
            oParam.vendedorTelefonoContacto = "+51 011 9 4324-4567";
            nuevosAcopios = new List<NuevoAcopio>();
            nuevosAcopios.Add(new NuevoAcopio { ArrendaPropia = false, CampañaId = 1, LocalidadId = 1, Toneladas = 123213 });
            nuevosCampos = new List<NuevoProduccion>();
            nuevosCampos.Add(new NuevoProduccion { ArrendaPropia = false, CampañaId = 1, LocalidadId = 1, Toneladas = 123213, Hectareas = 132121, MaterialId = 1 });


            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<Material>() { new Material { MaterialId = 1, Descripcion = "1" } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Campaña, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                           .Returns(new List<Campaña>() { new Campaña { CampañaId = 1, Descripcion = "1" } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Localidad, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                           .Returns(new List<Localidad>() { new Localidad { LocalidadId = 1, Nombre = "1", Provincia = new Provincia { Nombre = "1", ProvinciaId = 1 } } });

            var result = target.GenerarCartaDePresentacion(oParam, nuevosCampos, nuevosAcopios);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Material, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Campaña, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Localidad, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);


            Assert.NotNull(result);
            Assert.AreEqual(result.CapAlmacenaje.Count, 1);
            Assert.AreEqual(result.CapProduccion.Count, 1);
        }

    }
}
