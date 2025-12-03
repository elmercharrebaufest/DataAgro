using NLog;
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
using System.Net.Mail;
using System.Web;
using System.Web.Script.Serialization;

namespace Molinos.DataAgro.Test.Managers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class MailCompraManagerTest
    {
        private MailManager target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ILogger> loggerMock;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            loggerMock = new Mock<ILogger>();
            target = new MailManager(loggerMock.Object, repositorioMock.Object);
        }

        [Test]
        public void EnviarMailTest()
        {
            ConfigurationManager.AppSettings["CredentialUserName"] = "dataagro.baufest@gmail.com";
            ConfigurationManager.AppSettings["SmtpServerPort"] = "587";
            ConfigurationManager.AppSettings["SmtpServer"] = "smtp.gmail.com";
            ConfigurationManager.AppSettings["UseDefaultCredentials"] = "S";
            ConfigurationManager.AppSettings["EnableSSL"] = "S";
            ConfigurationManager.AppSettings["CredentialPassword"] = "Hola1234";
            Comercial desde = new Comercial { ComercialId = 1, IdActiveDirectory = "emartin1", Apellido = "a", Nombres = "n" };
            List<Comercial> enviarA = new List<Comercial> { desde };
            string asunto = "prueba";
            string cuerpo = "prueba";
            List<Comercial> copia = new List<Comercial> { desde };
            AlternateView vistaAlternativa = null;
            byte[] archivo = null;
            string nombreArchivo = null;
            try
            {
                target.EnviarMail(desde, enviarA, asunto, cuerpo, copia, vistaAlternativa, archivo, nombreArchivo);
            }
            catch (Exception)
            {
                Assert.IsFalse(true);
            }
            finally
            {
                Assert.IsTrue(true);
            }
        }
    }
}
