# Desarrollo y validación

## Herramientas

- SDK .NET 10 estable (`global.json` permite bandas 10.0 posteriores).
- Node.js 22.12+ o 24 LTS y npm.
- Terraform >= 1.9 y < 2 para infraestructura.
- Docker solo si se construye la imagen; Azure CLI y acceso Azure solo para despliegue.

Ejecutar los comandos desde la raíz salvo indicación contraria.

```sh
dotnet restore CommunityOps.slnx
npm --prefix src/frontend ci
dotnet run --project src/backend/CommunityOps.AppHost
```

Aspire muestra la dirección autenticada del dashboard y los endpoints de sus
recursos. El perfil de desarrollo permite HTTP **únicamente para uso local**;
no exponer estos puertos ni reutilizar esa configuración en Azure.
El SDK descarga las herramientas de orquestación al restaurar; no se requiere
instalar un workload antiguo de Aspire ni disponer de un clúster Kubernetes.
La fuente oficial de Aspire es [aspire.dev](https://aspire.dev/), incluida su
[integración JavaScript/Vite](https://aspire.dev/integrations/frameworks/javascript/).

Si aparece ASPIRE010, indica que no se usa el bundle del CLI Aspire. El AppHost
usa las herramientas suministradas por NuGet; no depende de funciones del bundle.
En entornos que restrinjan DCP o fallen al inicializarlo, usar el arranque separado.
Para usar HTTPS local seguir la [guía Microsoft de certificados](https://learn.microsoft.com/en-us/aspnet/core/security/enforcing-ssl?view=aspnetcore-10.0#trust-the-aspnet-core-https-development-certificate).

## Arranque separado

En una terminal:

```sh
dotnet run --project src/backend/CommunityOps.Api --launch-profile http
```

En otra:

```sh
npm --prefix src/frontend run dev
```

Abrir la URL de Vite (habitualmente `http://127.0.0.1:5173`). Su proxy `/api`
apunta a `http://localhost:5111`. `API_BASE_URL` permite cambiar el destino;
Aspire lo establece automáticamente. No se necesita CORS para este proxy.

## Configuración

| Variable | Uso |
| --- | --- |
| `Events__Store` | `InMemory` solo en Development; `Cosmos` en Azure |
| `Cosmos__Endpoint` | URL HTTPS de la cuenta |
| `Cosmos__Database` | Por defecto `community-ops` |
| `Cosmos__Container` | Por defecto `events` |
| `Cors__AllowedOrigins__0` | Origen exacto permitido sin barra final |
| `VITE_API_BASE_URL` | URL pública de la API, incorporada al compilar el frontend |
| `API_BASE_URL` | Destino del proxy local de Vite, no variable del navegador |

Las variables `VITE_*` son públicas: nunca poner credenciales en ellas.
`.env.example` describe la configuración; `.env` y `.env.*` locales se ignoran.
La configuración local de desarrollo no debe activarse en un despliegue público.
Para probar Cosmos localmente usar `Events__Store=Cosmos`, `az login` y un rol de
datos en Cosmos para la identidad del desarrollador; no basta ser Contributor ARM.

## Comprobaciones

```sh
dotnet build CommunityOps.slnx --configuration Release
dotnet test CommunityOps.slnx --configuration Release --no-build
dotnet format CommunityOps.slnx --verify-no-changes --no-restore
npm --prefix src/frontend run lint
npm --prefix src/frontend test
npm --prefix src/frontend run build
npm --prefix src/frontend audit
terraform -chdir=infraestructure init -backend=false
terraform -chdir=infraestructure fmt -check
terraform -chdir=infraestructure validate
```

Backend: xUnit y WebApplicationFactory, sin servicios externos. Frontend: pruebas
del puerto/caso de uso y adaptador HTTP con `node:test`, sin dependencias de test
adicionales. Comprobación manual: carga, evento demo, catálogo vacío, error,
reintento y vista móvil. La CI ejecuta build/test, lint y validación Terraform sin
credenciales Azure; no aplica infraestructura.

La versión directa de MessagePack en AppHost corrige una dependencia transitiva
de Aspire afectada por GHSA-qhrr-8q5h-9q3h; revisar al actualizar Aspire.
