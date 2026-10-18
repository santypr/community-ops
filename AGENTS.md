# Community Ops: instrucciones para agentes

Responsabilidad del agente general: entregar cambios pequeños, completos y
verificables. Leer primero README y la documentación funcional y técnica de `docs`.

- Consultar `.specify/memory/constitution.md` y la especificación de la capacidad.
  Resolver decisiones críticas con `speckit-clarify` antes de `speckit-plan`;
  ejecutar `speckit-analyze` tras generar tareas y antes de implementar.
  Un borrador no autoriza cambios de permisos ni operaciones de escritura.

- Mantener Clean Architecture: Domain no depende de otras capas; Application
  define casos de uso y puertos; Infrastructure implementa adaptadores; Api y
  `main.tsx` componen dependencias. No introducir frameworks de abstracción sin
  un caso real.
- Las fuentes para arquitectura/código son React, Vite, Microsoft Docs para
  .NET/Azure y https://aspire.dev para Aspire, enlazadas en
  `docs/architecture.md`. Verificar versiones y no inventar APIs.
- Mantener documentación funcional, técnica, OpenAPI, uso y despliegue en `docs`.
- Para una funcionalidad vertical, consultar la skill `implement-feature`.
- Añadir pruebas de reglas, errores y contrato. Validar con los comandos de
  `docs/development.md`; no afirmar validaciones que no se hayan ejecutado.
- No incluir secretos, datos personales ni estado Terraform. `VITE_*` es público.
  Mantener Cosmos con identidad administrada y mínimo privilegio.
- No añadir operaciones de escritura públicas sin autenticación/autorización.
- No usar InMemory fuera de Development ni convertir CORS en un comodín.
- Si conviene una revisión independiente, delegarla como misión de solo lectura
  a un subagente disponible: contrato, capas y seguridad, con hallazgos que
  incluyan archivo y caso demostrable. El agente principal integra y verifica.
- No crear un rol/agente por cada capa ni subagentes para búsquedas triviales.
