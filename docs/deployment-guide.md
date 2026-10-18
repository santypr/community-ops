# Despliegue en Azure

## Requisitos y límites

Azure CLI, Terraform >= 1.9, Docker y acceso a una suscripción. Se necesitan
permisos para crear recursos y asignaciones **de datos** Cosmos (no solo ARM).
Comprobar registro de proveedores Microsoft.App, Microsoft.OperationalInsights,
Microsoft.DocumentDB y Microsoft.Web, regiones y cuotas.

Cosmos permite una cuenta free tier por suscripción; se elige al crearla.
La plantilla habilita free tier y 400 RU/s compartidas. Si ya está ocupada,
decidir explícitamente si usar una cuenta existente o desactivar la opción,
aceptando el coste. Container Apps, Log Analytics, almacenamiento y tráfico pueden
generar cargos aunque el frontend sea Free. Configurar presupuesto y alertas.

No hay pipeline de despliegue automático ni credenciales en el repositorio.

## 1. Publicar la API

Construir desde la raíz y publicar en un registro accesible **sin credenciales**
por Container Apps, por ejemplo un paquete público GHCR:

```sh
docker build -f src/backend/CommunityOps.Api/Dockerfile -t ghcr.io/YOUR_ORG/community-ops-api:initial .
docker push ghcr.io/YOUR_ORG/community-ops-api:initial
```

Autenticarse previamente en el registro fuera del código. Sustituir `YOUR_ORG`
por el nombre real en minúsculas y configurar la visibilidad pública del paquete.
Usar tags únicos por entrega o, preferiblemente, digest `@sha256:...`.
La plantilla no configura pull de imágenes privadas ni publica la imagen.
El contenedor ejecuta .NET como usuario no root y escucha en 8080.

## 2. Aprovisionar

Desde la raíz:

```sh
az login
az account set --subscription YOUR_SUBSCRIPTION
cp infraestructure/terraform.tfvars.example infraestructure/terraform.tfvars
```

Editar el archivo local con suscripción, prefijo **globalmente único**, región
e imagen publicada. No versionar valores específicos del entorno.

```sh
terraform -chdir=infraestructure init
terraform -chdir=infraestructure fmt -check
terraform -chdir=infraestructure validate
terraform -chdir=infraestructure plan -out=deployment.tfplan
terraform -chdir=infraestructure apply deployment.tfplan
terraform -chdir=infraestructure output
```

Se provisionan grupo de recursos, SWA Free, entorno Container Apps, API,
Log Analytics, Cosmos y rol Data Reader para la identidad de la API.
La creación del rol es posterior a la identidad: los primeros GET pueden fallar
mientras se propaga RBAC; reintentar después. No se crean ni se usan claves Cosmos.
La API debe leer `[]` al estar vacío el catálogo.

El estado Terraform contiene información sensible del proveedor (incluido el
token de despliegue SWA aunque no se exponga como output). El estado y los planes
están ignorados, pero deben protegerse igualmente. Antes de trabajar en equipo,
configurar un backend remoto Azure Storage con RBAC, cifrado y bloqueo y migrar el
estado con `terraform init -migrate-state`. No publicar estado, planes ni tokens.
La plantilla no crea su propio backend remoto.

## 3. Publicar el frontend

SWA Free no admite el enlace integrado de Container Apps. Compilar con la URL
HTTPS pública que devuelve Terraform:

```sh
npm --prefix src/frontend ci
VITE_API_BASE_URL="$(terraform -chdir=infraestructure output -raw api_url)" npm --prefix src/frontend run build
```

Obtener el token de despliegue de SWA mediante el portal o Azure CLI
(`az staticwebapp secrets list`) y mantenerlo solo en la sesión o almacén seguro.
Publicar `src/frontend/dist` con el
[CLI oficial de Static Web Apps](https://learn.microsoft.com/en-us/azure/static-web-apps/static-web-apps-cli-deploy)
(`swa deploy src/frontend/dist --env production`) con `SWA_CLI_DEPLOYMENT_TOKEN`
en el entorno. Instalar el CLI según esa guía; no es dependencia de la aplicación.
No pasar tokens en archivos versionados ni incluirlos en `VITE_*`.

La URL API es configuración **de compilación**; cambiarla requiere recompilar y
volver a publicar los archivos estáticos. CORS permite únicamente el hostname SWA
creado por Terraform; añadir explícitamente cualquier dominio personalizado.

## 4. Publicar el primer evento

La API es de solo lectura y no necesita permisos de escritura.
Un operador autorizado puede insertar [el documento de ejemplo](api/sample-event.json)
en `community-ops/events` usando Cosmos Data Explorer con Entra ID.
Asignar a esa identidad el rol de datos Cosmos DB Built-in Data Contributor,
limitado al alcance necesario, según la
[guía RBAC oficial](https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/security/how-to-grant-data-plane-access).
El rol ARM Contributor por sí solo no permite leer/escribir documentos.
No habilitar claves ni ampliar los permisos de la API para esta operación.

Mantener `/communityId` y campos camelCase, UUID válido y fechas UTC ISO 8601
uniformes, con fin posterior al inicio. No insertar datos privados: el catálogo
es público. El dominio valida los documentos al leerlos; un documento inválido
puede hacer fallar la consulta completa.

## 5. Verificar y operar

- `GET <api_url>/health`: 200 y `Healthy` (solo proceso, no valida Cosmos).
- `GET <api_url>/api/events`: 200 con `[]` o documentos insertados.
- Abrir `frontend_url`, comprobar carga, fechas, vista vacía y reintento.
- Ante 403 Cosmos, revisar rol **de datos**, identidad y propagación.
- Ante fallo CORS, revisar origen exacto, HTTPS y URL incorporada al build.
- Revisar logs de Container Apps, métricas RU de Cosmos y presupuesto.
- Escalado a cero implica arranque en frío. Esta base no incorpora alertas,
  recuperación automatizada ni pruebas contra Azure.

Actualizar `api_image` y aplicar Terraform para nuevas versiones; publicar de
nuevo `dist` al cambiar el frontend. Para retirar el entorno, revisar el plan de
`terraform destroy`: **elimina también los datos Cosmos**. Exportar/custodiar datos
y comprobar las políticas de recuperación antes de confirmar.
