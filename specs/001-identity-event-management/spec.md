# Feature Specification: Identidad, permisos y gestión de eventos

**Feature Branch**: rama de trabajo actual; independiente del directorio de especificación

**Created**: 2026-10-18

**Status**: Draft — bloqueado para diseño e implementación por Q1–Q3

**Input**: Primer hito del plan: especificar identidad, permisos y gestión de
eventos sobre la agenda pública existente, sin reconstruirla.

## Clarifications

### Session 2026-10-18

- Q: ¿Con qué cuentas podrán identificarse los responsables de las comunidades? → A: Cuentas Microsoft y Google mediante OAuth.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Acceso de responsables (Priority: P1)

Como responsable de una comunidad, quiero identificarme y acceder únicamente
a las acciones y recursos para los que tengo permiso.

**Why this priority**: Sin acceso y permisos verificables no se puede habilitar escritura.

**Independent Test**: Con identidades de prueba, comprobar acceso permitido,
acceso denegado y sesión caducada, sin necesitar operaciones de edición.

**Acceptance Scenarios**:

1. **Given** un visitante sin sesión, **When** consulta la agenda pública,
   **Then** puede seguir viendo eventos sin identificarse.
2. **Given** un responsable habilitado, **When** se identifica correctamente,
   **Then** solo obtiene las acciones autorizadas por la matriz aprobada en Q2.
3. **Given** una identidad sin permiso o una sesión inválida/caducada,
   **When** intenta una acción restringida, **Then** se deniega sin cambiar datos,
   aunque la petición no provenga de la interfaz.
4. **Given** una sesión activa, **When** el responsable cierra sesión,
   **Then** esa sesión ya no permite acciones restringidas.

### User Story 2 - Crear y modificar eventos (Priority: P2)

Como responsable autorizado, quiero crear y corregir eventos de mi ámbito,
identificando cuáles son propios y cuáles pertenecen a terceros.

**Why this priority**: Permite operar el catálogo después de aprobar US1.

**Independent Test**: Con una identidad y comunidad autorizadas de prueba,
crear y modificar un evento y comprobar el resultado público según Q3.
Depende de US1; no exige completar borrado ni otras capacidades futuras.

**Acceptance Scenarios**:

1. **Given** permisos de creación, **When** se envía un evento válido,
   **Then** queda asociado a una comunidad autorizada y conserva su clasificación
   como propio o de terceros.
2. **Given** un evento editable, **When** se modifica con valores válidos,
   **Then** se conservan los cambios y se confirma el resultado al responsable.
3. **Given** campos vacíos o un fin no posterior al inicio,
   **When** se intenta guardar, **Then** se rechaza sin persistir cambios.
4. **Given** un evento fuera del ámbito autorizado, **When** se cambia su
   identificador o comunidad en una petición, **Then** se deniega la operación.
5. **Given** un fallo de almacenamiento, **When** se intenta guardar,
   **Then** no se comunica éxito y se permite recuperarse sin exponer datos privados.

### User Story 3 - Retirar eventos (Priority: P3)

Como responsable autorizado, quiero retirar un evento sin afectar a otros eventos.

**Why this priority**: Completa el mantenimiento después de US1 y US2.

**Independent Test**: Retirar un evento de prueba, verificar su ausencia pública
y comprobar que otro evento sigue disponible, según el ciclo aprobado en Q3.

**Acceptance Scenarios**:

1. **Given** un evento y permisos de retirada, **When** se confirma su retirada,
   **Then** deja de aparecer públicamente según Q3 y se confirma el resultado.
2. **Given** ausencia de permiso, **When** se solicita retirar un evento,
   **Then** se deniega sin modificar el catálogo.
3. **Given** un identificador inexistente, **When** se solicita la retirada,
   **Then** se informa del resultado sin revelar recursos privados.

### Edge Cases

- Sesión que caduca o permiso que se revoca entre abrir el formulario y guardar.
- Cambio de comunidad en una petición para intentar eludir autorización.
- Fechas con distintos husos horarios: se conserva la regla fin posterior al inicio.
- Evento inexistente o retirado mientras otro responsable lo está editando:
  Q3 debe definir el conflicto y evitar pérdida silenciosa de cambios.
