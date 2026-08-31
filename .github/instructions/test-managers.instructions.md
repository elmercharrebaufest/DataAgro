---
description: "Convención de tests unitarios de Managers en Molinos.DataAgro.Test: NUnit + Moq. Use when: escribir o modificar un {X}ManagerTest.cs, mockear IRepositorio/ILogger, o agregar cobertura de test a un Manager."
applyTo: "Molinos.DataAgro.Test/**/*.cs"
---

# Tests de Managers (NUnit + Moq)

- Framework: NUnit (`[TestFixture]`, `[SetUp]`, `[Test]`). Mocking: Moq.
- Un archivo `{X}ManagerTest.cs` por Manager, en `Molinos.DataAgro.Test/Managers/`.
- Patrón de `SetUp`: mockear `IRepositorio` y `ILogger` (NLog) siempre, más un `Mock<I{Dependencia}Manager>` (u otra interfaz) por cada dependencia adicional que reciba el constructor del Manager bajo test:
  ```csharp
  [TestFixture]
  public class {X}ManagerTest
  {
      private {X}Manager target;
      private Mock<IRepositorio> repositorioMock;
      private Mock<ILogger> logger;

      [SetUp]
      public void SetUp()
      {
          logger = new Mock<ILogger>();
          repositorioMock = new Mock<IRepositorio>();
          target = new {X}Manager(logger.Object, repositorioMock.Object /*, ... otras dependencias mockeadas */);
      }

      [Test]
      public void {Escenario}()
      {
          // Arrange: repositorioMock.Setup(...)
          // Act: var resultado = target.{Metodo}(...);
          // Assert
      }
  }
  ```
- Si el Manager depende de `ConfigurationManager.AppSettings` (ej. SMTP, credenciales), setearlos explícitamente en el `SetUp` antes de instanciar el target (ver `ReportesManagerTest`).
- Helpers de test ya existentes: `Molinos.DataAgro.Test/Mock/` (`ClaimsPrincipal.cs`, `FakeHttpContext.cs` para simular contexto HTTP/usuario) y `Molinos.DataAgro.Test/Util/` (assets de test, ej. imágenes usadas por reportes).
- La cobertura es parcial: no todos los Managers tienen test. No es obligatorio agregar test salvo que el usuario lo pida.
