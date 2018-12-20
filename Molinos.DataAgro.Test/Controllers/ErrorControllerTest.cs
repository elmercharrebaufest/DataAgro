using NUnit.Framework;
using System.Web;
using WebDataAgro.Controllers;

namespace Molinos.DataAgro.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class ErrorControllerTest
    {
        private ErrorController target;

        [SetUp]
        public void SetUp()
        {
            HttpContext.Current = Mock.FakeContext.FakeHttpContext();
            HttpContext.Current.Session["perfil"] = 1;
            target = new ErrorController();
        }

        [Test]
        public void IndexTest()
        {
            var result = target.Index();

            Assert.IsNotNull(result);
        }
    }
}
