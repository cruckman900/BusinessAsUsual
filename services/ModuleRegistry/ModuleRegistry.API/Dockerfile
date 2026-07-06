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

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ModuleRegistry.API.dll"]
