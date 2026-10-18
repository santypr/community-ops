# Arquitectura

## Dependencias hacia el dominio

Backend:

```text
Domain ← Application ← Infrastructure
              ↑              ↑
              └──── Api ─────┘
                      ↑
                   AppHost (solo orquestación)
```

- **Domain**: `CommunityEvent` y sus invariantes; sin frameworks ni persistencia.
- **Application**: puerto `IEventRepository`, caso de uso `ListEvents` y DTO.
- **Infrastructure**: adaptadores en memoria y Cosmos SDK; no contiene endpoints.
- **Api**: composición de dependencias, configuración, HTTP, CORS y Problem Details.
- **AppHost**: inicia API/Vite con Aspire e inyecta el destino del proxy local.
- **Tests**: invariantes, casos de uso e integración HTTP con WebApplicationFactory.

No se añaden repositorios genéricos, bus, CQRS framework ni mediador para un
único caso de uso. La API es la raíz de composición y puede conocer los
adaptadores; Application no depende de ellos.

Frontend:

```text
domain ← application ← infrastructure
              ↑               ↑
         App (UI) ← main (composición)
```

`EventRepository` permite sustituir el transporte. `HttpEventRepository`
valida la estructura JSON recibida. `App` solo conoce el puerto y el caso de uso;
`main` inyecta el adaptador. Las peticiones se cancelan al desmontar la vista.
React se ocupa de renderizar texto, sin HTML dinámico.

## Persistencia y configuración

Cosmos for NoSQL: base `community-ops`, contenedor `events`, partición
`/communityId`. Documentos con nombres camelCase y fechas UTC ISO 8601
uniformes, como en [el ejemplo](api/sample-event.json). La consulta atraviesa
particiones y limita el resultado a 50 documentos; revisar paginación,
consumo RU e índices cuando crezca el catálogo.

`CosmosClient` es singleton, usa System.Text.Json, transporte Gateway e identidad
Entra mediante `DefaultAzureCredential`. Newtonsoft.Json es una dependencia
requerida por el paquete Cosmos SDK, no el serializador del dominio.

El adaptador en memoria solo se admite explícitamente en Development.
Production exige endpoint HTTPS y utiliza Cosmos; no hay fallback silencioso.
Una configuración inválida impide arrancar. `/health` indica **vida del proceso**,
no conectividad, permisos ni disponibilidad de Cosmos.

## Azure

Terraform crea Static Web Apps Free, Container Apps de consumo (0–1 réplicas),
Log Analytics y Cosmos con 400 RU/s compartidas y free tier configurable.
La identidad de la API recibe el rol de datos **Cosmos DB Built-in Data Reader**
a nivel de cuenta, nunca permisos de escritura ni claves.

SWA Free no permite enlazar Container Apps como backend integrado. El frontend
usa la URL HTTPS pública de la API incorporada durante su build y CORS restringe
el origen a SWA. CORS no es autenticación: el catálogo es intencionalmente público.
No se despliega el dashboard Aspire en Azure.

Cosmos mantiene endpoint de red público con autenticación local deshabilitada.
No es una topología de red privada. Para producción sensible definir aislamiento
de red, autenticación, cuotas/rate limiting, observabilidad, recuperación y política
de datos antes de ampliar el alcance. Las cuotas free tier no garantizan coste cero.

## Fuentes de verdad

Para arquitectura y código se priorizan las referencias oficiales, no tutoriales:

- [Microsoft: Clean Architecture y aplicaciones web](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures)
- [Microsoft: Minimal APIs](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/overview?view=aspnetcore-10.0)
- [Microsoft: errores y Problem Details](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/error-handling?view=aspnetcore-10.0)
- [Microsoft: Aspire](https://learn.microsoft.com/en-us/dotnet/aspire/get-started/aspire-overview)
- [Microsoft: Cosmos SDK para .NET](https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/sdk-dotnet-v3)
- [Microsoft: RBAC Cosmos](https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/security/how-to-grant-data-plane-access)
- [Microsoft: Container Apps como backend de SWA](https://learn.microsoft.com/en-us/azure/static-web-apps/apis-container-apps)
- [React: aplicación desde cero](https://react.dev/learn/build-a-react-app-from-scratch)
- [React: efectos y sincronización](https://react.dev/learn/synchronizing-with-effects)
- [Vite: guía](https://vite.dev/guide/)
- [Vite: variables de entorno](https://vite.dev/guide/env-and-mode)

Para la sintaxis de Terraform, consultar además la
[documentación oficial del proveedor AzureRM](https://registry.terraform.io/providers/hashicorp/azurerm/4.50.0/docs).
