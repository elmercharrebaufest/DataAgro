using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebDataAgro.Controllers;
using WebDataAgro.Services;

namespace Molinos.DataAgro.Test.Services
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class DataAgroServicesTest
    {
        private DataAgroServices target;
        private Mock<ILogger> loggerMock;
        private Mock<IRiesgoComercialManager> riesgoComercialManagerMock;
        private Mock<ICampaniaActualManager> campaniaAcutalManagerMock;
        private Mock<ICampaniaMaterialManager> camaniaMaterialManagerMock;
        private Mock<IInformeComercialManager> informeComercialManagerMock;
        private Mock<IContratoManager> contratoManagerMock;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ICupoManager> cupoManagerMock;
        private Mock<IMailManager> mailManagerMock;
        private Mock<IFijacionDePrecioContratoManager> fijacionManager;


        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new JavaScriptSerializer();
            loggerMock = new Mock<ILogger>();
            riesgoComercialManagerMock = new Mock<IRiesgoComercialManager>();
            campaniaAcutalManagerMock = new Mock<ICampaniaActualManager>();
            camaniaMaterialManagerMock = new Mock<ICampaniaMaterialManager>();
            informeComercialManagerMock = new Mock<IInformeComercialManager>();
            contratoManagerMock = new Mock<IContratoManager>();
            repositorioMock = new Mock<IRepositorio>();
            cupoManagerMock = new Mock<ICupoManager>();
            mailManagerMock = new Mock<IMailManager>();
            fijacionManager = new Mock<IFijacionDePrecioContratoManager>();

            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            target = new DataAgroServices(loggerMock.Object, riesgoComercialManagerMock.Object, campaniaAcutalManagerMock.Object,
                camaniaMaterialManagerMock.Object, informeComercialManagerMock.Object, contratoManagerMock.Object, repositorioMock.Object, cupoManagerMock.Object
                ,mailManagerMock.Object, fijacionManager.Object);

            HttpContext.Current.Session["perfil"] = 1;
            HttpContext.Current.Session["comercialId"] = 1;

        }

        [Test]
        public void PingOk()
        {
            var result = target.Ping() as ResultadoSap;
            Assert.NotNull(result);
            Assert.IsFalse(result.HayError);
        }
        [Test]
        public void GrabarRiesgoComercialOk()
        {
            RiesgoComercial riesgoComercial = new RiesgoComercial();
            riesgoComercialManagerMock.Setup(x => x.ActualizacionDeRiesgoComercial(It.IsAny<RiesgoComercial>()))
                .Returns(new Resultado());

            var result = target.Ping() as ResultadoSap;
            Assert.NotNull(result);
            Assert.IsFalse(result.HayError);
            Assert.AreEqual(result.ListaErrores.Count(), 0);


        }
        [Test]
        public void GrabarRiesgoComercialError()
        {
            RiesgoComercial riesgoComercial = new RiesgoComercial();
            riesgoComercialManagerMock.Setup(x => x.ActualizacionDeRiesgoComercial(It.IsAny<RiesgoComercial>()))
                .Returns(new Resultado { Errores = new List<ErrorMessage> { new ErrorMessage { Source = "", Message = "" } } });

            var result = target.GrabarRiesgoComercial(riesgoComercial) as ResultadoSap;
            Assert.NotNull(result);
            Assert.IsTrue(result.HayError);
            Assert.AreEqual(result.ListaErrores.Count, 1);

        }
    }
}
