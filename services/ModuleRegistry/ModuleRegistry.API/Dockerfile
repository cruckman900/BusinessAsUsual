# ============================
# BUILD STAGE
# ============================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files (respecting the ModuleRegistry dependency graph) for a cached restore
COPY services/ModuleRegistry/ModuleRegistry.API/ModuleRegistry.API.csproj services/ModuleRegistry/ModuleRegistry.API/
COPY services/ModuleRegistry/ModuleRegistry.Application/ModuleRegistry.Application.csproj services/ModuleRegistry/ModuleRegistry.Application/
COPY services/ModuleRegistry/ModuleRegistry.Infrastructure/ModuleRegistry.Infrastructure.csproj services/ModuleRegistry/ModuleRegistry.Infrastructure/
COPY services/ModuleRegistry/ModuleRegistry.Domain/ModuleRegistry.Domain.csproj services/ModuleRegistry/ModuleRegistry.Domain/

# Restore
RUN dotnet restore services/ModuleRegistry/ModuleRegistry.API/ModuleRegistry.API.csproj

# Copy source
COPY services/ModuleRegistry/ModuleRegistry.API services/ModuleRegistry/ModuleRegistry.API
COPY services/ModuleRegistry/ModuleRegistry.Application services/ModuleRegistry/ModuleRegistry.Application
COPY services/ModuleRegistry/ModuleRegistry.Infrastructure services/ModuleRegistry/ModuleRegistry.Infrastructure
COPY services/ModuleRegistry/ModuleRegistry.Domain services/ModuleRegistry/ModuleRegistry.Domain

# Publish
RUN dotnet publish services/ModuleRegistry/ModuleRegistry.API/ModuleRegistry.API.csproj -c Release -o /app/publish

# ============================
# RUNTIME STAGE
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# curl is used by the container HEALTHCHECK below
RUN apt-get update && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 80

# Container-level health probe (ECS/ALB also probe /health independently)
HEALTHCHECK --interval=30s --timeout=5s --start-period=20s --retries=3 \
    CMD curl -fsS http://localhost:80/health || exit 1

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ModuleRegistry.API.dll"]
