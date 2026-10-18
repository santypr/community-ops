FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source
COPY global.json Directory.Build.props ./
COPY src/backend/CommunityOps.Domain/ src/backend/CommunityOps.Domain/
COPY src/backend/CommunityOps.Application/ src/backend/CommunityOps.Application/
COPY src/backend/CommunityOps.Infrastructure/ src/backend/CommunityOps.Infrastructure/
COPY src/backend/CommunityOps.Api/ src/backend/CommunityOps.Api/
RUN dotnet publish src/backend/CommunityOps.Api/CommunityOps.Api.csproj -c Release -o /app /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app .
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "CommunityOps.Api.dll"]
