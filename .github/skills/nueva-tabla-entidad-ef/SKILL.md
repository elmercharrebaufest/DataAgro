---
name: nueva-tabla-entidad-ef
description: 'Workflow para agregar una tabla nueva a la base de datos y su entidad EF correspondiente en DataAgro, sin usar Molinos.DataAgro.Mapping (código muerto, sin referencias). Usar cuando pidan crear una tabla nueva, una entidad EF nueva, o agregar un campo a una entidad existente.'
---

# Nueva tabla + entidad EF

## Contexto técnico
- El mapeo EF de `DataAgroDbContext` es **por convención**, no por Fluent API: `OnModelCreating` registra por reflection todas las clases del namespace de `Contrato` (`Molinos.DataAgro.Entities.Entities`). Alcanza con que la entidad nueva esté en ese namespace para quedar mapeada, sin tocar `DataAgroDbContext`.
- **No usar `Molinos.DataAgro.Mapping`** (clases `EntityTypeConfiguration<T>`): es código muerto, sin ninguna referencia activa en el proyecto. No crear clases `*Map` nuevas ahí ni tomarlas como ejemplo.
- Las entidades son POCOs simples, sin pluralización de nombre de tabla (`Zona` → tabla `Zona`, no `Zonas`):
  ```csharp
  public partial class {Nombre}
  {
      [Key]
      public int Id { get; set; }   // o "{Nombre}Id", ambas conviven en el código existente
      public string Descripcion { get; set; }
  }
  ```

## Procedimiento

1. **Crear el script de la tabla** en `Base de Datos/dbo/Tables/{Nombre}.sql` (proyecto SSDT), con las columnas que indique el usuario.
2. **Crear la entidad** en `Molinos.DataAgro.Entities/Entities/{Nombre}.cs`: POCO con `[Key]` en la PK, propiedades con el mismo nombre que las columnas (EF las mapea por convención).
3. **Confirmar con el usuario** el nombre de la propiedad de clave primaria (`Id` vs `{Nombre}Id`): ambas conviven en el código existente, no forzar una.
4. **Recordar el paso manual de despliegue**: el script SQL no se aplica solo. Hace falta publicar el proyecto SSDT (schema compare) contra la base de datos del ambiente correspondiente — avisar al usuario que este paso lo tiene que hacer un desarrollador/DBA.
5. Si la tabla nueva necesita exponerse en la UI o en un Manager, continuar con la skill [nueva-operacion-manager](../nueva-operacion-manager/SKILL.md).

## Fuera de alcance
- No crear ni modificar `Molinos.DataAgro.Mapping`.
- No modificar `DataAgroDbContext.OnModelCreating` salvo que la entidad nueva quede fuera del namespace `Molinos.DataAgro.Entities.Entities` (caso excepcional, confirmar con el usuario antes).
