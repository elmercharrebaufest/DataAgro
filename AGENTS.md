# AGENTS.md - Límites de Autonomía de IA

Cualquier agente/Copilot que interactúe con este repositorio debe apegarse
estrictamente a este mapa de límites.

## 1. ALWAYS (autónomo)
- Generar tests para toda lógica de negocio nueva o modificada.
- Formatear los archivos editados con el linter del proyecto.

## 2. ASK FIRST (pedir aprobación humana)
- Instalar dependencias npm o agregar paquetes NuGet.
- Modificar entidades que generen nuevas migraciones de EF Core.
- Cambiar firmas de controladores de API existentes o contratos externos.
- Modificar o desactivar tests unitarios previamente definidos.
- Confirmar o subir contraseñas, archivos .env, tokens o API keys.

## 3. NEVER (prohibido)
- Commitear `_sdd/progress/current/*` (es memoria local, va en .gitignore).
- Marcar una tarea `[x]` en tasks.md sin su test correspondiente en verde.
- Marcar un requisito de tipo "manual" como cubierto sin sign-off humano.
- Desactivar validaciones de seguridad, CORS, reglas de autorización o JWT.
