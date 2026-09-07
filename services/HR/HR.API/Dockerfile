# ============================
# BUILD STAGE
# ============================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files (respecting the HR dependency graph) for a cached restore
COPY services/HR/HR.API/HR.API.csproj services/HR/HR.API/
COPY services/HR/HR.Application/HR.Application.csproj services/HR/HR.Application/
COPY services/HR/HR.Infrastructure/HR.Infrastructure.csproj services/HR/HR.Infrastructure/
COPY services/HR/HR.Contracts/HR.Contracts.csproj services/HR/HR.Contracts/
COPY services/HR/HR.Domain/HR.Domain.csproj services/HR/HR.Domain/
COPY BusinessAsUsual.Core/BusinessAsUsual.Core.csproj BusinessAsUsual.Core/
COPY backend/BusinessAsUsual.Application/BusinessAsUsual.Application.csproj backend/BusinessAsUsual.Application/
COPY backend/BusinessAsUsual.Domain/BusinessAsUsual.Domain.csproj backend/BusinessAsUsual.Domain/
COPY backend/BusinessAsUsual.Infrastructure/BusinessAsUsual.Infrastructure.csproj backend/BusinessAsUsual.Infrastructure/
COPY services/Platform/Platform.Infrastructure/Platform.Infrastructure.csproj services/Platform/Platform.Infrastructure/
COPY services/Platform/Platform.Application/Platform.Application.csproj services/Platform/Platform.Application/
COPY services/Platform/Platform.Domain/Platform.Domain.csproj services/Platform/Platform.Domain/

# Restore
RUN dotnet restore services/HR/HR.API/HR.API.csproj

# Copy source
COPY services/HR/HR.API services/HR/HR.API
COPY services/HR/HR.Application services/HR/HR.Application
COPY services/HR/HR.Infrastructure services/HR/HR.Infrastructure
COPY services/HR/HR.Contracts services/HR/HR.Contracts
COPY services/HR/HR.Domain services/HR/HR.Domain
COPY BusinessAsUsual.Core BusinessAsUsual.Core
COPY backend/BusinessAsUsual.Application backend/BusinessAsUsual.Application
COPY backend/BusinessAsUsual.Domain backend/BusinessAsUsual.Domain
COPY backend/BusinessAsUsual.Infrastructure backend/BusinessAsUsual.Infrastructure
COPY services/Platform/Platform.Infrastructure services/Platform/Platform.Infrastructure
COPY services/Platform/Platform.Application services/Platform/Platform.Application
COPY services/Platform/Platform.Domain services/Platform/Platform.Domain

# Publish
RUN dotnet publish services/HR/HR.API/HR.API.csproj -c Release -o /app/publish

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
ENTRYPOINT ["dotnet", "HR.API.dll"]
