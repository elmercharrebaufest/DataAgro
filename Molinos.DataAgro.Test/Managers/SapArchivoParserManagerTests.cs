using Molinos.DataAgro.Business.Managers;
using Molinos.DataAgro.Repository;
using Moq;
using NLog;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Molinos.DataAgro.Test.Managers
{
    [TestFixture]
    public class SapArchivoParserManagerTests
    {
        private SapArchivoParserManager target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ILogger> loggerMock;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            loggerMock = new Mock<ILogger>();
            target = new SapArchivoParserManager(repositorioMock.Object, loggerMock.Object);
        }

        [Test]
        public void ParsearArchivo_TsvUtf16Le_FiltraContratosYCalculaCcpp()
        {
            var contenido = string.Join("\n", new[]
            {
                "Cabecera sin uso",
                "Tipo	Descripción Centro	Descripción del Material	Kilos a recibir total	CUIT Proveedor	Descripción Cl.Contrato	Cosecha	Fecha Desde	Fecha Hasta	Fecha	Numero	Descripción Proveedor	Descripción Corredor	Clasificación	Sust.	EPA	Valor	Moneda",
                "CCPP	Planta San Lorenzo	Soja Poroto	60.000	20-12345678-9		25-26										0	USD",
                "CTO	Planta San Lorenzo	Soja Poroto	900.000	20-12345678-9	Fijo	25-26	01.01.2026	30.06.2026	15.01.2026	0004500012345	AGRO S.A.		ACOPIADOR			450000	USD",
                "CTO	Planta San Lorenzo	Maíz	300.000	20-99999999-9	MP-Venta granos	25-26	01.01.2026	30.06.2026	15.01.2026	0004500019999	DESCARTAR		PRODUCTOR			90000	USD",
                "CTO	Otra Planta	Soja Poroto	300.000	20-00000000-0	Fijo	25-26	01.01.2026	30.06.2026	15.01.2026	0004500018888	OTRA		ACOPIADOR			120000	USD",
                "CTO	Planta San Lorenzo	Soja Poroto	300.000	20-12345678-9	Fijo	22-23	01.01.2026	30.06.2026	15.01.2026	0004500017777	COSECHA INVALIDA		ACOPIADOR			120000	USD"
            });

            var bytes = Encoding.Unicode.GetPreamble().Concat(Encoding.Unicode.GetBytes(contenido)).ToArray();
            using (var stream = new MemoryStream(bytes))
            {
                var resultado = target.ParsearArchivo(stream, ".tsv", new DateTime(2026, 6, 11));

                Assert.That(resultado.Contratos.Count, Is.EqualTo(1));
                Assert.That(resultado.Contratos[0].NumeroSAP, Is.EqualTo("4500012345"));
                Assert.That(resultado.Contratos[0].OpType, Is.EqualTo("acopiador"));
                Assert.That(resultado.Contratos[0].Proveedor, Is.EqualTo("AGRO S.A."));
                Assert.That(resultado.CcppByCuitMat["20-12345678-9|Soja Poroto"], Is.EqualTo(60000m));
                Assert.That(resultado.CcppByMat["Soja Poroto"], Is.EqualTo(60000m));
                Assert.That(resultado.Materiales, Has.Member("Soja Poroto"));
            }
        }

        [Test]
        public void ParsearArchivo_SinColumnaTipo_LanzaInvalidOperationException()
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes("Descripción Centro\tDescripción\nPlanta San Lorenzo\tDato")))
            {
                var ex = Assert.Throws<InvalidOperationException>(() => target.ParsearArchivo(stream, ".tsv", DateTime.Today));
                Assert.That(ex.Message, Does.Contain("Tipo"));
            }
        }
    }
}
