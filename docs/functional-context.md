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
