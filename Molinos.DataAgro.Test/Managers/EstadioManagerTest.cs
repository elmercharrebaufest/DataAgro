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
using System.Web;
using System.Web.Script.Serialization;

namespace Molinos.DataAgro.Test.Managers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class EstadioManagerTest
    {
        private EstadioManager target;
        private Mock<IRepositorio> repositorioMock;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            target = new EstadioManager(repositorioMock.Object);
        }

        [Test]
        public void TraerTodoEstadioPorMaterialTest()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Estadio, EstadioDto>>>(), It.IsAny<Expression<Func<Estadio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()))
                            .Returns(new List<EstadioDto>() { new EstadioDto { Id = 1, Descripcion="1" } });
            var result = target.TraerTodoEstadioPorMaterial(1);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<Estadio, EstadioDto>>>(), It.IsAny<Expression<Func<Estadio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);
            Assert.NotNull(result);
            Assert.AreEqual(1, result.Count);
        }
    }
}
