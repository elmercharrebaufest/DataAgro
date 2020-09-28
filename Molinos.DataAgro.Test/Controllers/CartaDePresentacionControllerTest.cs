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
    public class CartaDePresentacionControllerTest
    {
        private CartaDePresentacionController target;
        private Mock<ICartaDePresentacionManager> cartaDePresentacionManagerMock;
        private Mock<IReportesManager> reportesManagerMock;

        [SetUp]
        public void SetUp()
        {
            reportesManagerMock = new Mock<IReportesManager>();
            cartaDePresentacionManagerMock = new Mock<ICartaDePresentacionManager>();
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            HttpContext.Current.Session["perfil"] = 1;
            target = new CartaDePresentacionController(reportesManagerMock.Object, cartaDePresentacionManagerMock.Object);
        }




        [Test]
        public void ListarTest()
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
            nuevosAcopios.Add(new NuevoAcopio { ArrendaPropia = false, CampañaId = 5, LocalidadId = 3, Toneladas = 123213 });
            nuevosAcopios.Add(new NuevoAcopio { ArrendaPropia = true, CampañaId = 7, LocalidadId = 980, Toneladas = 9089 });
            nuevosAcopios.Add(new NuevoAcopio { ArrendaPropia = true, CampañaId = 7, LocalidadId = 980, Toneladas = 9089 });
            nuevosAcopios.Add(new NuevoAcopio { ArrendaPropia = true, CampañaId = 7, LocalidadId = 3457, Toneladas = 7565 });
            nuevosCampos = new List<NuevoProduccion>();
            nuevosCampos.Add(new NuevoProduccion { ArrendaPropia = false, CampañaId = 5, LocalidadId = 3, Toneladas = 123213, Hectareas = 132121, MaterialId = 1 });
            nuevosCampos.Add(new NuevoProduccion { ArrendaPropia = true, CampañaId = 7, LocalidadId = 980, Toneladas = 9089, Hectareas = 976, MaterialId = 2 });
            nuevosCampos.Add(new NuevoProduccion { ArrendaPropia = true, CampañaId = 7, LocalidadId = 980, Toneladas = 9089, Hectareas = 4436, MaterialId = 3 });
            nuevosCampos.Add(new NuevoProduccion { ArrendaPropia = true, CampañaId = 7, LocalidadId = 3457, Toneladas = 7565, Hectareas = 8765, MaterialId = 4 });

            cartaDePresentacionManagerMock.Setup(x => x.GenerarCartaDePresentacion(oParam, nuevosCampos, nuevosAcopios)).Returns(oParam);
            var result = target.Generar(oParam, nuevosCampos, nuevosAcopios);

            cartaDePresentacionManagerMock.Verify(x => x.GenerarCartaDePresentacion(It.IsAny<RptCartaDePresentacionInfo>(), It.IsAny<List<NuevoProduccion>>(), It.IsAny<List<NuevoAcopio>>()), Times.Once);

            var model = result.Result as JsonResult;
            Assert.NotNull(result);
            Assert.IsTrue(((ReportesModel)model.Data).DownloadKey != "");
        }


    }
}
