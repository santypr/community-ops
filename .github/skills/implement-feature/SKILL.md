---
name: implement-feature
description: Implementar una funcionalidad vertical en Community Ops con Clean Architecture, contrato API, pruebas y documentación.
---

# Implementar una funcionalidad

## Cuándo aplicarla

Al añadir o cambiar un caso de uso que afecte API, persistencia o interfaz.
No requiere crear otro agente ni ejecutar scripts adicionales.

## Procedimiento

1. Leer `AGENTS.md`, `docs/functional-context.md`, `docs/architecture.md` y el
   contrato `docs/api/openapi.yaml`. Aclarar reglas y alcance no definidos.
2. Verificar APIs de framework en las fuentes oficiales enlazadas. No copiar
   patrones de otros repositorios como autoridad técnica.
3. Identificar las invariantes del dominio y el caso de uso. Definir o extender
   el puerto mínimo en Application; evitar abstracciones genéricas prematuras.
4. Añadir pruebas de éxito, colección vacía, entradas inválidas, errores de
   dependencias y cancelación según aplique. Definir contrato y estados HTTP.
5. Implementar el adaptador Infrastructure y componerlo en Api; mantener la
   lógica de negocio fuera del endpoint y del SDK de persistencia.
6. En frontend, mantener tipos de dominio, caso de uso/puerto, adaptador HTTP y
   presentación separados. Inyectar adaptadores desde `main.tsx`; contemplar
   carga, vacío, fallo, reintento, cancelación y accesibilidad.
7. Revisar autenticación/autorización antes de añadir escritura o datos privados.
   No usar claves Cosmos ni secretos en frontend; mantener privilegio mínimo.
8. Actualizar documentación funcional, OpenAPI, manual y despliegue si cambian.
9. Ejecutar build/test .NET y lint/test/build frontend de `docs/development.md`.
   Si cambia Terraform, ejecutar init sin backend, fmt y validate. Revisar diff
   y secretos. No desplegar Azure sin petición explícita.
10. Cuando el cambio lo justifique, delegar una revisión de solo lectura a un
    subagente con objetivo, contexto, restricciones y salida esperada. Comprobar
    sus hallazgos antes de modificar código.

## Entrega

Resumir comportamiento, pruebas ejecutadas y limitaciones reales. No afirmar
que Azure o el descubrimiento del editor funcionan sin haberlos probado.
