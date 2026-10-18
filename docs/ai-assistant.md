# Asistente IA

Se sigue el criterio del repositorio de referencia
[agents_skills_subagents](https://github.com/santypr/agents_skills_subagents),
especialmente su
[guía de decisión](https://github.com/santypr/agents_skills_subagents/blob/main/docs/agentes-skills-subagentes.md).

## Configuración mínima

- **Agente general del entorno** con instrucciones estables en `AGENTS.md`,
  enlazadas desde `.github/copilot-instructions.md`.
- **Skill `implement-feature`** en `.github/skills/implement-feature/SKILL.md`:
  procedimiento bajo demanda para un cambio vertical completo.
- **Revisión delegada opcional**: subagente disponible de solo lectura con misión
  concreta; no se necesita un tipo de archivo «subagente».

No se crean agentes personalizados por capa: no aportan responsabilidades ni
límites distintos en esta fase. Si aparece una necesidad recurrente de revisión
con herramientas restringidas, se podrá añadir ese rol específico.

## Uso

Pedir al agente implementar un caso de uso y utilizar `implement-feature`.
Debe consultar contexto y contrato, separar capas, comprobar documentación
oficial y entregar pruebas y limitaciones. Las referencias oficiales React,
Vite, Microsoft Docs (.NET/Azure) y https://aspire.dev (Aspire) están en
`docs/architecture.md`; el repositorio de ejemplo
orienta la personalización del asistente, no la arquitectura de la aplicación.

El descubrimiento de skills depende del editor, versión y configuración.
Comprobar en el entorno real que aparecen las instrucciones y la skill; su
presencia en Git no prueba que el editor las haya cargado. Ningún archivo de
instrucciones sustituye permisos, revisión humana o validaciones automáticas.
