# Contexto funcional

## Objetivo

Centralizar la operación de comunidades técnicas: calendario y anuncios de
eventos propios y de terceros, publicaciones técnicas y difusión social.

## Primera entrega

Un visitante puede consultar la agenda sin iniciar sesión. Cada evento presenta
comunidad, título, inicio, fin y ubicación. Las fechas se muestran en el huso
horario del navegador. La pantalla distingue carga, catálogo vacío y fallo de
conexión, con reintento.

La API devuelve como máximo los primeros 50 eventos por fecha de inicio
ascendente, incluidos eventos pasados. No es todavía un calendario interactivo,
un buscador ni una API paginada. El cliente no escribe datos.

En desarrollo hay un único evento ficticio marcado como demo, fechado en 2030.
En Azure el contenedor Cosmos empieza vacío; no se insertan datos de ejemplo
automáticamente.

## Reglas

- Identificador UUID no vacío y comunidad, título y ubicación no vacíos.
- El fin debe ser posterior al inicio.
- Las fechas del dominio se normalizan a UTC.
- Solo se publicarán datos aptos para consulta pública; no almacenar datos de
  asistentes ni información personal en este catálogo.

## Fuera de alcance

Alta/edición/borrado de eventos, usuarios y roles, múltiples organizaciones con
aislamiento, inscripción de asistentes, anuncios, publicaciones, OAuth de redes
sociales y programación de mensajes. Requieren definición funcional,
autenticación/autorización y contratos específicos antes de implementarse.

## Evolución propuesta

Este orden es una hoja de ruta, no alcance implementado ni autorización
para habilitar escritura:

| Hito | Resultado esperado | Condición de entrada y aceptación |
| --- | --- | --- |
| 0. Constitución | Principios y controles compartidos | Revisar y ratificar `.specify/memory/constitution.md` |
| 1. Identidad y autorización | Acceso de responsables y permisos | Acordar proveedor, audiencia, política de datos y matriz de permisos; demostrar denegación por defecto |
| 2. Comunidades y eventos | Gestión autorizada; eventos propios y de terceros | Acordar alcance de comunidades y publicación; probar reglas, errores, permisos y regresión pública |
| 3. Agenda ampliada | Búsqueda, filtros y paginación | Validar necesidades y compatibilidad; revisar consumo e índices |
| 4. Publicaciones y anuncios | Preparación, revisión y publicación | Definir estados, permisos y criterios editoriales |
| 5. Difusión social | Conexiones autorizadas, programación y resultados | Aprobar proveedores, permisos, custodia de credenciales, reintentos y revocación |

El [borrador del primer hito](../specs/001-identity-event-management/spec.md)
abarca identidad, permisos y mantenimiento de eventos; contiene decisiones
pendientes que bloquean diseño e implementación. La gestión completa de
comunidades se especificará por separado.

El aislamiento entre organizaciones y la inscripción de asistentes necesitan
decisión explícita: no se incorporan por defecto. Cada capacidad seguirá
especificación, aclaración, plan, tareas y análisis antes de implementarse.

Antes de ampliar el uso público se deben acordar indicadores y alertas,
comprobación real de disponibilidad de Cosmos separada de `/health`, cuotas,
presupuesto, recuperación y protección frente a abuso. La salida de cada hito
requiere escenarios aceptados, contrato/documentación actualizados y evidencia
de validación, sin problemas de seguridad relevantes pendientes.
