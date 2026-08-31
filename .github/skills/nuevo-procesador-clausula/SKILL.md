---
name: nuevo-procesador-clausula
description: 'Scaffolding de un nuevo procesador de cláusula contractual en el motor de cláusulas de DataAgro (Molinos.DataAgro.Business/Clausulas y ClausulasBoleto). Usar cuando pidan agregar una cláusula/procesador nuevo para boletos o contratos. No genera la lógica de negocio/contractual, solo la estructura.'
---

# Nuevo procesador de cláusula

## Contexto
El motor de cláusulas tiene dos sub-patrones activos (no confundir, y **nunca** tomar como referencia `Molinos.DataAgro.Business/ProcesamientoClausulasOld`, que es legacy sin uso):

- **Genérico** (`Molinos.DataAgro.Business/Clausulas`): cualquier tipo que herede de `Clausula` necesita una clase que implemente `IProcesadorClausula<TClausula>`. `ServicioClausulas` la descubre **automáticamente por reflection** (busca la clase que implementa `IProcesadorClausula<>` para ese tipo); no hay que registrarla en ningún lado.
- **Por categoría** (`Molinos.DataAgro.Business/ClausulasBoleto/{Genericos, BoletoFisico, CartaOferta, Confirma}`): cada categoría tiene su propia interfaz (`IProcesadorClausula{Categoria}`) y una clase base abstracta genérica (`ProcesadorClausula{Categoria}<T>`). Las implementaciones existentes están numeradas en palabras (`...Uno`, `...Dos`, ..., `...CuarentaYNueve`), correspondiendo cada número a una cláusula contractual real.

## Procedimiento

1. Confirmar con el usuario a qué categoría pertenece la cláusula nueva (Genéricos / BoletoFisico / CartaOferta / Confirma / genérico fuera de esas categorías).
2. Si hace falta un tipo de cláusula nuevo, crearlo en `Molinos.DataAgro.Entities` heredando de la clase base correspondiente (`Clausula` o la de la categoría).
3. Crear la clase procesadora siguiendo el patrón de la categoría elegida, por ejemplo:
   ```csharp
   public class ProcesadorClausula{Categoria}{NombreODescripcion} : ProcesadorClausula{Categoria}<Clausula{Categoria}{NombreODescripcion}>
   {
       public ProcesadorClausula{Categoria}{NombreODescripcion}(IRepositorio repositorio, ILogger log /*, dependencias necesarias */)
           : base(repositorio, log /*, ... */) { }

       public override ResultadoClausula DevolverClausulas(Clausula{Categoria}{NombreODescripcion} clausula)
       {
           // TODO: lógica de negocio/contractual específica de esta cláusula — a definir con el usuario/negocio.
           throw new NotImplementedException();
       }
   }
   ```
4. No hace falta registrar la clase en ningún diccionario ni contenedor DI: se autodescubre por reflection (motor genérico) o queda disponible apenas existe (categorías).

## Constraints
- **NO inventar la lógica de negocio/contractual** dentro de `DevolverClausulas`. Dejarla como placeholder (`TODO`/`NotImplementedException`) salvo que el usuario provea explícitamente la regla exacta a implementar.
- No usar `ProcesamientoClausulasOld` como referencia de patrón: es código legacy sin referencias activas.
- No crear un agent para esta tarea: por el riesgo de lógica contractual, este flujo se hace guiado paso a paso con el usuario, no delegado a un subagente aislado.
