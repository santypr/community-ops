# Community Ops Constitution

## Core Principles

### I. Arquitectura limpia

Domain DEBE permanecer independiente de frameworks, transporte y persistencia.
Application DEBE definir casos de uso y puertos; Infrastructure DEBE implementar
adaptadores. Api y `main.tsx` DEBEN componer dependencias. AppHost solo orquesta.
La lógica de negocio NO DEBE trasladarse a endpoints, componentes React ni SDK.
Esto permite probar reglas sin servicios externos y sustituir adaptadores.

### II. Entrega vertical e incremental

Cada funcionalidad DEBE entregar un recorrido verificable con reglas, caso de
uso, contrato, adaptadores, interfaz, pruebas y documentación aplicables.
Las partes no aplicables DEBEN justificarse. Se DEBE conservar la consulta
pública existente, sin reconstruir la base ni ampliar alcance por inferencia.
Para implementar una funcionalidad vertical se DEBE usar `implement-feature`.

### III. Seguridad por defecto

Ninguna escritura pública se DEBE habilitar sin autenticación y autorización
verificadas en servidor. Los permisos DEBEN denegarse por defecto y comprobarse
sobre el recurso, no solo en la interfaz. CORS NO ES autenticación y sus orígenes
DEBEN ser explícitos. Cosmos DEBE usar identidad administrada en Azure, sin claves
y con mínimo privilegio. Su rol actual de lectura NO DEBE ampliarse hasta aprobar
un caso de uso de escritura. InMemory solo se permite en Development,
sin fallback silencioso en producción.

### IV. Privacidad y secretos

El catálogo público NO DEBE contener datos personales de asistentes ni datos
privados de administración. Antes de incorporar datos de identidad se DEBEN
definir finalidad, acceso, conservación y eliminación. NO se DEBEN versionar
secretos, credenciales ni estado Terraform. Las variables `VITE_*` son públicas
y NO DEBEN contener secretos. Errores y telemetría NO DEBEN revelar tokens
ni datos privados.

### V. Contratos y pruebas verificables

Todo cambio HTTP DEBE actualizar `docs/api/openapi.yaml` y sus pruebas de contrato.
Cada caso de uso DEBE probar éxito, reglas, entradas inválidas y errores de
dependencias; colección vacía, cancelación y permisos se DEBEN cubrir cuando
apliquen. El frontend DEBE contemplar carga, vacío, error, reintento, cancelación
y accesibilidad según el recorrido. Las plantillas genéricas NO convierten estas
pruebas en opcionales.

### VI. Simplicidad y reutilización

Se DEBEN reutilizar bibliotecas, herramientas y estructura existentes. Nuevas
dependencias, repositorios genéricos, buses, mediadores o frameworks de abstracción
DEBEN justificarse con una necesidad concreta y una alternativa más simple
evaluada. NO se crean agentes por capa ni se delegan búsquedas triviales.
La simplicidad reduce superficie de fallo y mantenimiento.

### VII. Calidad y evidencia

Antes de cerrar una entrega se DEBEN ejecutar las validaciones aplicables de
`docs/development.md`, revisar seguridad y escanear secretos. El resultado DEBE
distinguir comprobaciones ejecutadas, fallidas y no ejecutadas, con su motivo.
NO se DEBE afirmar que Azure, el editor o una integración funcionan sin probarlos.
Toda excepción DEBE registrar motivo, riesgo, responsable de revisión y condición
de resolución; no autoriza escrituras sin protección ni inclusión de secretos.

### VIII. Fuentes oficiales

Las decisiones y APIs DEBEN verificarse con las fuentes de `docs/architecture.md`:
React y Vite para frontend, Microsoft Docs para .NET/Azure y https://aspire.dev
para Aspire. Las versiones DEBEN comprobarse; no se inventan APIs ni se adopta
otro repositorio como autoridad técnica. Para Spec Kit se usa su documentación
oficial y la versión registrada en `.specify/init-options.json`.

## Alcance y restricciones operativas

La base es una agenda pública de solo lectura. Autenticación, escritura,
publicaciones e integraciones sociales son capacidades futuras, no comportamientos
habilitados por esta constitución.

El destino sigue siendo Azure Static Web Apps, Container Apps y Cosmos DB;
Aspire se limita a orquestación local. NO se requiere Kubernetes ni se despliega
el dashboard Aspire. NO se aplica Terraform ni se despliega Azure sin petición
explícita.

Antes de ampliar el uso público se DEBEN acordar observabilidad, disponibilidad
real de Cosmos, cuotas, costes, recuperación y protección frente a abuso.
`/health` solo acredita vida del proceso y el free tier no garantiza coste cero.

## Flujo de desarrollo y controles

1. Leer `AGENTS.md`, README, contexto funcional, arquitectura y contrato actual.
2. Usar `speckit-specify` y resolver ambigüedades con `speckit-clarify`.
   Identidad, permisos, aislamiento y datos DEBEN decidirse explícitamente;
   una duda crítica bloquea el diseño e implementación.
3. Preparar `speckit-plan` con un Constitution Check de I–VIII, capas, permisos,
   datos, contrato, pruebas y operación. Repetir el control después del diseño
   y antes de implementar.
4. Generar `speckit-tasks` con trazabilidad a requisitos e historias, pruebas
   y documentación obligatorias; ejecutar `speckit-analyze` antes de implementar.
   Los hallazgos críticos DEBEN resolverse primero.
5. Implementar con `implement-feature`, verificar y actualizar los documentos
   funcionales, técnicos, de uso y despliegue afectados.

Una entrega se acepta cuando cumple sus escenarios, conserva el catálogo actual,
alinea contrato y documentación y aporta evidencia de validación, sin problemas
de seguridad relevantes pendientes.

## Governance

Esta constitución establece controles junto con `AGENTS.md`. Las plantillas
de Spec Kit son guías subordinadas a ellos. Una contradicción se DEBE resolver
explícitamente, no ignorando una fuente.

Cada revisión DEBE comprobar I–VIII. Las enmiendas DEBEN explicar motivo,
impacto, cambios en plantillas/documentación y transición necesaria. Requieren
revisión y aprobación del mantenedor mediante el flujo de cambios del repositorio.
Esta primera versión queda propuesta hasta esa aprobación; no se presume
una ratificación humana.

El versionado es semántico: MAJOR para eliminar o redefinir principios de forma
incompatible; MINOR para añadir principios o ampliar obligaciones; PATCH para
aclaraciones sin cambio normativo. La última enmienda DEBE actualizarse y
la ratificación original conservarse una vez acordada.

**Version**: 1.0.0 | **Ratified**: TODO(RATIFICATION_DATE): aprobación del mantenedor pendiente | **Last Amended**: 2026-10-18
