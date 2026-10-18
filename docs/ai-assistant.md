# Asistente IA

Se sigue el criterio del repositorio de referencia
[agents_skills_subagents](https://github.com/santypr/agents_skills_subagents),
especialmente su
[guía de decisión](https://github.com/santypr/agents_skills_subagents/blob/main/docs/agentes-skills-subagentes.md).

El flujo de especificación usa [Spec Kit oficial](https://github.com/github/spec-kit).
No sustituye las fuentes técnicas de `docs/architecture.md`.

## Configuración mínima

- **Agente general del entorno** con instrucciones estables en `AGENTS.md`,
  enlazadas desde `.github/copilot-instructions.md`.
- **Skill `implement-feature`** en `.github/skills/implement-feature/SKILL.md`:
  procedimiento bajo demanda para un cambio vertical completo.
- **Revisión delegada opcional**: subagente disponible de solo lectura con misión
  concreta; no se necesita un tipo de archivo «subagente».
- **Spec Kit para Copilot**: skills `speckit-*` en `.github/skills/`,
  constitución en `.specify/memory/constitution.md`, plantillas y scripts
  en `.specify/`, y especificaciones por capacidad en `specs/`.

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

## Spec Kit: instalación y reproducción

Se inicializó sobre el repositorio existente con Specify CLI **1.2.1**,
integración `copilot`, modo skills y scripts Bash. Python 3.11+ es un requisito
de esta herramienta de desarrollo, no del runtime de Community Ops.
La aplicación no depende de Specify ni necesita instalarlo para arrancar.
Los scripts, plantillas, workflow y skills `speckit-*` proceden de Spec Kit;
se conserva su licencia MIT en `.specify/LICENSE`.

Para disponer de la misma CLI, en un entorno con `uv`:

```sh
uv tool install specify-cli==1.2.1
specify integration status
```

La inicialización original desde la raíz fue:

```sh
specify init --here --force --non-interactive --integration copilot --script sh --ignore-agent-tools
```

**No repetirla como paso habitual**: `--force` puede sobrescribir las plantillas
personalizadas. Antes de actualizar, revisar los cambios de la versión oficial,
trabajar con el árbol limpio y comparar el resultado. `init-options.json`
registra la versión; los manifiestos permiten identificar artefactos gestionados.
Las plantillas locales de spec, plan y tareas se han adaptado para permisos,
Constitution Check y pruebas obligatorias. Conservar esas adaptaciones al actualizar.
La skill `speckit-taskstoissues` también se adapta para deduplicar por directorio
de funcionalidad e identificador de tarea, no por `T001` global al repositorio.
No se instalaron extensiones de Git ni se crearon otras ramas.

## Flujo por capacidad

En esta integración las skills se invocan con **guiones**, no con puntos:
`/speckit-constitution`, no `/speckit.constitution`. Son instrucciones para
el chat del agente, no ejecutables de terminal.

1. `/speckit-constitution`: revisar los ocho principios y su gobierno.
   La versión propuesta es 1.0.0; la ratificación del mantenedor sigue pendiente.
2. `/speckit-specify`: definir una capacidad, sus actores, escenarios, requisitos
   y resultados medibles en `specs/<número>-<capacidad>/spec.md`.
3. `/speckit-clarify`: resolver decisiones críticas y registrar respuestas.
   El borrador inicial tiene pendientes identidad/datos, permisos/ámbito y
   publicación/retirada/conflictos. No se inventan respuestas.
4. `/speckit-plan`: tras resolverlas, definir contrato, capas, persistencia,
   permisos, experiencia de usuario y pruebas. Verificar Constitution Check
   antes y después del diseño.
5. `/speckit-tasks`: desglosar entregas verticales con pruebas y documentación.
6. `/speckit-analyze`: revisar consistencia y resolver hallazgos críticos antes
   de `/speckit-implement`, usando también `implement-feature`.

El workflow automatizado genérico incluido por la distribución no se ejecuta
como parte de esta entrega: no incluye los pasos de aclaración y análisis.
Usar la secuencia manual anterior; adaptar y revisar sus controles antes
de adoptar automatización. No aprobar gates con decisiones críticas pendientes.

Para retomar el borrador en un checkout nuevo sin depender del nombre de rama,
seleccionar explícitamente `SPECIFY_FEATURE_DIRECTORY=specs/001-identity-event-management`
en el contexto del agente y en los comandos de terminal. El puntero
`.specify/feature.json` es local y está ignorado por Git. Por ejemplo:

```sh
SPECIFY_FEATURE_DIRECTORY=specs/001-identity-event-management \
  .specify/scripts/bash/check-prerequisites.sh --paths-only --json
```

El chequeo completo de prerrequisitos debe fallar mientras falten el plan y las
tareas: no son artefactos aprobados ni se han generado para eludir Q1–Q3.
La [lista de requisitos](../specs/001-identity-event-management/checklists/requirements.md)
registra ese bloqueo. La [hoja de ruta](functional-context.md#evolución-propuesta)
ordena las siguientes capacidades sin habilitarlas.

Los archivos de integración se generaron con la CLI y se revisaron en este
entorno; el descubrimiento de las nuevas skills en el editor debe comprobarse
allí. Si no están disponibles como herramientas del agente, se puede seguir
el procedimiento de su `SKILL.md` sin afirmar que se ejecutó un comando de chat.
