---
description: "Genera un mensaje de commit siguiendo la convención del equipo DataAgro (Azure DevOps, ticket DAT-XXXX)."
agent: "agent"
---
Generá un mensaje de commit para los cambios actuales (staged o, si no hay staged, working tree), siguiendo la convención observada en el historial del repo (`git log`): `DAT-XXXX - Descripción breve en español, en modo indicativo`.

Pasos:
1. Mirá el nombre de la rama actual (`git branch --show-current`). Si matchea el patrón `feature/{autor}/DAT-XXXX`, usá ese ticket. Si no matchea o no hay ticket claro, preguntame el número de ticket antes de generar el mensaje.
2. Mirá el diff de los cambios (`git diff --staged`, o `git diff` si no hay nada staged) para entender qué se modificó.
3. Generá un único mensaje en una línea: `DAT-XXXX - {Descripción}`. La descripción debe ser concisa (una oración), en español, describiendo el efecto del cambio (no el detalle técnico de cada archivo).
4. No hagas el commit vos: solo proponé el mensaje para que el usuario lo use.