- Reintento tras una respuesta perdida: Q3 debe fijar cómo evitar duplicados.
- Fallo del acceso de responsables: la agenda pública sigue sin exigir sesión.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: La agenda pública DEBE seguir accesible sin sesión, conservar
  sus campos actuales y su límite y orden actuales hasta otro cambio aprobado.
- **FR-002**: El acceso de responsables DEBE admitir cuentas Microsoft y Google
  mediante OAuth, identificar al usuario y permitir cerrar sesión. Identificarse
  no concede permisos por sí solo.
  [NEEDS CLARIFICATION: Q1 — ¿Qué audiencia de cuentas Microsoft y Google se admite,
  y qué datos mínimos se conservan, durante cuánto tiempo y cómo se eliminan?]
- **FR-003**: Cada acción restringida DEBE comprobar permisos sobre el recurso
  y denegar por defecto. [NEEDS CLARIFICATION: Q2 — ¿Quién concede y revoca acceso,
  qué roles/acciones existen y cuál es su ámbito por comunidad? Confirmar si
  se excluye el aislamiento entre organizaciones en este hito.]
- **FR-004**: Los responsables autorizados DEBEN poder crear, modificar y retirar
  eventos de comunidades autorizadas, distinguiendo propios y de terceros.
  [NEEDS CLARIFICATION: Q3 — ¿Los cambios son públicos inmediatamente o requieren
  revisión, la retirada es lógica o definitiva, y cómo se resuelven conflictos
  de edición y reintentos de creación?]
- **FR-005**: Todo evento DEBE tener identificador no vacío, comunidad, título
  y ubicación no vacíos, fechas normalizadas y fin posterior al inicio.
- **FR-006**: La interfaz DEBE distinguir carga, vacío, éxito y error cuando
  apliquen, permitir reintento seguro y ser operable con teclado.
- **FR-007**: El catálogo público NO DEBE incluir datos de identidad ni información
  privada de administración o asistentes.
- **FR-008**: Los errores NO DEBEN exponer credenciales ni detalles internos.
- **FR-009**: La entrega DEBE incluir pruebas de reglas, errores, permisos,
  contrato y regresión del catálogo, además de cancelación cuando aplique.

### Key Entities *(include if feature involves data)*

- **Identidad de responsable**: referencia al usuario identificado con cuenta
  Microsoft o Google mediante OAuth; datos, audiencia y conservación pendientes de Q1.
- **Asignación de permisos**: acciones y recursos permitidos, con concesión y
  revocación pendientes de Q2. No implica roles aprobados todavía.
- **Comunidad**: ámbito de asociación del evento. Su administración completa
  se especificará aparte; Q2 debe definir cómo se asignan comunidades autorizadas.
- **Evento**: información pública existente y clasificación propio/terceros;
  publicación, retirada y conflictos pendientes de Q3.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Todos los escenarios de consulta pública existentes siguen pasando
  sin que el visitante tenga que iniciar sesión.
- **SC-002**: El 100 % de los intentos sin sesión válida, sin permiso o sobre
  recursos fuera del ámbito aprobado se deniegan sin cambiar datos.
- **SC-003**: Un responsable autorizado completa creación, modificación y
  retirada de un evento de prueba, y el catálogo refleja cada resultado
  conforme al ciclo de publicación aprobado.
- **SC-004**: Ningún evento inválido de los escenarios de prueba se persiste.
- **SC-005**: Ninguna respuesta pública de los escenarios incluye datos privados
  de identidad, administración ni asistentes.

## Assumptions

- Esta especificación es una propuesta, no una funcionalidad implementada
  ni una aprobación de las decisiones pendientes.
- Q1–Q3 bloquean diseño, contratos de escritura, tareas de implementación
  y cambios de permisos. Se han seleccionado Microsoft y Google mediante OAuth;
  la audiencia, la política de datos y el modelo de roles siguen pendientes.
- Quedan fuera inscripción de asistentes, publicaciones, anuncios, difusión
  social y administración completa de comunidades.
- No se asume aislamiento multi-organización; si Q2 lo exige, debe especificarse
  explícitamente antes de diseñar permisos y persistencia.
- Las pruebas de aceptación se ejecutarán con datos e identidades ficticios.
