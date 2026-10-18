# Community Ops

Base para gestionar comunidades técnicas, sus eventos, publicaciones y canales
sociales. Esta primera entrega implementa la **consulta pública de eventos**:
API .NET 10, React + TypeScript con Vite y Clean Architecture en ambos.

## Inicio rápido

Requisitos: SDK .NET 10 y Node.js 22.12+ (o 24 LTS).
Desde la raíz del repositorio:

```sh
dotnet restore CommunityOps.slnx
npm --prefix src/frontend ci
dotnet run --project src/backend/CommunityOps.AppHost
```

Aspire orquesta API y Vite; abre el enlace del frontend indicado en su dashboard.
El perfil local usa HTTP en loopback con autenticación del dashboard.
No necesita Azure, Cosmos ni contenedores para consultar el evento ficticio.
Si el entorno no permite ejecutar Aspire, consulta el arranque separado en la
[guía de desarrollo](docs/development.md).

## Organización

- `infraestructure/`: Terraform para Azure Static Web Apps, Container Apps y Cosmos DB.
- `docs/`: contexto funcional/técnico, contrato API, manual y despliegue.
- `src/backend/`: Domain, Application, Infrastructure, Api, AppHost y pruebas.
- `src/frontend/`: React + TypeScript con adaptador HTTP separado de la interfaz.

## Documentación

- [Alcance funcional](docs/functional-context.md)
- [Arquitectura y fuentes oficiales](docs/architecture.md)
- [Desarrollo y validación](docs/development.md)
- [Contrato OpenAPI](docs/api/openapi.yaml)
- [Manual de usuario](docs/user-manual.md)
- [Guía de despliegue](docs/deployment-guide.md)
- [Asistente IA: decisiones y uso](docs/ai-assistant.md)
- [Constitución del proyecto (propuesta 1.0.0)](.specify/memory/constitution.md)
- [Primer hito: identidad, permisos y eventos (borrador)](specs/001-identity-event-management/spec.md)

La infraestructura no se despliega automáticamente. Cosmos usa identidad
administrada sin claves; el modo en memoria se limita a Development.
Autenticación de usuarios, escritura, publicaciones e integraciones sociales
quedan fuera de esta entrega inicial.
