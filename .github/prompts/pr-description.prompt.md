---
description: "Genera una descripción de Pull Request para Azure DevOps a partir de los cambios de la rama actual."
agent: "agent"
---
Generá una descripción de Pull Request para los cambios de la rama actual, para pegar manualmente en Azure DevOps (el repo no tiene plantilla de PR ni CONTRIBUTING.md hoy; esta es una estructura sugerida, no una convención ya establecida en el equipo).

Pasos:
1. Identificá el ticket: mirá el nombre de rama actual (`git branch --show-current`, patrón `feature/{autor}/DAT-XXXX`). Si no hay match, preguntame el ticket.
2. Mirá los commits y el diff de la rama contra `dev` (`git log dev..HEAD --oneline`, `git diff dev...HEAD`) para entender el alcance real de los cambios.
3. Generá la descripción con esta estructura:
   ```markdown
   ## DAT-XXXX

   ### Resumen
   {Qué problema resuelve o qué funcionalidad agrega, en 1-3 líneas}

   ### Cambios
   - {Cambio relevante 1}
   - {Cambio relevante 2}

   ### Cómo probar
   {Pasos concretos para validar el cambio manualmente}
   ```
4. No inventes "cómo probar" genérico: basate en lo que realmente cambió (qué pantalla/Manager/Controller se tocó).
